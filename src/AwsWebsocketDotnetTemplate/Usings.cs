global using System.Text.Json;
global using Amazon.Lambda.Core;
global using Amazon.Lambda.APIGatewayEvents;
global using Amazon.Lambda.Serialization.SystemTextJson;
global using Amazon.DynamoDBv2;
global using Amazon.DynamoDBv2.Model;
global using Amazon.ApiGatewayManagementApi;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]