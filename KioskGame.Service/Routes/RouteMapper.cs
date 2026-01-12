using KioskGame.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace KioskGame.Service;

internal static class RouteMapper
{
    public static WebApplication MapRoutes(this WebApplication app)
    {
        var configuration = app.Services.GetRequiredService<IEndpointConfiguration>();

        //POST /api/player/login
        var endpoint = configuration.PlayerLogin();
        app.MapPost(endpoint.Uri!, async ([FromBody] PlayerIdRequest request, [FromServices] IGameService gameService, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await gameService.LoginAsync(request.PlayerId, cancellationToken).ConfigureAwait(false);
            await result.Response(context);
        });

        //GET /api/player/{id}/status
        endpoint = configuration.PlayerStatus();
        app.MapGet(endpoint.Uri!, async (string id, [FromServices] IGameService gameService, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await gameService.GetStatusAsync(id, cancellationToken).ConfigureAwait(false);
            await result.Response(context);
        });

        //POST /api/game/play
        endpoint = configuration.GamePlay();
        app.MapPost(endpoint.Uri!, async ([FromBody] PlayerIdRequest request, [FromServices] IGameService gameService, HttpContext context, CancellationToken cancellationToken) =>
        {
            var result = await gameService.PlayAsync(request.PlayerId, cancellationToken).ConfigureAwait(false);
            await result.Response(context);
        });

        return app;
    }
}
