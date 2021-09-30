using MediatR;
using MyApplication.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyApplication.Domain.Queries
{
    public class GetTodoItemsQuery: IRequest<IEnumerable<TodoItem>>
    {
    }

    public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, IEnumerable<TodoItem>>
    {
        private readonly IRepository<TodoItem> _todoItemsRepository;

        public GetTodoItemsQueryHandler(IRepository<TodoItem> todoItemsRepository)
        {
            _todoItemsRepository = todoItemsRepository;
        }

        public Task<IEnumerable<TodoItem>> Handle(
            GetTodoItemsQuery request, CancellationToken cancellationToken)
        {
            return _todoItemsRepository.GetAll();
        }
    }
}
