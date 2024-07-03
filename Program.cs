using EnderbyteProgramsAPIService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Constants.LoadConstants();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
