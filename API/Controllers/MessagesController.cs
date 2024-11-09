using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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

       if(receipient == null | sender == null) return BadRequest("Cannot send message at this time");

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
}
