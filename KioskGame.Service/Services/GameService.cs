using Microsoft.EntityFrameworkCore;

namespace KioskGame.Service;

public interface IGameService
{
    Task<RequestResult<PlayerStatusDto>> LoginAsync(string playerId, CancellationToken cancellationToken = default);
    Task<RequestResult<PlayerStatusDto>> GetStatusAsync(string playerId, CancellationToken cancellationToken = default);
    Task<RequestResult<PlayResponseDto>> PlayAsync(string playerId, CancellationToken cancellationToken = default);
}

public class GameService : IGameService
{
    private readonly AppDbContext _context;
    private readonly SessionService _session;
    private readonly PrizeService _prize;
    private readonly IClock _clock;
    private readonly ILogger<GameService> _logger;

    public GameService(AppDbContext context, SessionService session, PrizeService prize, IClock clock, ILogger<GameService> logger)
    {
        _context = context;
        _session = session;
        _prize = prize;
        _clock = clock;
        _logger = logger;
    }

    public async Task<RequestResult<PlayerStatusDto>> LoginAsync(string playerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId, cancellationToken).ConfigureAwait(false);
            if (player is null)
            {
                player = new Player { Id = playerId };
                _context.Players.Add(player);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Created new playerId: {PlayerId}", playerId);
            }

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var result = await GetStatusAsync(playerId, cancellationToken).ConfigureAwait(false);

            if (result.Success)
            {
                _logger.LogInformation("Successfully Logged In playerId: {PlayerId}", playerId);
            }

            return result;
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Login failed for PlayerId: {PlayerId}", playerId);
            return RequestResult<PlayerStatusDto>.Failure("Login failed");
        }
       
    }

    public async Task<RequestResult<PlayerStatusDto>> GetStatusAsync(string playerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId, cancellationToken).ConfigureAwait(false);
            if (player is null)
            {
                return RequestResult<PlayerStatusDto>.NotFound("PlayerId not found. Try Logging In First");
            }

            var session = await GetOrCreateTodaySessionAsync(playerId, cancellationToken).ConfigureAwait(false);

            if (session.SessionStartTime is null)
            {
                var dto = new PlayerStatusDto
                {
                    PlayerId = playerId,
                    PlaysRemaining = SessionService.MaxPlaysPerDay,
                    SessionExpires = null,
                    IsSessionExpired = false,
                };

                return RequestResult<PlayerStatusDto>.Ok(dto);
            }

            // session started
            var startTime = session.SessionStartTime.Value;

            if (_session.IsExpired(startTime))
            {
                var dto = new PlayerStatusDto
                {
                    PlayerId = playerId,
                    PlaysRemaining = 0,  
                    SessionExpires = null,
                    IsSessionExpired = true,
                    ExpirationReason = "5 minute session time has expired"
                };

                return RequestResult<PlayerStatusDto>.Ok(dto);
            }

            var activeSessionDto = new PlayerStatusDto
            {
                PlayerId = playerId,
                PlaysRemaining = _session.PlaysRemaining(session.PlaysUsed),
                IsSessionExpired = false,
                SessionExpires = _session.ExpiresAt(startTime)
            };

            return RequestResult<PlayerStatusDto>.Ok(activeSessionDto);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "GetStatus failed for PlayerId: {PlayerId}", playerId);
            return RequestResult<PlayerStatusDto>.Failure("Failed to get status");
        }

    }

    public async Task<RequestResult<PlayResponseDto>> PlayAsync(string playerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId, cancellationToken).ConfigureAwait(false);
            if (player == null)
            {
                return RequestResult<PlayResponseDto>.NotFound("PlayerId not found. Try Logging In First");
            }

            var session = await GetOrCreateTodaySessionAsync(playerId, cancellationToken).ConfigureAwait(false);

            // daily max reached
            if (session.PlaysUsed >= SessionService.MaxPlaysPerDay)
            {
                var dto = new PlayResponseDto
                {
                    PlayerId = playerId,
                    Prize = PrizeType.NoPrize,
                    PlaysRemaining = 0,
                    SessionExpires = null,
                    IsSessionExpired = true,
                    ExpirationReason = "Max daily plays used"

                };

                return RequestResult<PlayResponseDto>.Ok(dto);
            }

            if (session.SessionStartTime.HasValue)
            {
                // session started - enforce 5 minute expiration
                var startTime = session.SessionStartTime.Value;
                if (_session.IsExpired(startTime))
                {
                    //lost plays
                    session.PlaysUsed = SessionService.MaxPlaysPerDay;
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    var dto = new PlayResponseDto
                    {
                        PlayerId = playerId,
                        Prize = PrizeType.NoPrize,
                        PlaysRemaining = 0,
                        SessionExpires = null,
                        IsSessionExpired = true,
                        ExpirationReason = "5 minute session time has expired"
                    };

                    return RequestResult<PlayResponseDto>.Ok(dto);
                }
            }
           
            // first play session time
            if (session.SessionStartTime is null)
            {
                session.SessionStartTime = _clock.UtcNow;
            }

            var prize = _prize.Spin();

            // gift rule: $10 free play if gift hit twice
            if (prize == PrizeType.GiftItem && player.HasWonGiftItem)
            {
                prize = PrizeType.FreePlay10;
            }
            else if (prize == PrizeType.GiftItem)
            {
                player.HasWonGiftItem = true;
            }

            session.PlaysUsed++;

            _context.PlayHistory.Add(new PlayHistory
            {
                Id = Guid.NewGuid(),
                PlayerId = playerId,
                LastPlayed = _clock.UtcNow,
                PrizeAwarded = prize
            });

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            DateTime? expiresAt = null;
            if (session.SessionStartTime.HasValue)
            {
                expiresAt = _session.ExpiresAt(session.SessionStartTime.Value);
            }

            var response = new PlayResponseDto
            {
                PlayerId = playerId,
                Prize = prize,
                PlaysRemaining = _session.PlaysRemaining(session.PlaysUsed),
                IsSessionExpired = false,
                SessionExpires = expiresAt,
            }; 

            return RequestResult<PlayResponseDto>.Ok(response);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Play failed for PlayerId: {PlayerId}", playerId);
            return RequestResult<PlayResponseDto>.Failure("Play failed");
        }
        
    }

    private async Task<GameSession> GetOrCreateTodaySessionAsync(string playerId, CancellationToken cancellationToken = default)
    {
        var today = _session.TodayUtc();

        var session = await _context.GameSessions.FirstOrDefaultAsync(o => o.PlayerId == playerId && o.Today == today, cancellationToken).ConfigureAwait(false);

        if (session is null)
        {
            session = new GameSession
            {
                Id = Guid.NewGuid(),
                PlayerId = playerId,
                Today = today,
                SessionStartTime = null,
                PlaysUsed = 0
            };
            _context.GameSessions.Add(session);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return session;
    }

}

