/*using FPTTelecomBE.Data;
using FPTTelecomBE.Hubs;
using FPTTelecomBE.Mappings;
using FPTTelecomBE.Middleware;
using FPTTelecomBE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// DEBUG: Log environment info
Console.WriteLine($"🔍 Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"🔍 PORT: {Environment.GetEnvironmentVariable("PORT")}");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

// Logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
// Thêm dòng này vào phần services
builder.Services.AddScoped<IJobPostingService, JobPostingService>();
// service cho JobApplication
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();

// SignalR
builder.Services.AddSignalR();

//Response Comression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "text/json", "text/html", "text/plan" });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});
// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Remove default 5 minute tolerance
    };

    // SignalR Authentication
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };

});

builder.Services.AddAuthorization();

// CORS
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowFrontend", policy =>
//     {
//         policy.WithOrigins(
//             "http://localhost:3000",
//             "http://localhost:5173",
//             "http://localhost:5174")
//               .AllowAnyHeader()
//               .AllowAnyMethod()
//               .AllowCredentials();
//     });
// });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173",
            "https://fpt-telecom-fe.vercel.app",
            "https://your-app-name.onrender.com",
            "https://*.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FPT Telecom Bình Định API",
        Version = "v1",
        Description = "API for FPT Telecom Bình Định website - Tư vấn & Lắp đặt WiFi",
        Contact = new OpenApiContact
        {
            Name = "FPT Bình Định Support",
            Email = "support@fptbinhdinh.com"
        }
    });

    // JWT Bearer configuration in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("🔄 Starting database migration...");
            
            // Get DbContext instance
            var context = services.GetRequiredService<AppDbContext>();
            
            // Apply all pending migrations
            // Tương đương với: dotnet ef database update
            context.Database.Migrate();
            
            logger.LogInformation("✅ Database migration completed successfully.");
            
            // Optional: Seed initial data
            // await SeedData.Initialize(services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ An error occurred while migrating the database.");
            
            // IMPORTANT: Throw exception để fail deployment
            // Nếu migration lỗi mà app vẫn start → data inconsistency
            throw;
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FPT Telecom Bình Định API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}
else
{
    // Global exception handler for production
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    app.ConfigureExceptionHandler(logger);
}

//app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();

//app.MapHub<ChatHub>("/hubs/chat");

// HealthIsDevelopment check endpoint
//app.MapGet("/health", () => Results.Ok(new
//{
//    status = "healthy",
//    timestamp = DateTime.UtcNow,
//    environment = app.Environment.EnvironmentName
//}));
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

// Health check endpoint (GỘP 2 CÁI LẠI)
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
}));

app.Run();

app.Run();*/
using FPTTelecomBE.Data;
using FPTTelecomBE.Hubs;
using FPTTelecomBE.Mappings;
using FPTTelecomBE.Middleware;
using FPTTelecomBE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DEBUG: Log environment info
Console.WriteLine($"🔍 Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"🔍 PORT: {Environment.GetEnvironmentVariable("PORT")}");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

// Logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"🔍 Connection string length: {connString?.Length ?? 0}");

    options.UseNpgsql(connString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(180);
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null
        );
    });
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IJobPostingService, JobPostingService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();

// SignalR
builder.Services.AddSignalR();

// Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "text/json", "text/html", "text/plan" });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173",
            "https://fpt-telecom-fe.vercel.app",
            "https://fpttelecombinhdinhbe.onrender.com",
            "https://*.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FPT Telecom Bình Định API",
        Version = "v1",
        Description = "API for FPT Telecom Bình Định website - Tư vấn & Lắp đặt WiFi",
        Contact = new OpenApiContact
        {
            Name = "FPT Bình Định Support",
            Email = "support@fptbinhdinh.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// DATABASE SETUP (PRODUCTION)
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("🔄 Starting database setup...");

            var context = services.GetRequiredService<AppDbContext>();

            // Test connection
            logger.LogInformation("🔄 Testing database connection...");
            var canConnect = await context.Database.CanConnectAsync();
            logger.LogInformation($"🔍 Connection test: {(canConnect ? "✅ SUCCESS" : "❌ FAILED")}");

            if (!canConnect)
            {
                logger.LogWarning("⚠️ Cannot connect to database. Attempting to create...");
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("✅ Database created successfully.");
            }

            // Apply migrations
            logger.LogInformation("🔄 Applying pending migrations...");
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            logger.LogInformation($"🔍 Found {pendingMigrations.Count()} pending migrations");

            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();
                logger.LogInformation("✅ Database migration completed successfully.");
            }
            else
            {
                logger.LogInformation("✅ Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Database setup failed");
            logger.LogError($"❌ Error type: {ex.GetType().Name}");
            logger.LogError($"❌ Error message: {ex.Message}");

            if (ex.InnerException != null)
            {
                logger.LogError($"❌ Inner exception: {ex.InnerException.Message}");
            }

            // DON'T throw - let app start anyway
            logger.LogWarning("⚠️ App will start without database. Some features may not work.");
        }
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FPT Telecom Bình Định API v1");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    app.ConfigureExceptionHandler(logger);
}

app.UseResponseCompression();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
}));

// CHỈ CÓ MỘT app.Run() - XÓA CÁI DUPLICATE!
app.Run();