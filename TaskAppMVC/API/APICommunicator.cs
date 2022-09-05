using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TaskAppMVC.DB_Connection;

namespace TaskAppMVC.API
{
    public class APICommunicator
    {
        //private readonly IConfiguration Configuration;
        private string Configuration;
        private string strTKN = "1";
        public APICommunicator()
        {
            Configuration = Database.APICommunicator();
        }

        public string GetStoredProDataWithParaHeader(string strProName, string strParaNames, string strParaValues)
        {
            string StringURL = String.Format(@Configuration + "getStoredProDataWithParaHeader?tkn=" + strTKN + "&strProcedure=" + strProName + "&strParaNames=" + strParaNames + "&strParaValues=" + strParaValues);
            
            WebRequest reqObjectGET = WebRequest.Create(StringURL);

            reqObjectGET.Method = "GET";
            HttpWebResponse resObjectGET = null;
            resObjectGET = (HttpWebResponse)reqObjectGET.GetResponse();

            string strResultTest = null;
            //stream kerala read karanawa
            using (Stream strm = resObjectGET.GetResponseStream())
            {
                StreamReader sr = new StreamReader(strm);
                strResultTest = sr.ReadToEnd();
                sr.Close();
            }

            return strResultTest;
        }
        public string GetStoredProDataNoPara(string strProName)
        {
            string StringURL = String.Format(@Configuration + "getStoredProDataNoPara?tkn=" + strTKN + "&strProcedure=" + strProName);

            WebRequest reqObjectGET = WebRequest.Create(StringURL);

            reqObjectGET.Method = "GET";
            HttpWebResponse resObjectGET = null;
            resObjectGET = (HttpWebResponse)reqObjectGET.GetResponse();

            string strResultTest = null;
            //stream kerala read keranawa
            using (Stream strm = resObjectGET.GetResponseStream())
            {
                StreamReader sr = new StreamReader(strm);
                strResultTest = sr.ReadToEnd();
                sr.Close();
            }

            return strResultTest;
        }
        public string PostStoredProDataWithPara(string strProName, string strParaNames, string strParaValues)
        {
            string strOut = "";

            try 
            {
                string StringURL = String.Format(@Configuration + "PostStoredProDataWithPara");
                WebRequest reqObjectPOST = WebRequest.Create(StringURL);
                reqObjectPOST.Method = "POST";
                reqObjectPOST.ContentType = "application/json";

                string bodyRequest = "{\"tkn\":\"" + strTKN + "\",\"strProcedure\":\"" + strProName + "\",\"strParaNames\":\"" + strParaNames + "\",\"strParaValues\":\"" + strParaValues + "\"}";

                using var streamWriter = new StreamWriter(reqObjectPOST.GetRequestStream());
                streamWriter.Write(bodyRequest);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)reqObjectPOST.GetResponse();
                using (var streamreader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    strOut = streamreader.ReadToEnd();
                }
            }
            catch(WebException e)
            {
                strOut = e.ToString();
            }
            return strOut;
        }
    }
}
