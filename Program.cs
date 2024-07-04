using EnderbyteProgramsAPIService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Constants.LoadConstants();
builder.WebHost.UseUrls(new string[]
{
    "http://*.11111",
    "http://192.168.1.169:11111",
    "http://enderbyteprograms.ddnsfree.com:11111",
    "http://0.0.0.0:11111"
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Urls.Add("http://*.11111");
app.MapControllers();
app.Run();
