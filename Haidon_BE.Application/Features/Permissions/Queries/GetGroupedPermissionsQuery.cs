using System.Collections.Generic;
using Haidon_BE.Application.Features.Permissions.Dtos;
using MediatR;

namespace Haidon_BE.Application.Features.Permissions.Queries;

public class GetGroupedPermissionsQuery : IRequest<Dictionary<string, List<PermissionDto>>> {}
