using LMIS.DataStore.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var provider = builder.Configuration["ServerSettings:ServerName"];
string postgreSQLConnection = builder.Configuration.GetConnectionString("PostgreSQLConnection");



builder.Services.AddDbContext<ApplicationDbContext>(
options => _ = provider switch
{
    "PostgreSQL" => options.UseNpgsql(postgreSQLConnection),
   
    //"SqlServer" => options.UseSqlServer(
    //    builder.Configuration.GetConnectionString("DefaultConnection")),

    _ => throw new Exception($"Unsupported provider: {provider}")
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
