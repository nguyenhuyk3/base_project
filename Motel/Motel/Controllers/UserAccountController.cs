using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using WebProject.Models;
using WebProject.Utility.Database;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace WebProject.Controllers
{
    public class UserAccountController : Controller
    {
        private readonly IMongoCollection<UserAccount> _userAccountCollection;
        private readonly IMongoCollection<Role> _roleCollection;

        public UserAccountController(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            _userAccountCollection = database.GetCollection<UserAccount>
                (databaseSettings.Value.UserAccountsCollectionName);
            _roleCollection = database.GetCollection<Role>
                (databaseSettings.Value.RolesCollectionName);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var userAccount = await _userAccountCollection
                                        .Find(user => user.Email == email)
                                        .FirstOrDefaultAsync();

                if (userAccount != null && userAccount.Password == password)
                {
                    var cookie = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        // Will find out later
                        HttpOnly = true
                    };

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

                    return View(userAccount);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(
            string email,
            string password, string confirmPassword)
        {
            try
            {
                var existingUser = await _userAccountCollection
                                        .Find(user => user.Email == email)
                                        .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");

                    return View();
                }

                if (password != confirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu và mật khẩu xác nhận không khớp");

                    return View();
                }

                if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("AccountName", "Tài khoản đăng ký phải có @gmail.com");

                    return View();
                }

                var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");

                if (!passwordRegex.IsMatch(password))
                {
                    ModelState.AddModelError("Password",
                                                "Mật khẩu phải có ít nhất 8 ký tự, " +
                                                "bao gồm ít nhất một chữ thường, " +
                                                "một số và một ký tự đặc biệt.");

                    return View();
                }

                // I will convert "Customer" to id later
                var role = await _roleCollection.Find(r => r.Name == "Customer").FirstOrDefaultAsync();

                UserAccount userAccount = new UserAccount()
                {
                    Email = email,
                    Password = password
                };

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

                    await _roleCollection.InsertOneAsync(newRole);

                    userAccount.Role = newRole.Id.ToString();
                }

                await _userAccountCollection.InsertOneAsync(userAccount);

                var cookie = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    // Will find out later
                    HttpOnly = true
                };


                var keyValuePairs = new[]
                {
                        new KeyValuePair<string, string>("id", userAccount.Id.ToString()),
                        new KeyValuePair<string, string>("email", userAccount.Email)
                    };

                Response.Cookies.Append(keyValuePairs, cookie);

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi:{ex.Message}");
            }
        }
    }
}
