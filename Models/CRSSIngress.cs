namespace EnderbyteProgramsAPIService.Models
{
    public class CRSSIngress
    {
        public string OperatingSystem { get; set; }
        public string ApplicationVersion { get; set; }
        public bool IsActivated { get; set; }
        public int ServerCount {get; set;}
    }
}
