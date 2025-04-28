using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class CommunityMeetingService : ICommunityMeetingService
{
    private readonly ICommunityMeetingRepository _repository;
    private readonly ILogger<CommunityMeetingService> _logger;

    public CommunityMeetingService(ICommunityMeetingRepository repository, ILoggerFactory loggerFactory)
    {
        _repository = repository;
        _logger = loggerFactory.CreateLogger<CommunityMeetingService>();
    }
}
