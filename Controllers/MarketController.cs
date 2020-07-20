using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using YahooFinanceApi;

namespace finapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MarketController : ControllerBase
    {
        private readonly ILogger<MarketController> _logger;
        private readonly FinApiContext Context;

        public MarketController(ILogger<MarketController> logger, FinApiContext context)
        {
            _logger = logger;
            Context = context;
        }

        [HttpGet]
        public async Task<object> Get(string s)
        {
            var now = DateTime.UtcNow;
            var from = now.AddDays(-7);
            var markets = new[] { "SPY", s };
            var t = markets
                .Select(async s => new { m = s, d = await Yahoo.GetHistoricalAsync(s, from, now) })
                .ToArray();

            var data = (await Task
                .WhenAll(t))
                .ToDictionary(s => s.m, s => GetPerformance(q => q.AdjustedClose, s.d));
            
            await Context.AddAsync(new Market
            {
                Date = now,
                MarketId = s,
                ProviderId = "Yahoo",
                Response = JsonConvert.SerializeObject(data)
            });

            await Context.SaveChangesAsync();
            return data;
        }

        static IEnumerable<decimal> GetPerformance(Func<Candle, decimal> selector, IEnumerable<Candle> v)
        {
            var b = selector(v.First());
            return v.Select(s => (selector(s) / b - 1) * 100);
        }
    }
}
