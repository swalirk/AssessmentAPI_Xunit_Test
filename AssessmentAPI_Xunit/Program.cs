using AssessmentAPI_Xunit.model;
using AssessmentAPI_Xunit.Service.Interface;
using AssessmentAPI_Xunit.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<VehicleBrandContext>(options =>
options.UseMySQL(builder.Configuration.GetConnectionString("VehicleCS")));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddScoped<IVehicleInterface, VehicleRepository>();
builder.Services.AddScoped<IBrandInteface,BrandRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
