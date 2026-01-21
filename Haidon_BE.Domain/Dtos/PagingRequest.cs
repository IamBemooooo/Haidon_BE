namespace Haidon_BE.Domain.Dtos;

public class PagingRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
