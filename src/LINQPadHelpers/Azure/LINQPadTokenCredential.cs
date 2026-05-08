using Azure.Core;
using LINQPad;

namespace LINQPadHelpers.Azure;

public class LINQPadTokenCredential(string authority, string? userIdHint) : TokenCredential
{
    public override AccessToken GetToken (TokenRequestContext requestContext, CancellationToken cancelToken)
        => Task.Run(() => GetTokenAsync(requestContext, cancelToken).AsTask(), cancelToken).Result;

    public override async ValueTask<AccessToken> GetTokenAsync (TokenRequestContext requestContext,
                                                                CancellationToken cancelToken)
    {
        // Call LINQPad's AcquireTokenAsync method to authenticate interactively, and cache token in the LINQPad GUI.
        var auth = await Util.MSAL.AcquireTokenAsync (authority, requestContext.Scopes, userIdHint)
                             .ConfigureAwait (false);
        return new AccessToken (auth.AccessToken, auth.ExpiresOn);
    }

}
