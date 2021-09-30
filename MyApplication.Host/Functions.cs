using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyApplication.Domain.Commands;
using MyApplication.Domain.Entities;
using MyApplication.Domain.Queries;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyApplication.Host
{
    public class Functions : ApiGatewayHandlerBase
    {
        public const string ID_QUERY_STRING_NAME = "Id";
        private IMediator Mediator;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            this.Mediator = Startup.ConfigureServices().GetService<IMediator>();
        }

        /// <summary>
        /// A Lambda function that returns back a page worth of todo items.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of todo items</returns>
        public async Task<APIGatewayProxyResponse> GetTodoItemsAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Getting todo items");
            var todoItems = await Mediator.Send(new GetTodoItemsQuery());
            context.Logger.LogLine($"Found {todoItems.Count()} todo items");

            return Ok(todoItems);
        }

        /// <summary>
        /// A Lambda function that returns the todoItem identified by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> GetTodoItemAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var todoItemId = request.GetParameterValue(ID_QUERY_STRING_NAME);

            if (string.IsNullOrEmpty(todoItemId))
                return BadRequest(ID_QUERY_STRING_NAME);

            context.Logger.LogLine($"Getting todo item {todoItemId}");
            var todoItem = await Mediator.Send(new GetTodoItemQuery(todoItemId));
            context.Logger.LogLine($"Found todo item: {todoItem != null}");

            if (todoItem == null)
                return NotFound();

            return Ok(todoItem);
        }

        /// <summary>
        /// A Lambda function that adds a todo item.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> AddTodoItemAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var todoItem = JsonConvert.DeserializeObject<TodoItem>(request?.Body);

            context.Logger.LogLine($"Saving todo item with id {todoItem.Id}");
            await Mediator.Send(new AddTodoItemCommand(todoItem));

            return Ok(todoItem.Id);
        }

        /// <summary>
        /// A Lambda function that removes a todo item from the DynamoDB table.
        /// </summary>
        /// <param name="request"></param>
        public async Task<APIGatewayProxyResponse> RemoveTodoItemAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var todoItemId = request.GetParameterValue(ID_QUERY_STRING_NAME);

            if (string.IsNullOrEmpty(todoItemId))
                return BadRequest(ID_QUERY_STRING_NAME);

            context.Logger.LogLine($"Deleting todo item with id {todoItemId}");
            await Mediator.Send(new RemoveTodoItemCommand(todoItemId));

            return Ok();
        }
    }
}
