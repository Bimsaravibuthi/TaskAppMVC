using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskApp.Security;
using TaskAppMVC.API;
using TaskAppMVC.Models;

namespace TaskAppMVC.Controllers
{
    public class UserController : Controller
    {
        List<UserViewModel> userdetails = new List<UserViewModel>();

        string glob_UserLevel = "", glob_UserName = "";

        public readonly IConfiguration Configuration;
        public UserController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> ValidateAsync(LoginViewModel loginView, string returnUrl)
        {
            //if (!ModelState.IsValid) return View();

            if (UserValidate(loginView.Email, loginView.Password))
            {
                string adminPermission = "";
                if (glob_UserLevel.Equals("1"))
                {
                    adminPermission = "True";
                }

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, glob_UserName),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("User_ID", loginView.Email),
                    new Claim("Department", "HR"),
                    new Claim("Admin", adminPermission),
                    new Claim("Manager", "True")
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                if (returnUrl != null)
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return Redirect("Home/Welcome");
                }
            }
            return Redirect("/");
        }

        public bool UserValidate(string _emai, string _passwd)
        {
            try
            {
                APICommunicator api = new APICommunicator(Configuration);

                string strResultTest = api.GetStoredProDataWithParaHeader("Usr_Login", "User_id", _emai);
                var elevadoresModels = JsonConvert.DeserializeObject<List<UserViewModel>>(strResultTest);
                userdetails = elevadoresModels;
                if (userdetails[0].USR_ID != "")
                {
                    if (_passwd == PWD_EN_DE.DecryptString(userdetails[0].USR_PASSWORD.ToString()))
                    {
                        glob_UserLevel = userdetails[0].USR_LEVEL.ToString();
                        glob_UserName = userdetails[0].USR_NAMEFULL.ToString();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
    }
}
