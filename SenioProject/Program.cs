using Microsoft.EntityFrameworkCore;
using SenioProject.Models;
using SenioProject.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
options.AddPolicy("AllowAll",
builder =>
{
    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
});
});

builder.Services.AddDbContext<SeniorProjectDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Ghassan")));

        builder.Services.AddScoped<StartInterviewRepository>();


// builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
//app.UseRouting();

app.UseAuthorization();

//app.MapStaticAssets();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}")
//     .WithStaticAssets();

app.MapControllers();
app.UseCors("AllowAll");

app.Run();
