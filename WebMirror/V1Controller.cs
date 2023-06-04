using EmbedIO;
using EmbedIO.WebApi;
using EmbedIO.Routing;

/**
 * https://github.com/unosquare/embedio
 * https://github.com/unosquare/embedio/wiki/Cookbook
 */
internal class V1Controller : WebApiController
{
    private static readonly Logger logger = new($"{DateTime.UtcNow:yyyy-MM-dd}");

    [Route(HttpVerbs.Get, "/tt")]
    public void GetBinaryText()
    {
        using var writer = HttpContext.OpenResponseText();
        writer.WriteAsync($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
    }

    //[Route(HttpVerbs.Get, "/test/{addr}")]
    //public void GetTestAddr(string addr)
    //{
    //    var result = AddressConverter.Fix(addr);
    //    using var writer = HttpContext.OpenResponseText();
    //    writer.WriteAsync(result);
    //}

    //[Route(HttpVerbs.Post, "/ping")]
    //public void PostPing()
    //{
    //    using MemoryStream memoryStream = new();
    //    HttpContext.OpenRequestStream().CopyTo(memoryStream);
    //    var requestBytes = memoryStream.ToArray();
    //    var responseBytes = v1PostPing(requestBytes, HttpContext.Request.SafeGetRemoteEndpointStr());
    //    using var stream = HttpContext.OpenResponseStream();
    //    stream.Write(responseBytes);
    //}

}
