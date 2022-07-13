using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PantryPlannerCore.Models;
using PantryPlanner.Services;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<PantryPlannerContext>(options => options.UseSqlServer(configuration["ConnectionStrings:PantryPlannerDB"]));

// For Identity
builder.Services.AddIdentity<PantryPlannerUser, IdentityRole>()
    .AddEntityFrameworkStores<PantryPlannerContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddGoogle(opts =>
{
    opts.ClientId = configuration["Authentication:Google:ClientId"];
    opts.ClientSecret = configuration["Authentication:Google:ClientSecret"];
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});
//.AddOpenIdConnect("Google", "Google", options =>
//{
//    IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");

//    options.Authority = "https://accounts.google.com/";
//    options.ClientId = googleAuthNSection["ClientId"];
//    options.CallbackPath = "/signin-google";
//    options.SignedOutCallbackPath = "/signout-callback-google";
//    options.RemoteSignOutPath = "/signout-google";
//    options.SaveTokens = true;
//    options.Scope.Add("email");
//});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddSpaStaticFiles(c =>
    {
        c.RootPath = "/client";
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseSpaStaticFiles(new StaticFileOptions()
    {
        RequestPath = "/client"
    });

    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "/client";
        spa.Options.DefaultPage = "/client/index.html";
        spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions()
        {
            RequestPath = "/client"
        };
    });
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
