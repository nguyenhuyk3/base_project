using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Areas.Post.Models;
using Motel.Utility.Comparer;
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
        private readonly Motel.Utility.Sort.Post _postSort;

        public Home(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
            _postSort = new Utility.Sort.Post(_databaseConstructor);
        }

        [HttpGet]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int? page, string? term)
        {
            var userAccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favoritePosts = new List<string>();

            Motel.Models.UserAccount userAccountDoc = null;

            if (!string.IsNullOrEmpty(userAccountId))
            {
                userAccountDoc = await _databaseConstructor.UserAccountCollection
                                                                .Find(f => f.Id == userAccountId)
                                                                .FirstOrDefaultAsync();
                if (userAccountDoc.FavoriteList != null)
                {
                    favoritePosts = userAccountDoc.FavoriteList;
                }
            }

            List<Motel.Models.Post> posts = new List<Motel.Models.Post>();

            if (string.IsNullOrEmpty(term))
            {
                posts = await _databaseConstructor.PostCollection
                                                    .Find(_ => true)
                                                    .ToListAsync();
            }
            else
            {
                var postBySubjectOnSite = await _databaseConstructor.PostCollection
                                                    .Find(f => f.SubjectOnSite.Contains(term))
                                                    .ToListAsync();
                var postByAddress = await _databaseConstructor.PostCollection
                                                                .Find(f => f.PostDetail.AddressDetail.Address.Contains(term))
                                                                .ToListAsync();

                var combinedPosts = postBySubjectOnSite
                                        .Concat(postByAddress)
                                        .Distinct(new PostEqualityComparer())
                                        .ToList();

                posts = combinedPosts;
            }

            if (page == null)
            {
                page = 1;
            }

            var pageSize = 1;
            var pageNum = page ?? 1;
            var pagedPosts = posts?.ToPagedList(pageNum, pageSize);

            PostIndex model = new PostIndex()
            {
                Posts = pagedPosts,
                FavoritePosts = favoritePosts,
                CurrentTerm = term
            };

            return View(model);
        }

        //[AllowAnonymous]
        //public async Task<IActionResult> Index(int? page)
        //{
        //    var userAccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    Motel.Models.UserAccount userAccountDoc = null;

        //    List<string> favoritePosts = new List<string>();

        //    if (!string.IsNullOrEmpty(userAccountId))
        //    {
        //        userAccountDoc = await _databaseConstructor.UserAccountCollection
        //                                                        .Find(f => f.Id == userAccountId)
        //                                                        .FirstOrDefaultAsync();
        //        if (userAccountDoc.FavoriteList != null)
        //        {
        //            favoritePosts = userAccountDoc.FavoriteList;
        //        }
        //    }

        //    List<Motel.Models.Post> posts = new List<Motel.Models.Post>();

        //    if (TempData["posts"] != null)
        //    {
        //        posts = posts = JsonConvert.DeserializeObject<List<Motel.Models.Post>>(TempData["posts"].ToString());
        //    }
        //    else
        //    {
        //        posts = await _databaseConstructor.PostCollection
        //                                            .Find(_ => true)
        //                                            .ToListAsync();
        //    }

        //    posts = await _postSort.SortPosts(posts);

        //    if (page == null)
        //    {
        //        page = 1;
        //    }

        //    var pageSize = 1;
        //    var pageNum = page ?? 1;
        //    var pagedPosts = posts?.ToPagedList(pageNum, pageSize);

        //    PostIndex model = new PostIndex()
        //    {
        //        Posts = pagedPosts,
        //        FavoritePosts = favoritePosts,
        //    };

        //    return View(model);
        //}

        [HttpGet]
        public async Task<IActionResult> GetTerm()
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
                var combinedSet = new HashSet<string>(distinctSubjects);

                combinedSet.UnionWith(distinctAddresses);

                var combinedList = combinedSet.ToList();

                return Json(combinedList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SearchBySubjectOnSite(string term)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(term))
        //        {
        //            return RedirectToAction("Index");
        //        }

        //        var postsBySubjectOnSite = await _databaseConstructor.PostCollection
        //                                                                .Find(p => p.SubjectOnSite.Contains(term))
        //                                                                .ToListAsync();
        //        var postByAddress = await _databaseConstructor.PostCollection
        //                                                        .Find(p => p.PostDetail.AddressDetail.Address.Contains(term))
        //                                                        .ToListAsync();
        //        var combinedList = new List<Motel.Models.Post>();

        //        combinedList.AddRange(postsBySubjectOnSite);
        //        combinedList.AddRange(postByAddress);
        //        combinedList = combinedList
        //                            .GroupBy(p => p.Id)
        //                            .Select(g => g.First())
        //                            .ToList();

        //        TempData["posts"] = JsonConvert.SerializeObject(combinedList);

        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        //    }
        //}

        [HttpPost]
        [Authorize(Policy = "RequireCustomer")]
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

                userAccountDoc.FavoriteList ??= new List<string>();

                if (!userAccountDoc.FavoriteList.Contains(postId))
                {
                    userAccountDoc.FavoriteList.Add(postId);

                    var userAccountDocUpdateDefinition
                        = Builders<Motel.Models.UserAccount>
                                     .Update
                                     .Set(u => u.FavoriteList, userAccountDoc.FavoriteList);

                    await _databaseConstructor.UserAccountCollection
                                                .UpdateOneAsync(u => u.Id == userAccountId, userAccountDocUpdateDefinition);

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

                userAccountDoc.FavoriteList.RemoveAll(f => f == postId);

                if (userAccountDoc.FavoriteList.Count == 0)
                {
                    userAccountDoc.FavoriteList = null;
                }

                var userAccountDocUpdateDefinition
                                    = Builders<Motel.Models.UserAccount>
                                                .Update
                                                .Set(u => u.FavoriteList, userAccountDoc.FavoriteList);

                await _databaseConstructor.UserAccountCollection
                                            .UpdateOneAsync(u => u.Id == userAccountId, userAccountDocUpdateDefinition);

                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<List<Motel.Models.Post>> GetPostsByIds(List<string> postIds)
        {
            var filter = Builders<Motel.Models.Post>.Filter.In(post => post.Id, postIds);
            var posts = await _databaseConstructor.PostCollection.Find(filter).ToListAsync();

            return posts;
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
                var favoritePosts = await GetPostsByIds(ownerDoc.FavoriteList);

                ModificationLayoutViewModel model = new ModificationLayoutViewModel()
                {
                    Owner = ownerDoc,
                    FavoritePosts = favoritePosts,
                    FavoritePostIds = ownerDoc.FavoriteList
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
            public string PostId { get; set; } = null!;
            public string Img { get; set; } = null!;
            public string SubjectOnSite { get; set; } = null!;
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

            if (ownerDoc.FavoriteList == null)
            {
                return Json(new { success = false, count = 0 });
            }
            else
            {
                var favoritePostsOfOwner = await GetPostsByIds(ownerDoc.FavoriteList);
                var favoritePosts = new List<FavoritePost>();

                foreach (var post in favoritePostsOfOwner)
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

        //[HttpPost]
        //public async Task<JsonResult> ShowFavoritePosts()
        //{
        //    var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (string.IsNullOrEmpty(ownerId))
        //    {
        //        return Json(new { success = false });
        //    }

        //    var ownerDoc = await _databaseConstructor.UserAccountCollection
        //                                                .Find(f => f.Id == ownerId)
        //                                                .FirstOrDefaultAsync();

        //    if (ownerDoc.FavoriteList == null)
        //    {
        //        return Json(new { success = false, count = 0 });
        //    }

        //    return Json(new
        //    {
        //        favoritePosts = ownerDoc.PreferenceList,
        //        count = ownerDoc.PreferenceList.Count,
        //        sucess = true
        //    });
        //}
    }
}
