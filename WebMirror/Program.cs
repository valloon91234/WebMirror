// See https://aka.ms/new-console-template for more information

using EmbedIO;
using EmbedIO.WebApi;
using Swan.Logging;
using System.Diagnostics;
using System.Text;
using EmbedIO.Files;
using System.Net.Http.Headers;

//Console.BufferHeight = Int16.MaxValue - 1;
//AppHelper.MoveWindow(AppHelper.GetConsoleWindow(), 24, 0, 1080, 280, true);
AppHelper.FixCulture();

//System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

if (Debugger.IsAttached)
{
    ConsoleLogger.Instance.LogLevel = LogLevel.Warning;
}
else
{
    AppHelper.QuickEditMode(false);
    Swan.Logging.Logger.UnregisterLogger<ConsoleLogger>();
}

const string targetHost = "ak47bet.com";
using var server = CreateWebServer("http://*:80/");
server.OnAny(async (ctx) =>
{
    var rawUrl = ctx.Request.RawUrl.ToString();
    Logger.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] \t {ctx.Request.RemoteEndPoint.Address.ToString().Split(":").Last()} \t {ctx.Request.HttpMethod} \t {rawUrl}");

    if (rawUrl == "/asset/Banner/banner_jk.jpg")
    {
        ctx.Redirect("/q/banner_jk.jpg");
        return;
    }
    else if (rawUrl == "/asset/Banner/Head%200.png")
    {
        ctx.Redirect("/q/Head_0.jpg");
        return;
    }
    else if (rawUrl == "/asset/Banner/Head%201.jpg")
    {
        ctx.Redirect("/q/Head_1.jpg");
        return;
    }
    else if (rawUrl == "/asset/Banner/Head%202.jpg")
    {
        ctx.Redirect("/q/Head_2.jpg");
        return;
    }
    else if (rawUrl == "/asset/Banner/Head%203.jpg")
    {
        ctx.Redirect("/q/Head_3.jpg");
        return;
    }
    else if (rawUrl == "/asset/Banner/Banner%20in%20web%201.png")
    {
        ctx.Redirect("/q/Banner in web 1.jpg");
        return;
    }
    else if (rawUrl == "/asset/Banner/Banner%20in%20web%202.png")
    {
        ctx.Redirect("/q/Banner in web 2.jpg");
        return;
    }
    else if (rawUrl == "/asset/Banner/Banner%20in%20web%203.png")
    {
        ctx.Redirect("/q/Banner in web 3.jpg");
        return;
    }

    var url = $"{ctx.Request.Url.Scheme}://{targetHost}" + rawUrl;
    Logger.WriteLine($"{url}", ConsoleColor.Green);
    var forwardedResponse = await ForwardRequestAsync(url, ctx);

    // Copy the response headers and content to the client's response
    ctx.Response.StatusCode = (int)forwardedResponse.StatusCode;
    //Logger.WriteLine($"response.StatusCode = {forwardedResponse.StatusCode}");
    foreach (var header in forwardedResponse.Headers)
    {
        if (header.Key.StartsWith("Accept-")) continue;
        //Logger.WriteLine($"response.Headers    *****    {header.Key}    {string.Join(", ", header.Value)}");
        ctx.Response.Headers.Add(header.Key, string.Join(", ", header.Value));
    }

    string? contentType = null;
    long? contentLength = null;
    foreach (var header in forwardedResponse.Content.Headers)
    {
        //Logger.WriteLine($"response.Content.Headers    ----    {header.Key}    {string.Join(", ", header.Value)}");
        if (header.Key == "Content-Type")
        {
            contentType = string.Join(", ", string.Join(", ", header.Value));
            ctx.Response.ContentType = contentType;
        }
        else if (header.Key == "Content-Length")
        {
            try
            {
                contentLength = long.Parse(string.Join(", ", string.Join(", ", header.Value)));
            }
            catch { }
        }
        else
            ctx.Response.Headers.Add(header.Key, string.Join(", ", header.Value));
    }

    //if (contentType != null && contentType.StartsWith("text", StringComparison.OrdinalIgnoreCase))
    //{
    //    var content = await response.Content.ReadAsStringAsync();
    //    content = content.ReplaceAll("", "");
    //    await ctx.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(content));
    //}
    //else
    //{
    //    var content = await forwardedResponse.Content.ReadAsByteArrayAsync();
    //    await ctx.Response.OutputStream.WriteAsync(content);
    //}

    if (contentType == "text/html")
    {
        var contentText = @"<iframe src=""/q/support.html"" title=""Contact Support"" width=""100%"" height=""240px""
style=""background: #fff;position: fixed;z-index: 999999;top: 0;""></iframe>
<style>body{margin-top: 240px !important;}</style>";
        if (contentLength != null)
            ctx.Response.ContentLength64 = contentLength.Value + contentText.Length;
        var content = await forwardedResponse.Content.ReadAsByteArrayAsync();
        await ctx.Response.OutputStream.WriteAsync(content);
        await ctx.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(contentText));
    }
    else
    {
        if (contentLength != null)
        {
            ctx.Response.ContentLength64 = contentLength.Value;
        }
        var content = await forwardedResponse.Content.ReadAsByteArrayAsync();
        await ctx.Response.OutputStream.WriteAsync(content);
    }

});
var task = server.RunAsync();
Console.WriteLine("Running...");
Console.ReadKey(true);

/**
 * https://github.com/unosquare/embedio
 */
static WebServer CreateWebServer(string url)
{
    var server = new WebServer(o => o
            .WithUrlPrefix(url)
            .WithMode(HttpListenerMode.EmbedIO))
        .WithStaticFolder("/q", "assets", true, m => m.WithContentCaching(true))
        .WithWebApi("/v1", m => m.WithController<V1Controller>());
    server.StateChanged += (s, e) => $"WebServer New State - {e.NewState}".Info();
    return server!;
}

static async Task<HttpResponseMessage> ForwardRequestAsync(string url, IHttpContext context)
{
    var requestMessage = new HttpRequestMessage
    {
        // Set the request method
        Method = new HttpMethod(context.Request.HttpMethod),

        // Set the request URI
        RequestUri = new Uri(url)
    };

    // Copy the request body, if any
    if (context.Request.HasEntityBody)
    {
        var content = await context.GetRequestBodyAsByteArrayAsync();
        requestMessage.Content = new ByteArrayContent(content);
    }

    // Copy the request headers
    var userHostName = context.Request.Headers["Host"];
    foreach (var key in context.Request.Headers.AllKeys)
    {
        if (key == null) continue;
        var value = context.Request.Headers[key];
        if (/* key == "Host" || key == "Origin" || key == "Referer" || */ key.StartsWith("Accept-"))
        {
            //Logger.WriteLine($"Request.Headers (dismissed)    ----    {key}    {value}", ConsoleColor.DarkYellow);
            continue;
        }
        if (value != null && userHostName != null)
            value = value.Replace(userHostName, targetHost);
        //Logger.WriteLine($"Request.Headers    ----    {key}    {value}", ConsoleColor.Green);

        if (key == "Content-Type" && value != null && requestMessage.Content != null)
            requestMessage.Content!.Headers.ContentType = new MediaTypeHeaderValue(value);
        else if (!requestMessage.Headers.TryAddWithoutValidation(key, value))
            requestMessage.Content?.Headers.TryAddWithoutValidation(key, value);
    }

    var httpClientHandler = new HttpClientHandler
    {
        AllowAutoRedirect = false
    };
    using var httpClient = new HttpClient(httpClientHandler);
    return await httpClient.SendAsync(requestMessage);
}

