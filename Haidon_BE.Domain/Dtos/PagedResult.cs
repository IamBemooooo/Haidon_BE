using System.Collections.Generic;

namespace Haidon_BE.Domain.Dtos;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)System.Math.Ceiling((double)TotalCount / PageSize);
}
