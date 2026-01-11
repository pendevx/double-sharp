namespace MoneySum.API.Endpoints

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Routing

module AccountAccessConsent =

    let testEndpoint () =
        Results.Ok "hi"

    testEndpoint.Completed <- System.Action<_>(f)

    let mapAccountAccessConsent (routes: RouteGroupBuilder) =
        routes.MapGet("/", Func<IResult>(fun () -> testEndpoint())) |> ignore

        routes
