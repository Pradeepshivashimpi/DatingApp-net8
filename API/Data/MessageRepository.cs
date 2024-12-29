using System;
using System.Xml.Linq;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
{
    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Connection?> GetConnection(string ConnectionId)
    {
        return await context.Connections.FindAsync(ConnectionId);
    }

    public async Task<Group?> GetGroupForConnection(string ConnectionId)
    {
        return await context.Groups
           .Include(x => x.Connections)
           .Where(x => x.Connections.Any(c => c.ConnectionId == ConnectionId))
           .FirstOrDefaultAsync();
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await context.Groups
          .Include(x => x.Connections)
          .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = context.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.Receipient.UserName == messageParams.Username && 
                  x.ReceipientDeleted == false),
            "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username && 
                  x.SenderDeleted == false),
            _ => query.Where(x => x.Receipient.UserName == messageParams.Username && x.DateRead == null && 
                  x.ReceipientDeleted == false)
        };

        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber,
                messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string receipientUsername)
    {
        var messages = await context.Messages
                       .Where(x => 
                          x.ReceipientUsername == currentUsername && x.ReceipientDeleted == false 
                              && x.SenderUsername == receipientUsername ||
                          x.SenderUsername == currentUsername && x.SenderDeleted == false 
                              && x.ReceipientUsername == receipientUsername
                        )
                        .OrderBy(x => x.MessageSent)
                        .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
                        .ToListAsync();

        
        var unreadMessages = messages.Where(x => x.DateRead == null && 
            x.ReceipientUsername == currentUsername).ToList();

        if(unreadMessages.Count != 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }

        return messages;
    }

    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
    }
}
