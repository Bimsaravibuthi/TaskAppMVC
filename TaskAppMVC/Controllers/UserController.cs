using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskAppMVC.Security;
using TaskAppMVC.API;
using TaskAppMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (UserValidate(email, password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Login");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return Redirect("/Home/Welcome");
                //if (returnUrl != null)
                //{
                //    return Redirect(returnUrl);
                //}
                //else
                //{
                //    return Redirect("Home/Welcome");
                //}
            }
            else
            {
                return Redirect("/Home/AccessDenied");
            }
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
