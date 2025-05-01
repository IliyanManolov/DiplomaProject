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

        if (model.Message.Length > 512)
            throw new InvalidPropertyValueValidationError("Invalid community message");

        var dbMessage = new CommunityMessage()
        {
            CommunityId = model.CommunityId,
            CreatorId = model.CreatorId,
            Message = model.Message
        };

        await _messagesRepository.CreateAsync(dbMessage);

        return dbMessage.Id!.Value;
    }

    public async Task<CommunityMessageDetailsDto> GetByIdAsync(long? id)
    {
        if (id == null)
            throw new InvalidPropertyValueValidationError("Invalid message id");

        var dbMessage = await _messagesRepository.GetByIdAsync(id);

        if (dbMessage == null)
        {
            _logger.LogInformation("Message with ID '{id}' not found ", id);
            throw new InvalidPropertyValueValidationError("Invalid message id");
        }

        return new CommunityMessageDetailsDto()
        {
            Id = dbMessage.Id!.Value,
            CommunityId = dbMessage.CommunityId!.Value,
            Message = dbMessage.Message,
            CreateTimeStamp = dbMessage.CreateDate!.Value,
            CreatorUserName = dbMessage.Creator.Username!,
            LastUpdateTimeStamp = dbMessage.UpdateDate
        };
    }

    public async Task<IEnumerable<CommunityMessageDetailsDto>> GetForCommunityAsync(long communityId)
    {
        var dbMessages = await _messagesRepository.GetAllByCommunityId(communityId);

        return dbMessages.Select(x => new CommunityMessageDetailsDto()
        {
            Id = x.Id!.Value,
            CommunityId = x.CommunityId!.Value,
            Message = x.Message,
            CreateTimeStamp = x.CreateDate!.Value,
            CreatorUserName = x.Creator.Username!,
            LastUpdateTimeStamp = x.UpdateDate
        });
    }

    public async Task<CommunityMessageDetailsDto> UpdateMessage(EditCommunityMessageDto model)
    {
        if (string.IsNullOrEmpty(model.NewMessage))
            throw new InvalidPropertyValueValidationError("Community message cannot be empty/null");

        if (model.NewMessage.Length > 512)
            throw new InvalidPropertyValueValidationError("Invalid community message");

        if (model.Id == null)
            throw new InvalidPropertyValueValidationError("Invalid id");

        var dbMessage = await _messagesRepository.GetByIdAsync(model.Id);

        if (dbMessage == null)
        {
            _logger.LogWarning("Message not found in database");
            throw new InvalidPropertyValueValidationError("Invalid id");
        }

        if (model.NewMessage == dbMessage.Message)
            throw new InvalidPropertyValueValidationError("Cannot edit with same message");

        dbMessage.Message = model.NewMessage;

        await _messagesRepository.UpdateAsync(dbMessage);

        return new CommunityMessageDetailsDto()
        {
            Id = dbMessage.Id!.Value,
            CommunityId = dbMessage.CommunityId!.Value,
            Message = dbMessage.Message,
            CreateTimeStamp = dbMessage.CreateDate!.Value,
            CreatorUserName = dbMessage.Creator.Username!,
            LastUpdateTimeStamp = dbMessage.UpdateDate
        };
    }
}
