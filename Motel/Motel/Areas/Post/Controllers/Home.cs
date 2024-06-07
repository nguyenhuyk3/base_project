using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Utility.Database;
using Motel.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using X.PagedList;

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
        public async Task<IActionResult> Index(int? page)
        {
            List<Motel.Models.Post> posts = new List<Motel.Models.Post>();

            if (TempData["posts"] != null)
            {
                posts = posts = JsonConvert.DeserializeObject<List<Motel.Models.Post>>(TempData["posts"].ToString());
            }
            else
            {
                posts = await _databaseConstructor.PostCollection
                                                    .Find(_ => true)
                                                    .ToListAsync();
            }

            if (page == null)
            {
                page = 1;
            }

            int pageSize = 1;
            int pageNum = page ?? 1;

            var pagedPosts = posts?.ToPagedList(pageNum, pageSize);

            return View(pagedPosts);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubjectOnSite()
        {
            try
            {
                var subjects = await _databaseConstructor.PostCollection
                                                            .Find(_ => true)
                                                            .Project(p => p.SubjectOnSite)
                                                            .ToListAsync();
                var addresses = await _databaseConstructor.PostCollection
                                                            .Find(_ => true)
                                                            .Project(f => f.PostDetail.AddressDetail.Address)
                                                            .ToListAsync();
                var distinctSubjects = subjects.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                var distinctAddresses = addresses.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                var combinedList = new List<string>();

                combinedList.AddRange(distinctSubjects);
                combinedList.AddRange(distinctAddresses);

                return Json(combinedList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchBySubjectOnSite(string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    return RedirectToAction("Index");
                }

                var postsBySubjectOnSite = await _databaseConstructor.PostCollection
                                                                        .Find(p => p.SubjectOnSite.Contains(term))
                                                                        .ToListAsync();
                var postByAddress = await _databaseConstructor.PostCollection
                                                                .Find(p => p.PostDetail.AddressDetail.Address.Contains(term))
                                                                .ToListAsync();
                var combinedList = new List<Motel.Models.Post>();

                combinedList.AddRange(postsBySubjectOnSite);
                combinedList.AddRange(postByAddress);
                combinedList = combinedList
                                    .GroupBy(p => p.Id)
                                    .Select(g => g.First())
                                    .ToList();

                TempData["posts"] = JsonConvert.SerializeObject(combinedList);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
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
                var userAccountDoc = await _databaseConstructor.UserAccountCollection
                                                                .Find(f => f.Id == userAccountId)
                                                                .FirstOrDefaultAsync();
                var postDoc = await _databaseConstructor.PostCollection
                                                            .Find(p => p.Id == postId)
                                                            .FirstOrDefaultAsync();

                userAccountDoc.PreferenceList ??= new List<Motel.Models.Post>();

                if (userAccountDoc.PreferenceList.All(p => p.Id != postId))
                {
                    userAccountDoc.PreferenceList.Add(postDoc);

                    postDoc.IsLiked = true;

                    var updateDefinition = Builders<Motel.Models.UserAccount>
                                                    .Update
                                                    .Set(u => u.PreferenceList, userAccountDoc.PreferenceList);

                    await _databaseConstructor.UserAccountCollection
                                                .UpdateOneAsync(u => u.Id == userAccountId, updateDefinition);

                    var favoritePost = new
                    {
                        PostId = postId,
                        Img = postDoc.PostDetail.Images[0].Url,
                        SubjectOnSite = postDoc.SubjectOnSite,
                        CreatedAt = DateTime.Now,
                    };

                    return Json(new { favoritePost = favoritePost, success = true });
                }

                return Json(new { success = false });
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
                var postDoc = await _databaseConstructor.PostCollection
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

                return Json(new { success = false });
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

        class FavoritePost
        {
            public string PostId { get; set; }
            public string Img { get; set; }
            public string SubjectOnSite { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
        public async Task<JsonResult> GetAllFavoritePosts()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(ownerId))
            {
                return Json(new { success = false, count = 0 });
            }

            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == ownerId)
                                                        .FirstOrDefaultAsync();

            if (ownerDoc.PreferenceList == null)
            {
                return Json(new { success = false, count = 0 });
            }
            else
            {
                List<FavoritePost> favoritePosts = new List<FavoritePost>();

                foreach (var post in ownerDoc.PreferenceList)
                {
                    FavoritePost favoritePost = new FavoritePost()
                    {
                        PostId = post.Id,
                        Img = post.PostDetail.Images[0].Url,
                        SubjectOnSite = post.SubjectOnSite,
                        CreatedAt = DateTime.Now,
                    };

                    favoritePosts.Add(favoritePost);
                }

                favoritePosts.Reverse();

                return Json(new { favoritePosts = favoritePosts, success = true, count = favoritePosts.Count });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ShowFavoritePosts()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(ownerId))
            {
                return Json(new { success = false });
            }

            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                 .Find(f => f.Id == ownerId)
                                                 .FirstOrDefaultAsync();

            if (ownerDoc.PreferenceList == null)
            {
                return Json(new { success = false, count = 0 });
            }

            return Json(new
            {
                favoritePosts = ownerDoc.PreferenceList,
                count = ownerDoc.PreferenceList.Count,
                sucess = true
            });
        }
    }
}
