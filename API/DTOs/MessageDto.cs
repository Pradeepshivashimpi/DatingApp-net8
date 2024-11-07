using System;

namespace API.DTOs;

public class MessageDto
{
   public int Id { get; set; }
   public int SenderId { get; set; }
   public required string SenderUsername { get; set; }
   public required string SenderPhotoUrl { get; set; }
   public int ReceipientId { get; set; }
   public required string ReceipientUsername { get; set; }
   public required string ReceipientPhotoUrl { get; set; }
   public required string Content { get; set; }
   public DateTime? DateRead { get; set; }
   public DateTime MessageSent { get; set; } 
}
