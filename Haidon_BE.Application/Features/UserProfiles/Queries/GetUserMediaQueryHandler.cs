using Haidon_BE.Domain.Entities;
using Haidon_BE.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haidon_BE.Application.Features.UserProfiles.Queries;

public class GetUserMediaQueryHandler : IRequestHandler<GetUserMediaQuery, List<UserMedia>>
{
    private readonly ApplicationDbContext _dbContext;
    public GetUserMediaQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserMedia>> Handle(GetUserMediaQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _dbContext.UserMedias.AsQueryable();
            if (request.UserId.HasValue)
            {
                query = query.Where(m => m.UserId == request.UserId.Value);
            }
            return await query.OrderBy(m => m.Order).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log exception if needed
            throw new Exception("Có l?i x?y ra khi l?y danh sách media", ex);
        }
    }
}
