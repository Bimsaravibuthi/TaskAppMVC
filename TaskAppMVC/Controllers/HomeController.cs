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
using Microsoft.AspNetCore.Http;
using System.IO;

namespace TaskAppMVC.Controllers
{
    public class HomeController : Controller
    {
        List<MaxUserIdModel> maxUserId = new List<MaxUserIdModel>();
        List<MaxTaskIdModel> maxId = new List<MaxTaskIdModel>();
        List<AllUserIdNameModel> allUserIdNames = new List<AllUserIdNameModel>();
        List<CreatedUserModel> createdUser = new List<CreatedUserModel>();
        List<TaskModel> tasks = new List<TaskModel>();
        List<GetFileModel> files = new List<GetFileModel>();

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

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IEnumerable<MaxTaskIdModel> MaxTaskIdModels { get; set; }

        [Authorize(Policy = "ManagersOnly")]
        public IActionResult SaveTask()
        {
            ViewData["MaxTaskId"] = GetMaxTaskId();
            ViewBag.UserIdList = GetAllUserId();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveTask(SaveTaskModel saveTask, IFormFile file)
        {
            byte[] supFile = FileConvert(file);
            string strSupFile = Convert.ToBase64String(supFile);
            string maxTaskId = GetMaxTaskId();
            try
            {
                APICommunicator api = new APICommunicator(Configuration);

                string strResultTest = api.PostStoredProDataWithPara("Save_Task",
                    "Task_id|Company_id|Start_date|End_date|Description|Support_file|Priority|Create_user",
                    maxTaskId+ "|"+saveTask.TSK_COMID+"|"+saveTask.TSK_STDATE+"|"+saveTask.TSK_ENDATE+
                    "|"+saveTask.TSK_DESC+"|"+strSupFile+"|"+saveTask.TSK_PRIORITY+"|"+saveTask.TSK_CREATEUSER);
                
                var elevadoresModels = JsonConvert.DeserializeObject(strResultTest);
                if (elevadoresModels.ToString().Equals("1"))
                {
                    return Redirect("/Home/SaveTask");
                }
                else
                {
                    return Redirect("/Home/AccessDenied");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }         
        }
  
        private byte[] FileConvert(IFormFile file)
        {
            byte[] convertedFile = null;

            if (file != null)
            {
                {
                    var target = new MemoryStream();
                    file.CopyTo(target);
                    convertedFile = target.ToArray();
                }
            }
            return convertedFile;
        }

        public IActionResult CreatedUser()
        {
            GetCreatedUser();
            return View(createdUser);
        }

        [Authorize(Policy = "AdminOnly")]
        public IActionResult CreateUser()
        {           
            return View();
        }

        [Authorize(Policy = "LoggedUsersOnly")]
        public IActionResult TaskDashboard()
        {
            //GetAllTasks();
            return View(/*tasks*/);
        }

        public JsonResult getTask()
        {
            GetAllTasks();

            //List<TaskModel> tasks2 = new List<TaskModel>()
            //{
            //    new TaskModel{TSK_ID="TSK1",TSK_COMID="KFL",TSK_DESC="Nothing"},
            //    new TaskModel{TSK_ID="TSK2",TSK_COMID="MHL",TSK_DESC="There is something"}
            //};
            
            var data = JsonConvert.SerializeObject(tasks);
            return Json(data);
        }

        public bool claimTask(string tskId)
        {
            string asds = User.FindFirst("User_ID").Value.ToString();

            if (ClaimNewTask(tskId, asds))
            {
                return true;
            }
            return false;
        }

        public bool submitTask(string tskId)
        {
            string asds = User.FindFirst("User_ID").Value.ToString();

            if (SubmitNewTask(tskId))
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        public int Add(int number1, int number2)
        {
            return number1+number2;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserModel createUser)
        {
            createUser.USR_CREATEDBY = User.FindFirst("User_ID").Value;
            if (CreateUserPost(createUser))
            {
                return Redirect("/Home/CreatedUser");
            }

            return View("Home/Welcome");
        }

        //[HttpGet]
        //public FileResult DownLoadFile(string tskId)
        //{
        //    byte[] fileByte = Convert.FromBase64String(GetFileData(tskId));
        //    return File(fileByte, "application/pdf", tskId + " Documents.pdf");
        //}

        public JsonResult DownLoadFile(string tskId)
        {
            byte[] fileByte = Convert.FromBase64String(GetFileData(tskId));
            var data = JsonConvert.SerializeObject(fileByte);
            return Json(data);
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
                    "Usr_id|Usr_password|Usr_nic|Usr_namefull|Usr_createdate|Usr_level|Usr_createdby",
                    userId+"|"+pwd+"|"+cum.USR_NIC+"|"+cum.USR_NAMEFULL+"|"+cum.USR_CREATEDATE+"|"+cum.USR_LEVEL+"|"+cum.USR_CREATEDBY);
                var elevadoresModels = JsonConvert.DeserializeObject(strResultTest);

                if (elevadoresModels.ToString().Equals("1"))
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
                string strResultTest = api.GetStoredProDataWithParaHeader("Created_User",
                    "Usr_createdby", User.FindFirst("User_ID").Value);
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
        public List<TaskModel> GetAllTasks()
        {
            try
            {
                APICommunicator api = new APICommunicator(Configuration);

                string strResultTest = api.GetStoredProDataNoPara("All_Tasks");
                var elevadoresModels = JsonConvert.DeserializeObject<List<TaskModel>>(strResultTest);
                tasks = elevadoresModels;

                if (tasks.Count != 0)
                {
                    return tasks;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public bool ClaimNewTask(string tskId, string usrId)
        {
            try
            {
                APICommunicator api = new APICommunicator(Configuration);

                string strResultTest = api.PostStoredProDataWithPara("Claim_Task", "Tsk_id|Usr_id", tskId+"|"+usrId);
                var elevadoresModels = JsonConvert.DeserializeObject(strResultTest);

                if (elevadoresModels.ToString().Equals("1"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool SubmitNewTask(string tskId)
        {
            try
            {
                APICommunicator api = new APICommunicator(Configuration);

                string strResultTest = api.PostStoredProDataWithPara("Submit_Task", "Tsk_id", tskId);
                var elevadoresModels = JsonConvert.DeserializeObject(strResultTest);

                if (elevadoresModels.ToString().Equals("1"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetFileData(string tskId)
        {
            try
            {
                APICommunicator api = new APICommunicator(Configuration);

                string strResultTest = api.GetStoredProDataWithParaHeader("File_Download",
                    "tsk_id", tskId);
                files = JsonConvert.DeserializeObject<List<GetFileModel>>(strResultTest);

                if (files.Count != 0)
                {
                    return files[0].TSK_SUPFILE;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public async Task<FileResult> OnGetDownloadFileAsync(int? id)
        //{
        //    // obtain bytes of the file 
        //    // from database or by directly 
        //    // reading the files 
        //    //byte[] fileBytes = . . . ;

        //    return File(
        //        fileBytes,         /*byte []*/
        //        "application/pdf", /*mime type*/
        //        "fileName.pdf"     /*name of the file*/
        //    );

        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
