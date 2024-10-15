using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwait.Task2.CodeReviewChallenge.Headers;
using CloudServices.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AsyncAwait.Task2.CodeReviewChallenge.Middleware;

public class StatisticMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IStatisticService _statisticService;

    public StatisticMiddleware(RequestDelegate next, IStatisticService statisticService)
    {
        _next = next;
        _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string path = context.Request.Path;
        //TODO: Optimize
        await _statisticService.RegisterVisitAsync(path);
        long visitsCount = await _statisticService.GetVisitsCountAsync(path);
        context.Response.Headers.TryAdd(CustomHttpHeaders.TotalPageVisits, visitsCount.ToString());

        //Thread.Sleep(3000); // without this the statistic counter does not work
        await _next(context);
    }
}
