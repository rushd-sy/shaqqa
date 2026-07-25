using RealEstate.API.Extensions;
using RealEstate.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDatabaseService(builder.Configuration);
builder.Services.AddIdentityService();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
<<<<<<< HEAD
builder.Services.AddInfrastructureServices(builder.Configuration);
=======
builder.Services.AddInfrastructure(builder.Configuration);

>>>>>>> origin/main
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

