using MMDB.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Sriracha.Deploy.Offline
{
    public static class WinFormsHelper
    {
        [Obsolete("Move to MMDB.Shared.DateTimeHelper")]
        public static string LocalDateText(DateTime dateTime)
        {
            return DateTimeHelper.FromUtcToTimeZone(dateTime, TimeZone.CurrentTimeZone.StandardName).ToString();
        }

        public static void DisplayError(string message, Exception err)
        {
            using (var dlg = new ErrorForm(message, err))
            {
                dlg.ShowDialog();
            }
        }

        public static T GetJsonUrl<T>(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            using(var response = request.GetResponse())
            using(var responseStream = response.GetResponseStream())
            using(var responseReader = new StreamReader(responseStream))
            {
                string json = responseReader.ReadToEnd();
                var returnValue = JsonConvert.DeserializeObject<T>(json);
                return returnValue;
            }
        }
    }
}
