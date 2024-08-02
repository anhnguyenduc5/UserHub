using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project_MVC.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Project_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly PRN231_1Context _context;
        private readonly IWebHostEnvironment _environment;

        public HomeController(PRN231_1Context context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return View("Error");
            }

            string link = $"http://localhost:5100/api/Users/{userId}";

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(link))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        using (HttpContent content = res.Content)
                        {
                            string data = await content.ReadAsStringAsync();
                            var user = JsonConvert.DeserializeObject<dynamic>(data);
                            ViewBag.User = user;
                        }
                    }
                    else
                    {
                        return View("Error");
                    }
                }
            }
            return View();
        }

        [HttpPut]
        public async Task<IActionResult> EditUser([FromForm] UpdateUserModel model)
        {

            string link = $"http://localhost:5100/api/Users/{model.UserId}";
            string wwwRootPath = _environment.WebRootPath;
            if (model.AvatarImg != null && model.AvatarImg.Length > 0)
            {
                string fileName = Path.GetFileNameWithoutExtension(model.AvatarImg.FileName);
                string extension = Path.GetExtension(model.AvatarImg.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string imagePath = Path.Combine(wwwRootPath + "/images/", fileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await model.AvatarImg.CopyToAsync(fileStream);
                }

                model.Avatar = "~/images/" + fileName;
            }

            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(new
                {
                    UserId = model.UserId,
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role,
                    FullName = model.FullName,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth,
                    Avatar = model.Avatar
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpResponseMessage res = await client.PutAsync(link, content))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", new { userId = model.UserId });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Update failed.");
                    }
                }
            }
            return RedirectToAction("Index", new {model.UserId});
        }
    }
}
