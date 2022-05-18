using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppMVC.Models
{
    public class UserViewModel
    {
        public string USR_ID { get; set; }
        public string USR_PASSWORD { get; set; }
        public string USR_NIC { get; set; }
        public string USR_NAMEFULL { get; set; }
        public DateTime USR_CREATEDATE { get; set; }
        public string USR_LEVEL { get; set; }
    }
}
