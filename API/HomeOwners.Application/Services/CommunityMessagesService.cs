using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class CommunityMessagesService : ICommunityMessagesService
{
    private readonly ICommunityMessagesRepository _messagesRepository;
    private readonly ICommunityRepository _communityRepository;
    private readonly ILogger<CommunityMessagesService> _logger;

    public CommunityMessagesService(ICommunityMessagesRepository messagesRepository, ICommunityRepository communityRepository, ILoggerFactory loggerFactory)
    {
        _messagesRepository = messagesRepository;
        _communityRepository = communityRepository;
        _logger = loggerFactory.CreateLogger<CommunityMessagesService>();
    }
}
