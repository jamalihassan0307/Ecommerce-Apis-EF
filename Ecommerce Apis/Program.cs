using System.Text;
using Ecommerce_Apis.Data;
using Ecommerce_Apis.Utills;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ecommerce_Apis.UserModule.Repositories.InterFace;
using Ecommerce_Apis.UserModule.Repositories;
using Ecommerce_Apis.CartModule.Repositories.InterFace;
using Ecommerce_Apis.CartModule.Repositories;
using Ecommerce_Apis.OrderModule.Repositories.InterFace;
using Ecommerce_Apis.OrderModule.Repositories;
using Ecommerce_Apis.ProductModule.Repositories.InterFace;
using Ecommerce_Apis.ProductModule.Repositories;
using Ecommerce_Apis.CouponModule.Repositories.InterFace;
using Ecommerce_Apis.CouponModule.Repositories;
using Ecommerce_Apis.BannerModule.Repositories;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddControllers().AddNewtonsoftJson(option =>
    option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// JWT Configuration
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

// Register Services
builder.Services.AddScoped<ITokenHelper, TokenHelper>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register Repositories
// User Module
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Cart Module
builder.Services.AddScoped<ICartRepositories, CartRepositories>();
builder.Services.AddScoped<IWishListRepositories, WishListRepositories>();

// Order Module
builder.Services.AddScoped<IOrderRepositories, OrderRepositories>();
builder.Services.AddScoped<IAddressRepositories, AddressRepositories>();
builder.Services.AddScoped<IDeliveryBoyRepositories, DeliveryBoyRepositories>();

// Product Module
builder.Services.AddScoped<IProductRepository, ProductRepositories>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// Coupon Module
builder.Services.AddScoped<ICouponRepositories, CouponRepositories>();

// Banner Module
builder.Services.AddScoped<IBannerRepository, BannerRepository>();

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
