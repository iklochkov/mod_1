using MediatR;

namespace MyCRM.Application.Commands.Deals;

public class DeleteDealCommand : IRequest<bool>
{
    public int Id { get; set; }
    
    public DeleteDealCommand() { }
    
    public DeleteDealCommand(int id)
    {
        Id = id;
    }
}
