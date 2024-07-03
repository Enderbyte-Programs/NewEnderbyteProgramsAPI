using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MySqlConnector;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters;

namespace EnderbyteProgramsAPIService
{
    public static class Constants
    {
        public static string SQLConnectionString = "";
        public static Dictionary<Exception,int> ErrorMappings = new Dictionary<Exception, int>
        {
            //Fill it in later
        };
        public static void LoadConstants()
        {
            if (Directory.Exists("C:\\Users\\jorda"))
            {
                SQLConnectionString = "Server=raspberrypi;Database=epapi;Uid=api;";
            } else
            {
                SQLConnectionString = "Server=localhost;Database=epapi;Uid=api;";
            }

        }
        public static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
        };
    }

   public static class SQL
    {
        public static void RunSqlNonQuery(string query)
        {
            MySqlConnection sq = new MySqlConnection(Constants.SQLConnectionString);
            sq.Open();
            MySqlCommand sqc = new MySqlCommand(query, sq);
            sqc.ExecuteNonQuery();
            sq.Close();
        }
        public static List<Dictionary<string, object>> RunSqlQuery(string query)
        {
            List<Dictionary<string, object>> dx = new List<Dictionary<string, object>>();
            MySqlConnection sq = new MySqlConnection(Constants.SQLConnectionString);
            sq.Open();
            MySqlCommand sqc = new MySqlCommand(query, sq);
            var sqr = sqc.ExecuteReader();
            while (sqr.Read())
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                for (int lp = 0; lp < sqr.FieldCount; lp++)
                {
                    dict.Add(sqr.GetName(lp), sqr.GetValue(lp));
                }
                dx.Add(dict);
            }
            sq.Close();
            return dx;
        }
    }
    public static class Utilities
    {
        
        public static string SerializeJson(object data)
        {
            return JsonConvert.SerializeObject(data);
        }
        public static String GetIP(HttpContext hc)
        {
            return hc.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
    public class ApiResponse
    {
        public static int MapExceptionType(Exception ee)
        {
            return 100;
        }
        public static int EC2HTTP(int ec)
        {
            return 400;
        }
        public static Dictionary<string, object> ExceptionToJson(Exception ex)
        {
            string msg = ex.Message;
            string stack = ex.StackTrace;
            string type = ex.GetType().Name;
            Dictionary<string, object> final = new Dictionary<string, object>();
            final.Add("error", true);
            final.Add("errorMessage", msg);
            final.Add("message", msg);
            final.Add("errorStack", stack);
            final.Add("errorType", type);
            final.Add("errorCode", MapExceptionType(ex));
            //return SerializeJson(final);
            return final;
        }
        private Dictionary<string, object> master = new Dictionary<string, object>();
        private int ReturnCode = 200;
        public ApiResponse(string message)
        {
            master.Add("error", false);
            master.Add("message", message);
            master["data"] = new Dictionary<string, object>();

        }
        public ApiResponse(Exception ex)
        {
            LoadException(ex);
        }
        public Dictionary<string, object> ToDictionary()
        {
            return master;
        }
        //Everything below here is just exposing dictionary methods
        public void Add(string key, object value)
        {
            ((Dictionary<string, object>)master["data"])[key] = value;
        }
        public bool KeyExists(string key)
        {
            return ((Dictionary<string, object>)master["data"]).ContainsKey(key);
        }
        public bool ValueExists(string value)
        {
            return ((Dictionary<string, object>)master["data"]).ContainsValue(value);
        }
        public void DeleteKey(string key)
        {
            ((Dictionary<string, object>)master["data"]).Remove(key);
        }
        public void SetMessage(string m)
        {
            this.master["message"] = m;
        }
        public ApiResponse()
        {
            //Blank
            master.Add("error", false);
            master.Add("message","");
            master["data"] = new Dictionary<string, object>();
        }
        public void LoadException(Exception ex)
        {
            master = ExceptionToJson(ex);
            master["data"] = new Dictionary<string, object>();
            ReturnCode = EC2HTTP(MapExceptionType(ex));
        }
        //TODO return statuscode
        public JsonResult Finalize()
        {
            var a = new JsonResult(this.master);
            
            a.StatusCode = this.ReturnCode;
            return a;
        }
    }

    namespace Exceptions
    {

    }

    public class EPController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string ip = Utilities.GetIP(context.HttpContext);
            SQL.RunSqlNonQuery($"insert ignore into RequestStats values ('{ip}',0);" +
                $"update RequestStats set Requests = Requests + 1 where IPAddress like '{ip}';" +
                $"update GlobalStats set ValueData = ValueData + 1 WHERE KeyName = 'TotalRequests';");
        }
    }
}
