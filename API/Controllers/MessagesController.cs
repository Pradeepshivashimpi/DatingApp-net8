using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, 
  IMapper mapper) : BaseApiController
{
   [HttpPost]
   public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
   {
       var username = User.GetUsername();

       if (username == createMessageDto.ReceipientUsername.ToLower())
           return BadRequest("You cannot message yourself");

       var sender = await userRepository.GetUserByUsernameAsync(username);

       var receipient = await userRepository.GetUserByUsernameAsync(createMessageDto.ReceipientUsername);

       if(receipient == null || sender == null || sender.UserName == null || receipient.UserName == null) 
         return BadRequest("Cannot send message at this time");

       var message = new Message
       {
         Sender = sender,
         Receipient = receipient,
         SenderUsername = sender.UserName,
         ReceipientUsername = receipient.UserName,
         Content = createMessageDto.Content
       };

       messageRepository.AddMessage(message);

       if(await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));

       return BadRequest("Failed to save message");
   }

   [HttpGet]
   public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser(
      [FromQuery] MessageParams messageParams)
      {
        messageParams.Username = User.GetUsername();

        var messages = await messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages);
        return messages;
      }


    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
       var currentUsername = User.GetUsername();

       return Ok(await messageRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
      var username = User.GetUsername();

      var message = await messageRepository.GetMessage(id);

      if(message == null) return BadRequest("Cannot delete this message");

      if(message.SenderUsername != username && message.ReceipientUsername != username)
          return Forbid();
      
      if(message.SenderUsername == username) message.SenderDeleted = true;
      if(message.ReceipientUsername == username) message.ReceipientDeleted = true;

      if(message is {SenderDeleted: true, ReceipientDeleted: true}) {
          messageRepository.DeleteMessage(message);
      }

      if(await messageRepository.SaveAllAsync()) return Ok();

      return BadRequest("Problem deleting message");
    }
}
