using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace FPTTelecomBE.Middleware;

public static class GlobalExceptionHandler
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var exception = contextFeature.Error;

                    logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

                    var response = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = "Internal Server Error",
                        details = app.ApplicationServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                            ? exception.Message
                            : "An error occurred processing your request"
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            });
        });
    }
}