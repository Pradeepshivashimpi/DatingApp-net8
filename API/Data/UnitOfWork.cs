using System;
using API.Interfaces;

namespace API.Data;

public class UnitOfWork(DataContext context, IUserRepository userRepository, 
   IMessageRepository messageRepository, ILikesRepository likesRepository) : IUnitOfWork
{
    public IUserRepository userRepository => userRepository;

    public IMessageRepository messageRepository => messageRepository;

    public ILikesRepository likesRepository => likesRepository;

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
