using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.Roles.Commands;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
{
    private readonly ApplicationDbContext _dbContext;
    public DeleteRoleCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);
        if (role == null) return false;
        role.IsDeleted = true;
        role.UpdatedAt = DateTime.UtcNow;
        role.UpdatedBy = request.UpdatedBy;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
