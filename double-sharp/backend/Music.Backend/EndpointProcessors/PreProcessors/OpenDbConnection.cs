using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;

namespace Music.Backend.EndpointProcessors.PreProcessors;

public class OpenDbConnection<TReq> : IPreProcessor<TReq>
{
    public async Task PreProcessAsync(IPreProcessorContext<TReq> context, CancellationToken ct)
    {
        var musicContext = context.HttpContext.Resolve<MusicContext>();
        await musicContext.Database.OpenConnectionAsync(cancellationToken: ct);
    }
}
