﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Application.ActionFilters
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class UseCacheAttribute : ActionFilterAttribute
	{
		private readonly int _timeToLiveSeconds;

		public UseCacheAttribute(int timeToLiveSeconds)
		{
			_timeToLiveSeconds = timeToLiveSeconds;
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var cacheSettings = context.HttpContext.RequestServices.GetRequiredService<RedisCacheSettings>();
			if (!cacheSettings.Enabled)
			{
				await next();
				return;
			}

			var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
			var cacheKey = context.HttpContext.Request.GenerateCacheKeyFromRequest();
			var cachedResponse = await cacheService.GetCacheResponseAsync(cacheKey);

			if (cachedResponse != null)
			{
				context.Result = new OkObjectResult(cachedResponse);
				return;
			}

			var executedContext = await next();

			if (executedContext.Result is OkObjectResult okObjectResult)
				await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value,
					TimeSpan.FromSeconds(_timeToLiveSeconds));
		}
	}

	public static class CacheExtensions
	{
		public static string GenerateCacheKeyFromRequest(this HttpRequest request)
		{
			var cacheKeyBuilder = new StringBuilder(request.Path);

			if (request.Query.Any())
				cacheKeyBuilder = request.Query
					.OrderBy(q => q.Key)
					.Aggregate(
						cacheKeyBuilder,
						(orderedQueryBuilder, currentQueryStringPair) =>
						{
							var previousFullValue = orderedQueryBuilder
								.Append(orderedQueryBuilder.ToString() == request.Path.ToString() ? "?" : "&")
								.ToString();
							orderedQueryBuilder.Clear()
								.Append(
									$"{previousFullValue}{currentQueryStringPair.Key}={currentQueryStringPair.Value}");
							return orderedQueryBuilder;
						});

			cacheKeyBuilder = request.Headers
				.Where(header =>
					!string.IsNullOrWhiteSpace(header.Value) && !header.Key.ToLowerInvariant().Contains("token"))
				.Aggregate(cacheKeyBuilder,
					(headerBuilder, header) => headerBuilder.Append($"|{header.Key}={header.Value}"));

			return cacheKeyBuilder.ToString();
		}
	}
}