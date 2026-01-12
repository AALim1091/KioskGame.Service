namespace KioskGame.Service;

public interface IEndpointConfiguration
{
    DataEndpoint PlayerLogin();
    DataEndpoint PlayerStatus();
    DataEndpoint GamePlay();
}
