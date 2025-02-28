// using Microsoft.EntityFrameworkCore;
// using RestaurantAdmin.Data;

// //adding for auth

// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// //end

// var builder = WebApplication.CreateBuilder(args);

// // Add Database Context
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowReactApp",
//         policy => policy.WithOrigins("http://localhost:5173") // Adjust if needed
//                         .AllowAnyMethod()
//                         .AllowAnyHeader());
// });

// //adding for auth
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             ValidAudience = builder.Configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//         };
//     });

// builder.Services.AddAuthorization();    


// var app = builder.Build();
// app.UseCors("AllowReactApp");  // ✅ Apply CORS before authorization

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();
// app.UseAuthorization();
// app.MapControllers();

// app.Run();

using Microsoft.EntityFrameworkCore;
using RestaurantAdmin.Data;

// Adding for authentication
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Ensure configuration is properly loaded
var configuration = builder.Configuration;

// ✅ Retrieve JWT settings safely
string jwtKey = configuration.GetValue<string>("Jwt:Key") 
                ?? throw new ArgumentNullException("Jwt:Key is missing in appsettings.json.");

string jwtIssuer = configuration.GetValue<string>("Jwt:Issuer") 
                   ?? throw new ArgumentNullException("Jwt:Issuer is missing in appsettings.json.");

string jwtAudience = configuration.GetValue<string>("Jwt:Audience") 
                     ?? throw new ArgumentNullException("Jwt:Audience is missing in appsettings.json.");

// ✅ Add Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// ✅ Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:5173") // Adjust if needed
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ✅ Add Authentication and Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ✅ Add API Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Apply CORS before Authentication/Authorization
app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Middleware Order is Important
app.UseHttpsRedirection();
app.UseAuthentication();  // ✅ Added missing Authentication middleware
app.UseAuthorization();
app.MapControllers();

app.Run();
