using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Queries
{
    public class GetAllClientQuery : IRequest<PagedResponse<ClientDto>>
    {
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Name { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string SortBy { get; set; } = "Id";
    public string SortOrder { get; set; } = "asc";
    }
}