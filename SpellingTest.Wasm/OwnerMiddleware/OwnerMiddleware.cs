
using Microsoft.AspNetCore.Http;
using PolyhydraGames.Core.Interfaces;

namespace SpellingTest.Wasm.OwnerMiddleware;

public   class OwnerMiddleware
{
    //private readonly RequestDelegate _next;

    //public OwnerMiddleware(RequestDelegate next)
    //{
    //    _next = next;
    //}

    //public async Task InvokeAsync(HttpContext context, IOwnerService service)
    //{ 
    //        service.AuthorizationToken = await context.GetTokenAsync("access_token");
    //        service.OwnerId = context.GetUserId();
    //        await _next(context);
    //}
}