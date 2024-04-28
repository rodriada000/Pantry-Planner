using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PantryPlannerCore.Models;
using PantryPlanner.Services;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using PantryPlannerCore.Services;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddScoped<RecipeScrapeService>();
builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<RecipeStepService>();
builder.Services.AddScoped<RecipeIngredientService>();
builder.Services.AddScoped<RecipeService>();


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

    app.UseAuthorization();
    app.MapControllers();
}
else
{
    app.UseRouting();
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    var fileExtensionProvider = new FileExtensionContentTypeProvider();
    fileExtensionProvider.Mappings[".webmanifest"] = "application/manifest+json";
    app.UseStaticFiles(new StaticFileOptions()
    {
        ContentTypeProvider = fileExtensionProvider,
        RequestPath = "/client",
        FileProvider = new PhysicalFileProvider(
            Path.Combine(app.Environment.ContentRootPath, "client"))
    });

    app.UseSpaStaticFiles();

    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "/client";

        var spaStaticFileOptions = new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "client"))
        };

        spa.Options.DefaultPageStaticFileOptions = spaStaticFileOptions;

    });
}

app.UseHttpsRedirection();


app.Run();
