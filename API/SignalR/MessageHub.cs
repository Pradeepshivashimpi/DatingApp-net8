using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, 
     IMapper mapper) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];

        if(Context.User == null || string.IsNullOrEmpty(otherUser)) 
          throw new Exception("Cannot join group");
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
         var username = Context.User?.GetUsername() ?? throw new Exception("Could not get the user");

       if (username == createMessageDto.ReceipientUsername.ToLower())
           throw new HubException("You cannot message yourself");

       var sender = await userRepository.GetUserByUsernameAsync(username);

       var receipient = await userRepository.GetUserByUsernameAsync(createMessageDto.ReceipientUsername);

       if(receipient == null || sender == null || sender.UserName == null || receipient.UserName == null) 
          throw new HubException("Cannot send message at this time");

       var message = new Message
       {
         Sender = sender,
         Receipient = receipient,
         SenderUsername = sender.UserName,
         ReceipientUsername = receipient.UserName,
         Content = createMessageDto.Content
       };

       messageRepository.AddMessage(message);

       if(await messageRepository.SaveAllAsync()) 
       {
         var group = GetGroupName(sender.UserName, receipient.UserName);
         await Clients.Group(group).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
       }
    }

    private string GetGroupName(string caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
