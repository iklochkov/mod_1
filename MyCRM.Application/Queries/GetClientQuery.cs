using MediatR;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Queries;

public class GetClientQuery : IRequest<ClientDto?>
{
    public int Id { get; set; }
    
    public GetClientQuery() { }
    
    public GetClientQuery(int id)
    {
        Id = id;
    }
}
