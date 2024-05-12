using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Areas.UserAccount.Models;
using Motel.Models;
using Motel.Utility.Database;
using System.Security.Claims;

namespace Motel.Areas.UserAccount.Controllers
{
    [Area("UserAccount")]
    public class UserAccount : Controller
    {
        private readonly DatabaseConstructor _databaseConstuctor;

        public UserAccount(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstuctor = new DatabaseConstructor(databaseSettings);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
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

                Motel.Models.UserAccount userAccount = new Motel.Models.UserAccount()
                {
                    Email = model.Email,
                    Password = model.Password
                };

                Info info = new Info()
                {
                    FullName = model.FullName,
                    DayOfBirth = model.DayOfBirth,
                    Sex = model.Sex,
                    Email = model.Email,
                };

                userAccount.Info = info;

                // I will convert "Customer" to id later
                var role = await _databaseConstuctor
                                .RoleCollection
                                .Find(r => r.Name == "Customer")
                                .FirstOrDefaultAsync();

                if (role != null)
                {
                    userAccount.Role = role.Name;
                }
                else
                {
                    var newRole = new Role
                    {
                        Name = "Customer"
                    };

                    await _databaseConstuctor.RoleCollection.InsertOneAsync(newRole);

                    userAccount.Role = newRole.Name.ToString();
                }

                await _databaseConstuctor.UserAccountCollection.InsertOneAsync(userAccount);
                

                return RedirectToAction("Login", "UserAccount");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
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
                    /*
                        A claim is a statement about a subject by an issuer and    
                        represent attributes of the subject that are useful 
                        in the context of authentication and authorization operations.
                    */
                    var claims = new List<Claim>() {
                        new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
                        new Claim(ClaimTypes.Name, userAccount.Email),
                        new Claim(ClaimTypes.Role, userAccount.Role),
                    };

                    // Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    //  //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                    var principal = new ClaimsPrincipal(identity);

                    //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                    {
                        IsPersistent = model.RememberMe
                    });

                    return RedirectToAction("Index", "Home", new { area = "Post" });
                }
                else
                {
                    ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu không đúng");

                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home", new {area = "Post"});
        }
    }
}
