using EnderbyteProgramsAPIService.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnderbyteProgramsAPIService.Controllers
{
    public class CraftServerSetup : EPController
    {
        [Route("/craftserversetup/call")]
        [HttpPost]
        public IActionResult Post([FromBody] CRSSIngress ingress)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (ingress is null)
                {
                    throw new ArgumentNullException(nameof(ingress));
                }
                SQL.RunSqlNonQuery($"insert into CraftServerSetup values ('{Utilities.GetIP(HttpContext)}','{ingress.OperatingSystem}','{ingress.ApplicationVersion}',{ingress.IsActivated.ToString()},NOW(),{ingress.ServerCount})");
                response.SetMessage("Successfully registered telemetry call");
            } catch (Exception ex)
            {
                response.LoadException(ex);
            }
            return response.Finalize();
        }
    }
}
