using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project_MVC.Models;

namespace Project_MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public UserController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /*public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:5100/api/Users");

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<dynamic>>(responseString);
                ViewBag.Users = users;
            }
            else
            {
                // Handle error response
                return View("Error");
            }
            return View();
        }*/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DeletedUser()
        {
            return View();
        }
    }
}
