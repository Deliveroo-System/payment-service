using Microsoft.EntityFrameworkCore;
using Payment_Service.Models;
using Payment_Service.Service;
using Payment_Service.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

// Register DbContext with the appropriate scope (no need for AddScoped separately)
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register PayPalService for dependency injection
builder.Services.AddScoped<PayPalService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = string.Empty;  // Removes 'index.html' and makes Swagger UI accessible at the root URL
    });
}

builder.Logging.AddConsole();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// Configure routing and authorization
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Developer exception page for development environment
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();
