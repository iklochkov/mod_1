using MediatR;

namespace MyCRM.Application.Commands.Clients;

public class DeleteClientCommand : IRequest<bool>
{
    public int Id { get; set; }
    
    public DeleteClientCommand() { }
    
    public DeleteClientCommand(int id)
    {
        Id = id;
    }
}
