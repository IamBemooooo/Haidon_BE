using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;

namespace Haidon_BE.Application.Features.Users.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly ApplicationDbContext _dbContext;
    public CreateUserCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = request.PasswordHash,
            RoleId = request.RoleId,
            Status = (Domain.Enums.UserStatus)request.Status,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy,
            IsDeleted = false
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }
}
