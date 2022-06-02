using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TaskAppMVC.Security;
using TaskAppMVC.API;
using TaskAppMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace TaskAppMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        List<MaxUserIdModel> maxUserId = new List<MaxUserIdModel>();
        List<MaxTaskIdModel> maxId = new List<MaxTaskIdModel>();
        List<AllUserIdNameModel> allUserIdNames = new List<AllUserIdNameModel>();
        List<CreatedUserModel> createdUser = new List<CreatedUserModel>();

        public readonly IConfiguration Configuration;

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public IEnumerable<MaxTaskIdModel> MaxTaskIdModels { get; set; }

        //[HttpGet("SaveTask")]
        public IActionResult SaveTask()
        {
            ViewData["MaxTaskId"] = GetMaxTaskId();
            ViewBag.UserIdList = GetAllUserId();
            return View();
        }
        
        public IActionResult CreatedUser()
        {
            GetCreatedUser();
            return View(createdUser);
        }
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUserAsync(CreateUserModel createUser)
        {
            if (CreateUserPost(createUser))
            {
                return CreatedUser();
            }

            return View("Home/Welcome");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string GetMaxTaskId()
        {
            try
            {
                APICommunicator api = new APICommunicator(Configuration);

                string strResultTest = api.GetStoredProDataNoPara("Select_Max_Tsk_Id");
                var elevadoresModels = JsonConvert.DeserializeObject<List<MaxTaskIdModel>>(strResultTest);
                maxId = elevadoresModels;

                if (maxId[0].TASK_ID != "")
                {
                    return maxId[0].TASK_ID.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AllUserIdNameModel> GetAllUserId()
        {
            try
            {
                APICommunicator api = new APICommunicator(Configuration);
                
                string strResultTest = api.GetStoredProDataNoPara("All_Users_Id");
                var elevadoresModels = JsonConvert.DeserializeObject<List<AllUserIdNameModel>>(strResultTest);
                allUserIdNames = elevadoresModels;
                
                if (allUserIdNames.Count != 0)
                {
                    return allUserIdNames;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetMaxUserId()
        {
            APICommunicator api = new APICommunicator(Configuration);

            string strResultTest = api.GetStoredProDataNoPara("Select_Max_Usr_Id");
            var elevadoresModels = JsonConvert.DeserializeObject<List<MaxUserIdModel>>(strResultTest);
            maxUserId = elevadoresModels;

            int userId = 0;
            foreach(var item in maxUserId)
            {
                userId = Int32.Parse(item.USER_ID[3..]);
            }
            userId++;
            return "KFL" + userId.ToString();
        }
        public bool CreateUserPost(CreateUserModel cum)
        {
            try
            {
                string userId = GetMaxUserId();
                string pwd = PWD_EN_DE.EncryptString(cum.USR_PASSWORD.ToString());

                APICommunicator api = new APICommunicator(Configuration);            

                string strResultTest = api.PostStoredProDataWithPara("Create_User",
                    "Usr_id|Usr_password|Usr_nic|Usr_namefull|Usr_createdate|Usr_level",
                    userId+"|"+pwd+"|"+cum.USR_NIC+"|"+cum.USR_NAMEFULL+"|"+cum.USR_CREATEDATE+"|"+cum.USR_LEVEL);
                var elevadoresModels = JsonConvert.DeserializeObject<ReturnStatusModel>(strResultTest);

                if(elevadoresModels.RETURN_STATUS == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public List<CreatedUserModel> GetCreatedUser()
        {
            try
            {
                APICommunicator api = new APICommunicator(Configuration);

                string strResultTest = api.GetStoredProDataNoPara("Select_Max_User");
                createdUser = JsonConvert.DeserializeObject<List<CreatedUserModel>>(strResultTest);

                if (createdUser.Count != 0)
                {
                    return createdUser;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
