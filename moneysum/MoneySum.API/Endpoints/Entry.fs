namespace MoneySum.API.Endpoints

open AccountAccessConsent
open Microsoft.AspNetCore.Builder

module Entry =

    let registerEndpoints (app: WebApplication) =
        app.MapGroup("/account-access-consent")
