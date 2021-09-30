using MediatR;
using MyApplication.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MyApplication.Domain.Commands
{
    public class RemoveTodoItemCommand : IRequest<Unit>
    {
        public string Id { get; set; }

        public RemoveTodoItemCommand(string id)
        {
            Id = id;
        }
    }

    public class RemoveTodoItemCommandHandler : IRequestHandler<RemoveTodoItemCommand, Unit>
    {
        private readonly IRepository<TodoItem> _repository;

        public RemoveTodoItemCommandHandler(IRepository<TodoItem> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveTodoItemCommand request, CancellationToken cancellationToken)
        {
            await _repository.Remove(request.Id);

            return Unit.Value;
        }
    }
}
