using Amazon.DynamoDBv2.DataModel;
using MyApplication.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApplication.Infrastructure
{
    public class DyamoDbRepository<T> : IRepository<T>
    {
        private readonly IDynamoDBContext dynamoDBContext;

        public DyamoDbRepository(IDynamoDBContext dynamoDBContext)
        {
            this.dynamoDBContext = dynamoDBContext;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dynamoDBContext
                .ScanAsync<T>(null)
                .GetNextSetAsync();
        }

        public async Task<T> Get(string id)
        {
            return await dynamoDBContext.LoadAsync<T>(id);
        }

        public async Task Save(T item)
        {
            await dynamoDBContext.SaveAsync(item);
        }

        public async Task Remove(string id)
        {
            await dynamoDBContext.DeleteAsync<T>(id);
        }
    }
}
