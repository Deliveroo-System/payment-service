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

builder.Services.AddSingleton<PayPalClient>();
builder.Services.AddScoped<PayPalService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

builder.Logging.AddConsole();



app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

//app.UseHttpsRedirection();



app.UseRouting();
app.UseAuthorization();
app.MapControllers();

//app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}



app.Run();
