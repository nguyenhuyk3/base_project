using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Utility.Database;
using Motel.ViewModels;
using System.Security.Claims;

namespace Motel.Areas.Post.Controllers
{
    [Area("Post")]
    public class Home : Controller
    {
        private readonly DatabaseConstructor _databaseConstructor;

        public Home(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var posts = await _databaseConstructor.PostCollection
                                                    .Find(_ => true)
                                                    .ToListAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewData["UserAccountId"] = userId;

            return View(posts);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(string postId)
        {
            var userAccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userAccountId))
            {
                return Unauthorized();
            }

            try
            {
                var userAccount = await _databaseConstructor.UserAccountCollection
                                                                .Find(f => f.Id == userAccountId)
                                                                .FirstOrDefaultAsync();
                var post = await _databaseConstructor.PostCollection
                                                        .Find(p => p.Id == postId)
                                                        .FirstOrDefaultAsync();

                userAccount.PreferenceList ??= new List<Motel.Models.Post>();

                if (userAccount.PreferenceList.All(p => p.Id != postId))
                {
                    userAccount.PreferenceList.Add(post);

                    post.IsLiked = true;

                    var updateDefinition = Builders<Motel.Models.UserAccount>
                                                    .Update
                                                    .Set(u => u.PreferenceList, userAccount.PreferenceList);

                    await _databaseConstructor.UserAccountCollection
                                                .UpdateOneAsync(u => u.Id == userAccountId, updateDefinition);
                }

                return Json(new { success = true, message = "Bài viết đã được thêm danh sách yêu thích" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(string postId)
        {
            var userAccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var userAccount = await _databaseConstructor.UserAccountCollection
                                                                .Find(f => f.Id == userAccountId)
                                                                .FirstOrDefaultAsync();
                var post = await _databaseConstructor.PostCollection
                                                        .Find(p => p.Id == postId)
                                                        .FirstOrDefaultAsync();

                userAccount.PreferenceList.RemoveAll(p => p.Id == postId);

                if (userAccount.PreferenceList.Count == 0)
                {
                    userAccount.PreferenceList = null;
                }

                var updateDefinition = Builders<Motel.Models.UserAccount>
                                        .Update
                                        .Set(u => u.PreferenceList, userAccount.PreferenceList);

                await _databaseConstructor.UserAccountCollection
                                            .UpdateOneAsync(u => u.Id == userAccountId, updateDefinition);

                return Json(new { success = true, message = "Bài viết đã được xóa khỏi danh sách yêu thích" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<IActionResult> FavoritePosts()
        {
            var userAccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewData["UserId"] = userAccountId;

            if (string.IsNullOrEmpty(userAccountId))
            {
                return RedirectToAction("Login", "UserAccount");
            }

            try
            {
                var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                                .Find(f => f.Id == userAccountId)
                                                                .FirstOrDefaultAsync();
                var favoritePosts = ownerDoc.PreferenceList?
                                                        .GroupBy(f => f.Id)
                                                        .Select(f => f.First())
                                                        .ToList();

                ModificationLayoutViewModel model = new ModificationLayoutViewModel()
                {
                    Owner = ownerDoc,
                    FavoritePosts = favoritePosts
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
