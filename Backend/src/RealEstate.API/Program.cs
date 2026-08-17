using Microsoft.OpenApi;
using Microsoft.AspNetCore.RateLimiting;
using RealEstate.API.Extensions;
using RealEstate.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var jwtSecurityScheme = new OpenApiSecurityScheme
{
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Enter your JWT token below."
};

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "RealEstate.API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDatabaseService(builder.Configuration);
builder.Services.AddIdentityService();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("otp-policy", opt =>
    {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();

