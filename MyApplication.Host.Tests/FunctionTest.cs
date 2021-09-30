using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using MyApplication.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyApplication.Host.Tests
{
    public class FunctionTest : IDisposable
    { 
        string TableName { get; }
        IAmazonDynamoDB DDBClient { get; }
        
        public FunctionTest()
        {
            this.TableName = "MyApplication.Host-TodoItems";
            System.Environment.SetEnvironmentVariable("TodoItem", this.TableName);
            this.DDBClient = new AmazonDynamoDBClient(RegionEndpoint.APSoutheast2);

            SetupTableAsync().Wait();
        }

        //[Fact(Skip ="Fix Tests Later")]
        [Fact]
        public async Task TodoItemTestAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions();


            // Add a new todo item
            TodoItem myTodoItem = new TodoItem();
            myTodoItem.Name = "First thing to do";
            myTodoItem.Description = "Some description should go in here";

            request = new APIGatewayProxyRequest
            {
                Body = JsonConvert.SerializeObject(myTodoItem)
            };
            context = new TestLambdaContext();
            response = await functions.AddTodoItemAsync(request, context);
            Assert.Equal(200, response.StatusCode);

            var todoItemId = response.Body.Trim('"');

            // Confirm we can get the todo item back out
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, todoItemId } }
            };
            context = new TestLambdaContext();
            response = await functions.GetTodoItemAsync(request, context);
            Assert.Equal(200, response.StatusCode);

            TodoItem readTodoItem = JsonConvert.DeserializeObject<TodoItem>(response.Body);
            Assert.Equal(myTodoItem.Name, readTodoItem.Name);
            Assert.Equal(myTodoItem.Description, readTodoItem.Description);

            // List the todo items
            request = new APIGatewayProxyRequest
            {
            };
            context = new TestLambdaContext();
            response = await functions.GetTodoItemsAsync(request, context);
            Assert.Equal(200, response.StatusCode);

            TodoItem[] todoItems = JsonConvert.DeserializeObject<TodoItem[]>(response.Body);
			Assert.Single(todoItems);
            Assert.Equal(myTodoItem.Name, todoItems[0].Name);
            Assert.Equal(myTodoItem.Description, todoItems[0].Description);


            // Delete the todo item 
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, todoItemId } }
            };
            context = new TestLambdaContext();
            response = await functions.RemoveTodoItemAsync(request, context);
            Assert.Equal(200, response.StatusCode);

            // Make sure the todo item was deleted.
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, todoItemId } }
            };
            context = new TestLambdaContext();
            response = await functions.GetTodoItemAsync(request, context);
            Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
        }



        /// <summary>
        /// Create the DynamoDB table for testing. This table is deleted as part of the object dispose method.
        /// </summary>
        /// <returns></returns>
        private async Task SetupTableAsync()
        {
            
            CreateTableRequest request = new CreateTableRequest
            {
                TableName = this.TableName,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 3,
                    WriteCapacityUnits = 1
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        KeyType = KeyType.HASH,
                        AttributeName = Functions.ID_QUERY_STRING_NAME
                    }
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = Functions.ID_QUERY_STRING_NAME,
                        AttributeType = ScalarAttributeType.S
                    }
                }
            };

            await this.DDBClient.CreateTableAsync(request);

            var describeRequest = new DescribeTableRequest { TableName = this.TableName };
            DescribeTableResponse response = null;
            do
            {
                Thread.Sleep(1000);
                response = await this.DDBClient.DescribeTableAsync(describeRequest);
            } while (response.Table.TableStatus != TableStatus.ACTIVE);
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.DDBClient.DeleteTableAsync(this.TableName).Wait();
                    this.DDBClient.Dispose();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


    }
}
