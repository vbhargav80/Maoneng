using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace MyApplication.Host
{
    public abstract class ApiGatewayHandlerBase
    {
        public APIGatewayProxyResponse Ok()
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public APIGatewayProxyResponse OK(string content)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = content,
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };
        }

        public APIGatewayProxyResponse Ok<T>(T content) where T : class
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(content),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        public APIGatewayProxyResponse NotFound()
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }

        public APIGatewayProxyResponse BadRequest(string parameterName)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = $"Missing required parameter {parameterName}"
            };
        }
    }
}
