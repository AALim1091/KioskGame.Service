namespace KioskGame.Service;

public class EndpointConfiguration : IEndpointConfiguration
{
    public DataEndpoint PlayerLogin() => new() { Uri = "/api/player/login", Operation = EndpointOperation.Execute };

    public DataEndpoint PlayerStatus() => new() { Uri = "/api/player/{id}/status", Operation = EndpointOperation.Execute };

    public DataEndpoint GamePlay() => new() { Uri = "/api/game/play", Operation = EndpointOperation.Execute };
}
