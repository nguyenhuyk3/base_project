using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Models;
using Motel.Utility.Database;
using Motel.ViewModels;

namespace Motel.Areas.UserAccount.Controllers
{
    [Area("UserAccount")]
    public class UserAccountController : Controller
    {
        private readonly DatabaseConstructor _databaseConstuctor;

        public UserAccountController(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstuctor = new DatabaseConstructor(databaseSettings);
        }

        public void DeleteCookie()
        {
            Response.Cookies.Delete("id");
            Response.Cookies.Delete("email");
        }

        [HttpGet]
        public IActionResult Login()
        {
            DeleteCookie();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserAccount model)
        {
            if (ModelState.IsValid)
            {
                var userAccount = await _databaseConstuctor
                                        .UserAccountCollection
                                        .Find(user => user.Email == model.Email)
                                        .FirstOrDefaultAsync();

                if (userAccount != null && userAccount.Password == model.Password)
                {
                    var cookie = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        // Will find out later
                        HttpOnly = true,
                    };
                    
                    // Cookie
                    var keyValuePairs = new[]
                    {
                        new KeyValuePair<string, string>("id", userAccount.Id.ToString()),
                        new KeyValuePair<string, string>("email", userAccount.Email)
                    };

                    Response.Cookies.Append(keyValuePairs, cookie);

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu không đúng");

                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            DeleteCookie();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisteredUserAccount model)
        {

            if (ModelState.IsValid)
            {
                var existingUser = await _databaseConstuctor
                                        .UserAccountCollection
                                        .Find(user => user.Email == model.Email)
                                        .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");

                    return View();
                }

                // I will convert "Customer" to id later
                var role = await _databaseConstuctor
                                .RoleCollection
                                .Find(r => r.Name == "Customer")
                                .FirstOrDefaultAsync();

                Motel.Models.UserAccount userAccount = new Motel.Models.UserAccount()
                {
                    Email = model.Email,
                    Password = model.Password
                };

                Info info = new Info()
                {
                    FullName = model.FullName,
                    BirthDay = model.BirthDay,
                    Sex = model.Sex,
                    Email = model.Email,
                };

                userAccount.Info = info;

                if (role != null)
                {
                    userAccount.Role = role.Id.ToString();
                }
                else
                {
                    var newRole = new Role
                    {
                        Name = "Customer"
                    };

                    await _databaseConstuctor.RoleCollection.InsertOneAsync(newRole);

                    userAccount.Role = newRole.Id.ToString();
                }

                await _databaseConstuctor.UserAccountCollection.InsertOneAsync(userAccount);

                var cookie = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    // Will find out later
                    HttpOnly = true
                };

                // Cookie
                var keyValuePairs = new[]
                {
                    new KeyValuePair<string, string>("id", userAccount.Id.ToString()),
                    new KeyValuePair<string, string>("email", userAccount.Email)
                };

                Response.Cookies.Append(keyValuePairs, cookie);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }
    }
}
