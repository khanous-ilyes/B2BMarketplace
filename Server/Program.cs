using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Services.Contracts;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Config DB Context avec SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));
});

// Config JWT Section
builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));
var jwtSection = builder.Configuration.GetSection(nameof(JwtSection)).Get<JwtSection>();

// Dependency Injection
// Implementations have been removed for public GitHub showcase.
// builder.Services.AddScoped<IUserAccount, UserAccountRepository>();
// builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
// builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
// builder.Services.AddScoped<IContactRepository, ContactRepository>();
// builder.Services.AddScoped<IEmailService, EmailService>();

// Firebase & Storage
var firebaseCredPath = builder.Configuration["Firebase:CredentialsPath"];
if (!string.IsNullOrEmpty(firebaseCredPath) && File.Exists(firebaseCredPath))
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(firebaseCredPath)
    });
}
else 
{
    try { FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.GetApplicationDefault() }); } catch { }
}
// builder.Services.AddScoped<IStorageService, SupabaseStorageService>();

// Authentication Config
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection!.Issuer,
        ValidAudience = jwtSection.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.Key))
    };
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS : Autoriser le client React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebClient",
        policy => policy
            .WithOrigins("http://localhost:5173", "https://localhost:5173", "https://red-grass-00db05610.2.azurestaticapps.net") // Port par défaut de Vite
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


// CORS doit être AVANT HttpsRedirection
app.UseCors("AllowWebClient");

// Serve static files (uploaded images)
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    try 
    {
        await context.Database.MigrateAsync();
        await DbSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

app.Run();
