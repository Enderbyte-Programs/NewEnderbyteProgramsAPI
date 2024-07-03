using EnderbyteProgramsAPIService.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnderbyteProgramsAPIService.Controllers
{
    public class CRSSClassic : EPController
    {
        [Route("/api/amcs/")]
        [HttpGet]
        public IActionResult Get([FromQuery] CRSSClassicIngress ingress)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                
                SQL.RunSqlNonQuery($"insert into CraftServerSetup (IPAddress,OperatingSystem,AppVersion,IsActivated) values ('{Utilities.GetIP(HttpContext)}','{ingress.os}','{ingress.appversion}',{ingress.activated.ToString()})");
                response.SetMessage("Successfully registered telemetry call");
            } catch (Exception ex)
            {
                response.LoadException(ex);
            }
            return response.Finalize();
        }
    }
}
