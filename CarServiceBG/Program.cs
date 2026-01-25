using CarService.Business.Abstract;
using CarService.Business.Concrete;
using CarService.DataAccess;
using CarService.DataAccess.Abstract;
using CarService.DataAccess.Concrete;
using CarService.DataAccess.Concrete.EntityFramework;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using CarServiceBG.Hubs;
using CarServiceBG.Mappings;
using CarServiceBG.Services;
using CarServiceBG.Services.Abstract;
using CarServiceBG.Services.Concrete;
using CarServiceBG.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddScoped<ICarService, CarService.Business.Concrete.CarService>();
builder.Services.AddScoped<ICarRepository, EfCarRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatRepository, EfChatRepository>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, EfMessageRepository>();

builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPostRepository, EfPostRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, EfProductRepository>();

builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IShopRepository, EfShopRepository>();

builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<IWorkerRepository, EfWorkerRepository>();

builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<CarServiceBG.Services.Abstract.ICommentService, CarServiceBG.Services.Concrete.CommentService>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();

builder.Services.AddScoped<IAuctionService, AuctionService>();
builder.Services.AddHostedService<AuctionBackgroundService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Identity services
builder.Services.AddIdentity<User, IdentityRole<Guid>>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 1;

    opt.User.RequireUniqueEmail = true;

    //opt.SignIn.RequireConfirmedEmail = true;
    opt.Lockout.MaxFailedAccessAttempts = 3;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
}).AddEntityFrameworkStores<CarServiceDbContext>()
.AddRoleManager<RoleManager<IdentityRole<Guid>>>()
    .AddSignInManager<SignInManager<User>>()
    .AddDefaultTokenProviders();

builder.Services.Configure<CloudinarySettings>(options =>
{
    options.CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
    options.ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
    options.ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new
        Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier,

        };

        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // Yalnız SignalR yolu üçün
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                   path.StartsWithSegments("/chathub") || path.StartsWithSegments("/auctionHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    opt.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
    opt.AddPolicy("SellerPolicy", policy => policy.RequireRole("Seller"));
    opt.AddPolicy("WorkerPolicy", policy => policy.RequireRole("Worker"));
});

// CORS configuration to allow React app to communicate with API

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider.GetRequiredService<IAuctionService>();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error"); // Define an error handling endpoint
    app.UseHsts();
    app.UseHttpsRedirection();
}
// Add CORS middleware
app.UseCors("MyCorsPolicy");

// Add authentication and authorization middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapHub<AuctionHub>("/auctionHub");

app.Run();

