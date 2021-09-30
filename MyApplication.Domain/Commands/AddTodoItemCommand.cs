using MediatR;
using MyApplication.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyApplication.Domain.Commands
{
    public class AddTodoItemCommand: IRequest<Unit>
    {
        public AddTodoItemCommand(TodoItem todoItem)
        {
            TodoItem = todoItem ?? throw new ArgumentNullException(nameof(todoItem));
        }

        public TodoItem TodoItem { get; set; }
    }

    public class AddTodoItemCommandHandler : IRequestHandler<AddTodoItemCommand, Unit>
    {
        private readonly IRepository<TodoItem> _repository;

        public AddTodoItemCommandHandler(IRepository<TodoItem> repository)
        {
            _repository = repository;
        }
        public async Task<Unit> Handle(
            AddTodoItemCommand request, CancellationToken cancellationToken)
        {
            request.TodoItem.Id = Guid.NewGuid().ToString();
            request.TodoItem.CreatedOn = DateTime.Now;

            await _repository.Save(request.TodoItem);
            return Unit.Value;
        }
    }
}
