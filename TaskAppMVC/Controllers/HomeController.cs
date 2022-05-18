using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TaskAppMVC.API;
using TaskAppMVC.Models;

namespace TaskAppMVC.Controllers
{
    public class HomeController : Controller
    {
        List<MaxTaskIdModel> maxId = new List<MaxTaskIdModel>();
        List<AllUserIdNameModel> allUserIdNames = new List<AllUserIdNameModel>();

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
    }
}
