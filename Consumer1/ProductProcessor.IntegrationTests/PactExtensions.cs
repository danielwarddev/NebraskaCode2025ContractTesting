using System.Net;
using PactNet;
using Match = PactNet.Matchers.Match;

namespace ProductProcessor.IntegrationTests;

public static class PactExtensions
{
    public static IRequestBuilderV4 WithHeaders(this IRequestBuilderV4 builder, IEnumerable<HttpHeader> headers)
    {
        foreach (var header in headers)
        {
            builder = builder.WithHeader(header.Key, header.Value);
        }

        return builder;
    }
    
    public static IRequestBuilderV4 UponReceivingGet(
        this IPactBuilderV4 builder,
        string path,
        string description,
        string providerState,
        IDictionary<string, string>? parameters = null)
    { 
        var requestBuilder = builder 
            .UponReceiving(description)
            .WithRequest(HttpMethod.Get, path)
            .WithHeaders(ProductClient.RequestHeaders);
        
        if (parameters == null)
        {
            requestBuilder.Given(providerState);
        }
        else
        {
            requestBuilder.Given(providerState, parameters);
        }

        return requestBuilder;
    }

    public static IResponseBuilderV4 WillRespondOk(
        this IRequestBuilderV4 builder,
        dynamic body)
    {
        return builder
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(Match.Type(body));
    }
}