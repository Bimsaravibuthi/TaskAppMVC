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

        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginView, string returnUrl)
        {
            if (UserValidate(loginView.User_id, loginView.Password))
            {
                string adminPermission = "False";
                string normalUserPermission = "False";

                if(userdetails[0].USR_LEVEL.ToString().Equals("2"))
                {
                    normalUserPermission = "True";
                }
                else if(userdetails[0].USR_LEVEL.ToString().Equals("1"))
                {
                    adminPermission = "True";
                    normalUserPermission = "True";
                }

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, userdetails[0].USR_NAMEFULL.ToString()),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("User_ID", loginView.User_id),
                    new Claim("Department", "HR"),
                    new Claim("Admin", adminPermission),
                    new Claim("NormalUser", normalUserPermission)
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
                    return Redirect("/Home/Welcome");
                }
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
                APICommunicator api = new APICommunicator();

                string strResultTest = api.GetStoredProDataWithParaHeader("Usr_Login", "User_id", _emai);
                var elevadoresModels = JsonConvert.DeserializeObject<List<UserViewModel>>(strResultTest);
                userdetails = elevadoresModels;
                if (userdetails[0].USR_ID != "")
                {
                    if (_passwd == PWD_EN_DE.DecryptString(userdetails[0].USR_PASSWORD.ToString()))
                    {
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
