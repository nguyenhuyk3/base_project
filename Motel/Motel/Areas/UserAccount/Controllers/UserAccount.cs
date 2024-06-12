using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Areas.UserAccount.Models;
using Motel.Models;
using Motel.Utility.Database;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Motel.Areas.UserAccount.Controllers
{
    [Area("UserAccount")]
    public class UserAccount : Controller
    {
        private readonly DatabaseConstructor _databaseConstuctor;
        private readonly IConfiguration _configuration;
        public UserAccount(IConfiguration configuration, IOptions<DatabaseSettings> databaseSettings)
        {
            _configuration = configuration;
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
                    // Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
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
                    userAccount.RoleName = role.Name;
                }
                else
                {
                    var newRole = new Role
                    {
                        Name = "Customer"
                    };

                    await _databaseConstuctor.RoleCollection.InsertOneAsync(newRole);

                    userAccount.RoleName = newRole.Name.ToString();
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

                //  if (userAccount != null && BCrypt.Net.BCrypt.Verify(model.Password, userAccount.Password))
                if (userAccount != null && userAccount.Password == model.Password)
                {
                    /*
                        A claim is a statement about a subject by an issuer and    
                        represent attributes of the subject that are useful 
                        in the context of authentication and authorization operations.
                    */
                    var claims = new List<Claim>() {
                        new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
                        new Claim(ClaimTypes.Name, userAccount.Info.FullName),
                        new Claim(ClaimTypes.Email, userAccount.Email),
                        new Claim(ClaimTypes.Role, userAccount.RoleName),
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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home", new { area = "Post" });
        }

        [HttpGet]
        public async Task<IActionResult> GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "UserAccount", null, Request.Scheme);
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }

            var claims = authenticateResult.Principal
                                            .Identities
                                            .FirstOrDefault()
                                            .Claims
                                            .Select(claim => new { claim.Type, claim.Value })
                                            .ToList();
            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var fullName = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            if (email != null)
            {
                var userAccount = _databaseConstuctor.UserAccountCollection
                                                        .Find(f => f.Email == email)
                                                        .FirstOrDefault();

                if (userAccount == null)
                {
                    userAccount = new Motel.Models.UserAccount
                    {
                        Info = new Info()
                        {
                            FullName = fullName,
                            Email = email
                        },
                        Email = email,
                        Password = BCrypt.Net.BCrypt.HashPassword("RandomPassword"),
                        RoleName = "Customer",

                    };

                    await _databaseConstuctor.UserAccountCollection.InsertOneAsync(userAccount);
                }

                var userClaims = new List<Claim>() {
                        new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
                        new Claim(ClaimTypes.Name, userAccount.Info.FullName),
                        new Claim(ClaimTypes.Email, userAccount.Email),
                        new Claim(ClaimTypes.Role, userAccount.RoleName),
                    };



                var claimsIdentity = new ClaimsIdentity(userClaims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            }

            return RedirectToAction("Index", "Home", new { area = "Post" });
        }

        private async Task SendResetPasswordEmail(string email, string token)
        {
            var smtpConfig = _configuration.GetSection("SMTPConfig");
            var senderAddress = smtpConfig["SenderAddress"];
            var senderDisplayName = smtpConfig["SenderDisplayName"];
            var password = smtpConfig["Password"];
            var host = smtpConfig["Host"];
            var port = int.Parse(smtpConfig["Port"]);
            var enableSsl = bool.Parse(smtpConfig["EnableSsl"]);

            var resetPasswordUrl = Url.Action("ResetPassword", "UserAccount", new { email = email, token = token }, protocol: HttpContext.Request.Scheme);

            var subject = "Yêu cầu đặt lại mật khẩu";
            var body = $"Vui lòng nhấp vào liên kết sau để đặt lại mật khẩu của bạn: <a href='{HtmlEncoder.Default.Encode(resetPasswordUrl)}'>Đặt lại mật khẩu</a>";

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = enableSsl;
                client.Credentials = new NetworkCredential(senderAddress, password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderAddress, senderDisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }

        private async Task SaveResetToken(string email, string token)
        {
            HttpContext.Session.SetString("ResetPasswordEmail", email);
            HttpContext.Session.SetString("ResetPasswordToken", token);
        }

        private string GenerateResetToken()
        {
            return Guid.NewGuid().ToString();
        }

        private bool IsValidResetToken(string email, string token)
        {
            var storedEmail = HttpContext.Session.GetString("ResetPasswordEmail");
            var storedToken = HttpContext.Session.GetString("ResetPasswordToken");

            return email == storedEmail && token == storedToken;
        }

        public IActionResult NotificationForgotPassword()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Email không được để trống.";
                return View();
            }

            var user = await _databaseConstuctor.UserAccountCollection
                                            .Find(u => u.Email == email)
                                            .FirstOrDefaultAsync();

            if (user == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy người dùng với địa chỉ email này.";
                return View();
            }

            string resetToken = GenerateResetToken();
            await SaveResetToken(email, resetToken);
            await SendResetPasswordEmail(email, resetToken);

            return RedirectToAction("NotificationForgotPassword");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (IsValidResetToken(email, token))
            {
                ViewBag.Email = email;
                ViewBag.Token = token;
                var model = new ResetPasswordViewModel { Email = email, Token = token };
                return View(model);
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid reset password token.";
                return View("ResetPasswordError");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không hợp lệ";

                return View();
            }

            if (!IsValidResetToken(model.Email, model.Token))
            {
                ViewBag.ErrorMessage = "Mã thông báo đặt lại mật khẩu không hợp lệ.";

                return View();
            }

            var user = await _databaseConstuctor.UserAccountCollection
                                                    .Find(u => u.Email == model.Email)
                                                    .FirstOrDefaultAsync();

            if (user == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy người dùng với địa chỉ email này.";

                return View();
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            user.Password = hashedPassword;
            user.ResetToken = null;

            await _databaseConstuctor.UserAccountCollection.ReplaceOneAsync(u => u.Id == user.Id, user);

            return RedirectToAction("Login");
        }
    }
}
