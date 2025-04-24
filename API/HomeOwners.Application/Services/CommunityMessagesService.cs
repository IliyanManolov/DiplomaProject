using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.CommunityMessages;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class CommunityMessagesService : ICommunityMessagesService
{
    private readonly ICommunityMessagesRepository _messagesRepository;
    private readonly ILogger<CommunityMessagesService> _logger;

    public CommunityMessagesService(ICommunityMessagesRepository messagesRepository,
                                    ILoggerFactory loggerFactory)
    {
        _messagesRepository = messagesRepository;
        _logger = loggerFactory.CreateLogger<CommunityMessagesService>();
    }

    public async Task<long> CreateMessageAsync(CreateCommunityMessageDto model)
    {
        if (string.IsNullOrEmpty(model.Message))
            throw new InvalidPropertyValueValidationError("Community message cannot be empty/null");

        var dbMessage = new CommunityMessage()
        {
            CommunityId = model.CommunityId,
            CreatorId = model.CreatorId,
            Message = model.Message
        };

        await _messagesRepository.CreateAsync(dbMessage);

        return dbMessage.Id!.Value;
    }

    public async Task<IEnumerable<CommunityMessageDetailsDto>> GetForCommunityAsync(long communityId)
    {
        var dbMessages = await _messagesRepository.GetAllByCommunityId(communityId);

        return dbMessages.Select(x => new CommunityMessageDetailsDto()
        {
            Message = x.Message,
            CreateTimeStamp = x.CreateDate!.Value,
            CreatorUserName = x.Creator.Username!,
            LastUpdateTimeStamp = x.UpdateDate
        });
    }
}
