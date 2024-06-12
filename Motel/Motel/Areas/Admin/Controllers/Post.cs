using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Utility.Database;
using Motel.ViewModels;
using System.Security.Claims;

namespace Motel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class Post : Controller
    {
        private readonly DatabaseConstructor _databaseConstructor;

        public Post(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> Index()
        {
            var postFilterBuilder = Builders<Motel.Models.Post>.Filter;
            var postFilter = postFilterBuilder.Empty;

            postFilter &= postFilterBuilder.Eq(f => f.State.IsViolated, false);

            var posts = await _databaseConstructor.PostCollection
                                                    .Find(postFilter)
                                                    .ToListAsync();
            var userAccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == userAccountId)
                                                        .FirstOrDefaultAsync();

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                Posts = posts
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> ViolatedPosts()
        {
            var postFilterBuilder = Builders<Motel.Models.Post>.Filter;
            var postFilter = postFilterBuilder.Empty;

            postFilter &= postFilterBuilder.Eq(f => f.State.IsViolated, true);

            var posts = await _databaseConstructor.PostCollection
                                                    .Find(postFilter)
                                                    .ToListAsync();
            var userAccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == userAccountId)
                                                        .FirstOrDefaultAsync();

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                Posts = posts
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> ResponsePosts()
        {
            var postFilterBuilder = Builders<Motel.Models.Post>.Filter;
            var postFilter = postFilterBuilder.Empty;

            postFilter &= postFilterBuilder.Eq(f => f.State.IsEdited, true);

            var posts = await _databaseConstructor.PostCollection
                                                    .Find(postFilter)
                                                    .ToListAsync();
            var userAccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == userAccountId)
                                                        .FirstOrDefaultAsync();

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                Posts = posts
            };

            return View(model);
        }

        // Send warning 
        [HttpPost]
        public async Task<JsonResult> CreateWarning(string postId)
        {
            var postDoc = await _databaseConstructor.PostCollection
                                                        .Find(f => f.Id == postId)
                                                        .FirstOrDefaultAsync();
         
            postDoc.State.IsViolated = true;

            var updatePostDoc = Builders<Motel.Models.Post>.Update.Set(f => f.State, postDoc.State);
            var filterPostDoc = Builders<Motel.Models.Post>.Filter.Eq(f => f.Id, postDoc.Id);

            await _databaseConstructor.PostCollection.UpdateOneAsync(filterPostDoc, updatePostDoc);

            return Json(new { success = true, message = "Thành công" });
        }

        [HttpPost]
        public async Task<ActionResult> CreateAuthenticating(string postId)
        {

            var postDoc = await _databaseConstructor.PostCollection
                                                        .Find(f => f.Id == postId)
                                                        .FirstOrDefaultAsync();

            postDoc.State.IsAuthenticated = true;

            var updatePostDoc = Builders<Motel.Models.Post>.Update.Set(f => f.State, postDoc.State);
            var filterPostDoc = Builders<Motel.Models.Post>.Filter.Eq(f => f.Id, postDoc.Id);

            await _databaseConstructor.PostCollection.UpdateOneAsync(filterPostDoc, updatePostDoc);

            return Json(new { success = true, message = "Thành công" });
        }
    }
}
