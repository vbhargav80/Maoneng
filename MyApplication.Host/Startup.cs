using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyApplication.Domain;
using MyApplication.Domain.Entities;
using MyApplication.Domain.Queries;
using MyApplication.Infrastructure;
using System;
using System.Reflection;

namespace MyApplication.Host
{
    public static class Startup
    {
        // This const is the name of the environment variable that the serverless.template will use to set
        // the name of the DynamoDB table used to store blog posts.
        const string TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP = "TodoItem";

        public static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddMediatR(typeof(GetTodoItemsQuery).GetTypeInfo().Assembly);
            serviceCollection.AddTransient(typeof(IRepository<>), typeof(DyamoDbRepository<>));

            RegisterDynamoDb(serviceCollection);

            return serviceCollection.BuildServiceProvider();

        }

        private static void RegisterDynamoDb(ServiceCollection serviceCollection)
        {
            // Check to see if a table name was passed in through environment variables and if so 
            // add the table mapping.
            // MOVE ACCESS KEY AND SECRET TO Secrets Manager
            var credentials = new BasicAWSCredentials("<Add Your Credentials Here>", "<Add Your Credentials Here>");
            var dbconfig = new AmazonDynamoDBConfig()
            {
                RegionEndpoint = RegionEndpoint.APSoutheast2
            };

            var tableName = System.Environment.GetEnvironmentVariable(TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP);
            if (!string.IsNullOrEmpty(tableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(TodoItem)] = new Amazon.Util.TypeMapping(typeof(TodoItem), tableName);
            }

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            var dbContext = new DynamoDBContext(new AmazonDynamoDBClient(credentials, dbconfig), config);

            serviceCollection.AddSingleton<IDynamoDBContext>(dbContext);
        }
    }
}
