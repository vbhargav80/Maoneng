using MediatR;
using MyApplication.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MyApplication.Domain.Queries
{
    public class GetTodoItemQuery : IRequest<TodoItem>
    {
        public string Id { get; set; }
        public GetTodoItemQuery(string id)
        {
            Id = id;
        }
    }

    public class GetTodoItemQueryHandler : IRequestHandler<GetTodoItemQuery, TodoItem>
    {
        private readonly IRepository<TodoItem> _repository;

        public GetTodoItemQueryHandler(IRepository<TodoItem> repository)
        {
            _repository = repository;
        }

        public Task<TodoItem> Handle(GetTodoItemQuery request, CancellationToken cancellationToken)
        {
            return _repository.Get(request.Id);
        }
    }
}
