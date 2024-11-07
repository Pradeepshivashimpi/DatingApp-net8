using System;

namespace API.Entities;

public class Message
{
   public int Id { get; set; }
   public required string SenderUsername { get; set; }
   public required string ReceipientUsername { get; set; }
   public required string Content { get; set; }
   public DateTime? DateRead { get; set; }
   public DateTime MessageSent { get; set; } = DateTime.UtcNow;
   public bool SenderDeleted { get; set; }
   public bool ReceipientDeleted { get; set; }

   // navigation properties
   public int SenderId { get; set; }
   public AppUser Sender { get; set; } = null!;
   public int ReceipientId { get; set; }
   public AppUser Receipient { get; set; } = null!;
}
