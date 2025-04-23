using HomeOwners.Web.UI.Clients.Authentication;
using HomeOwners.Web.UI.Clients.Authentication.Requests;
using HomeOwners.Web.UI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Refit;
using System.Net;
using HomeOwners.Web.UI.ResponseModels;
using Microsoft.AspNetCore.Authorization;

namespace HomeOwners.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthenticationClient _authenticationClient;

        public HomeController(ILogger<HomeController> logger, IAuthenticationClient authenticationClient)
        {
            _logger = logger;
            _authenticationClient = authenticationClient;
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _authenticationClient.LoginAsync(new AuthenticateRequest() { Username = model.Username, Password = model.Password });

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                await HttpContext.SignInAsync(
                    new ClaimsPrincipal(
                        new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
                    )
                );

            }
            catch (ApiException ex)
            {
                switch (ex.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        var errorResponse = await ex.GetContentAsAsync<BadRequestResponseModel>();

                        foreach (var item in errorResponse.ValidationErrors)
                        {
                            ModelState.AddModelError(string.Empty, item.Message);
                        }

                        break;

                    case HttpStatusCode.NotFound:
                        ModelState.AddModelError(string.Empty, "Invalid username or password");
                        break;

                    default:
                        _logger.LogError(ex.Message);
                        break;
                }
                return View(model);
            }


            return RedirectToAction(nameof(HomeController.Index));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
