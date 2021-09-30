using Amazon.Lambda.APIGatewayEvents;

namespace MyApplication.Host
{
    public static class APIGatewayProxyRequestExtensions
    {
        public static string GetParameterValue(this APIGatewayProxyRequest request, string parameterName)
        {
            string todoItemId = null;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(parameterName))
                todoItemId = request.PathParameters[parameterName];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(parameterName))
                todoItemId = request.QueryStringParameters[parameterName];

            return todoItemId;
        }
    }
}
