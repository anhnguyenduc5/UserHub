using Humanizer.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Project_MVC.Models;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Project_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public AccountController(UserManager<AspNetUser> userManager, HttpClient httpClient, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _httpClient = httpClient;
            _userManager = userManager;
            _configuration = configuration;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _environment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(model.AvatarImg.FileName);
                string extension = Path.GetExtension(model.AvatarImg.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string imagePath = Path.Combine(wwwRootPath + "/images/", fileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    model.AvatarImg.CopyTo(fileStream);
                }

                model.Avatar = "~/images/" + fileName;

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("http://localhost:5100/api/Users/register", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                    return RedirectToAction("Register");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, responseString);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("http://localhost:5100/api/Users/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
                    HttpContext.Response.Cookies.Append("JwtToken", tokenResponse.Token);

                    var userId = GetUserIdFromToken(tokenResponse.Token);
                    var roleId = GetRoleIdFromToken(tokenResponse.Token);
                    HttpContext.Session.SetInt32("UserId", int.Parse(userId));
                    HttpContext.Session.SetInt32("RoleId", int.Parse(roleId));

                    return RedirectToAction("Index", "Home", new { userId = userId });
                }

                var responseString = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, responseString);
            }
            return View(model);
        }
        public string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return null;

            var userId = jwtToken.Claims.First(claim => claim.Type == "nameid").Value;
            return userId;
        }
        public string GetRoleIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return null;

            // Lấy roleId từ claim
            var roleId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role")?.Value;
            return roleId;
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Invalidate the token by deleting the cookie
            HttpContext.Response.Cookies.Delete("JwtToken");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RequestPasswordReset()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RequestPasswordReset(string email)
        {
            var apiUrl = "http://localhost:5100/api/PasswordReset/send"; // Thay đổi URL nếu cần

            // Tạo đối tượng email request
            var emailRequest = new
            {
                Email = email
            };

            // Chuyển đổi đối tượng thành JSON
            var content = new StringContent(JsonConvert.SerializeObject(emailRequest), Encoding.UTF8, "application/json");

            // Gửi yêu cầu POST đến API gửi email
            var response = await _httpClient.PostAsync(apiUrl, content);

            // Xử lý phản hồi từ API
            if (response.IsSuccessStatusCode)
            {
                // Nếu thành công, chuyển hướng đến trang xác nhận
                return RedirectToAction("PasswordResetConfirmation");
            }

            // Nếu thất bại, thêm lỗi vào ModelState và trả về view hiện tại
            ModelState.AddModelError(string.Empty, "Failed to send email.");
            return View();
        }

        public IActionResult PasswordResetConfirmation()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
