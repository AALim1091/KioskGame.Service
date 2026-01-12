using System.Text.Json;

namespace KioskGame.Service;

public static class RequestResultExtension
{
    public static async Task Response<T>(this IRequestResult<T> result, HttpContext context)
    {
        context.Response.StatusCode = result.StatusCode ?? 500;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = result.Success,
            data = result.Data,
            error = result.Error,         
            statusCode = result.StatusCode,    
            
        };

        await context.Response.WriteAsJsonAsync(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }
}
