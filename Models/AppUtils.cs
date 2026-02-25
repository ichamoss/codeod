using Framework;
using FrameworkUI;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dr7Hmi
{
    public static class AppUtils
    {
        public const string Images = @"/Dr7Hmi;component/Images/";

        public const int SeverityMessage = 0;
        public const int SeverityWarning = 100;
        public const int SeverityAlarm = 200;
    }

    public static class AppApi
    {
        public static bool IsInSimulation
        {
            get
            {
                return ServiceLocator.GetService<IIOSystem>() is IOSystemSimulation;
            }
        }
    }

    public static class DateTimeExtensions
    {
        public static string ToDateHourMinutes(this DateTime dateTime) => dateTime.ToString("dd.MM.yy HH:mm");
    }
}
