using MediatR;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Queries;

public class GetDealQuery : IRequest<DealDto?>
{
    public int Id { get; set; }
    
    public GetDealQuery() { }
    
    public GetDealQuery(int id)
    {
        Id = id;
    }
}
