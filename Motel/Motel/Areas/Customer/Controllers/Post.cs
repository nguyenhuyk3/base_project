using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Utility.Database;
using Motel.ViewModels;
using System.Security.Claims;

namespace Motel.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class Post : Controller
    {
        private readonly DatabaseConstructor _databaseConstructor;

        public Post(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        public async Task<List<Motel.Models.Post>> GetPostsByIds(List<string> postIds)
        {
            var filter = Builders<Motel.Models.Post>.Filter.In(post => post.Id, postIds);
            var posts = await _databaseConstructor.PostCollection.Find(filter).ToListAsync();

            return posts;
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
        public async Task<IActionResult> PostingList()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                      .Find(f => f.Id == ownerId)
                                                      .FirstOrDefaultAsync();
            ownerDoc.Posts ??= new List<string>();

            var posts = await GetPostsByIds(ownerDoc.Posts);

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                Posts = posts,
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
        public async Task<IActionResult> ViolatedPosts()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                      .Find(f => f.Id == ownerId)
                                                      .FirstOrDefaultAsync();
            ownerDoc.Posts ??= new List<string>();

            var posts = await GetPostsByIds(ownerDoc.Posts);
            var violatedPosts = posts.Where(post => post.State.IsViolated == true).ToList();

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                Posts = violatedPosts,
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
        public async Task<IActionResult> ExpiredPosts()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                      .Find(f => f.Id == ownerId)
                                                      .FirstOrDefaultAsync();
            ownerDoc.Posts ??= new List<string>();

            var posts = await GetPostsByIds(ownerDoc.Posts);
            var expiredPosts = posts.Where(post => post.ExpiredAt <= DateTime.Now 
                                                && post.State.IsDeleted == false).ToList();

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                Posts = expiredPosts,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> ExtendPost(string postId, string vipName)
        {
            var postDoc = await _databaseConstructor.PostCollection
                                                    .Find(f => f.Id == postId)
                                                    .FirstOrDefaultAsync();

            if (postDoc.ExpiredAt >= DateTime.Now)
            {
                return Json(new { expired = false, message = "Bài viết còn hạn!" });
            }

            var vipDoc = await _databaseConstructor.VipCollection
                                                     .Find(f => f.Name == vipName)
                                                     .FirstOrDefaultAsync();
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == postDoc.OwnerId)
                                                        .FirstOrDefaultAsync();

            if (ownerDoc.Balance < vipDoc.Cost)
            {
                return Json(new { insufficientBalance = true, message = "Số dư của bạn không đủ!" });
            }

            ownerDoc.Balance -= vipDoc.Cost;
            postDoc.ExpiredAt = postDoc.ExpiredAt.AddDays(vipDoc.Days);

            var postDocUpdateDefinition = Builders<Motel.Models.Post>.Update.Set(f => f.ExpiredAt, postDoc.ExpiredAt);
            var ownerDocUpdateDefinition = Builders<Motel.Models.UserAccount>.Update.Set(f => f.Balance, ownerDoc.Balance);
            var postUpdateResult = await _databaseConstructor.PostCollection
                                                                .UpdateOneAsync(f => f.Id == postDoc.Id, postDocUpdateDefinition);
            var ownerUpdateResult = await _databaseConstructor.UserAccountCollection
                                                                .UpdateOneAsync(f => f.Id == ownerDoc.Id, ownerDocUpdateDefinition);

            if (postUpdateResult.ModifiedCount > 0 && ownerUpdateResult.ModifiedCount > 0)
            {
                return Json(new { expired = true, message = "Gia hạn bài viết thành công!" });
            }
            else
            {
                return Json(new { expired = false, success = false, message = "Gia hạn bài thất bại!" });
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
        public async Task<IActionResult> DeletedPosts()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                      .Find(f => f.Id == ownerId)
                                                      .FirstOrDefaultAsync();
            ownerDoc.Posts ??= new List<string>();

            var posts = await GetPostsByIds(ownerDoc.Posts);
            var deletedPosts = posts.Where(post => post.State.IsDeleted == true).ToList();

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                Posts = deletedPosts,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var postDoc = await _databaseConstructor.PostCollection
                                                    .Find(f => f.Id == postId)
                                                    .FirstOrDefaultAsync();
            var postDocUpdateDefinition = Builders<Motel.Models.Post>.Update.Set(f => f.State.IsDeleted, true);

            await _databaseConstructor.PostCollection
                                        .UpdateOneAsync(f => f.Id == postId, postDocUpdateDefinition);

            return Json(new { success = true, message = "Xóa thành công bài viêt" });
        }
    }
}
