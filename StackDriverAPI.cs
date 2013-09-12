using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace StackDriver
{
    public class StackDriverAPI
    {
        public static void SubmitMetric(string metricName, object metricValue, DateTime collectedAt)
        {
            using (WebClient wc = new WebClient())
            {

                var URI = new Uri("https://custom-gateway.stackdriver.com/v1/custom");

                wc.Headers["Content-Type"] = "application/json";

                wc.Headers["x-stackdriver-apikey"] = "Your_Key_Here";

                GatewayMessage gm = new GatewayMessage();
                gm.timestamp = DateTime.Now.ToUnixTime();
                gm.data = new DataPoint()
                {
                    collected_at = collectedAt.ToUnixTime(),
                    name = metricName,
                    value = metricValue,
                };

                try
                {
                    wc.UploadString(URI, "POST", Serializer.Serialize(gm));
                }
                catch (Exception ex)
                {
                    // Do something with the exception
                }
            }
        }
    }

    public class GatewayMessage
    {
        public long timestamp { get; set; }
        public int proto_version = 1;
        public DataPoint data { get; set; }
    }

    public class DataPoint
    {
        public string name { get; set; }
        public object value { get; set; }
        public long collected_at { get; set; }
    }


    public static class Serializer
    {
        public static string Serialize<T>(this T e) where T : class, new()
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = int.MaxValue;
            return ser.Serialize(e);
        }
    }

    public static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }
    }
}