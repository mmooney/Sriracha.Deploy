using MMDB.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
