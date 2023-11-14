using LoginWebExample.Middleware;
using LoginWebExample.ExampleModel;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Para SQL Server
builder.Services.AddDbContext<ExampleDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));

// Para MySQL
builder.Services.AddDbContext<ExampleDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("connectionString")!));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCustomAuth();
app.MapControllers();

app.Run();