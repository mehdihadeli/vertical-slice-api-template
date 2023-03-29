using Catalogs.Shared;
using ECommerce.Services.Catalogs.Shared.Extensions.WebApplicationBuilderExtensions;
using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddCustomProblemDetails();

builder.Services.AddControllers();

builder.AddServices();

var app = builder.Build();
app.UseProblemDetails();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureServices();
app.MapEndpoints();

app.Run();
