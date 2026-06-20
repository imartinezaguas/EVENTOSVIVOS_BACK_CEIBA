using System.Collections.Generic;

namespace EventosVivos.Application.DTOs;

public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)System.Math.Ceiling(TotalRecords / (double)PageSize) : 0;
}
