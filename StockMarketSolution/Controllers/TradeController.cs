using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using StockMarketSolution.Models;

namespace StockMarketSolution.Controllers;

[Route("[controller]")]
public class TradeController : Controller
{
private readonly TradingOptions _tradingOptions;
private readonly IFinnhubService _finnhubService;
private readonly IConfiguration _configuration;
private readonly IStocksService _stocksService;

public TradeController(IOptions<TradingOptions> tradingOptions, IFinnhubService finnhubService, IConfiguration configuration
  ,IStocksService stocksService)
{
_tradingOptions = tradingOptions.Value;
_finnhubService = finnhubService;
_configuration = configuration;
_stocksService = stocksService;
}


[Route("/")]
[Route("[action]")]
[Route("~/[controller]")]
public IActionResult Index()
{
    if (string.IsNullOrEmpty(_tradingOptions.DefaultStockSymbol))
        _tradingOptions.DefaultStockSymbol = "MSFT";


    Dictionary<string, object>? companyProfileDictionary = _finnhubService.GetCompanyProfile(_tradingOptions.DefaultStockSymbol);

    Dictionary<string, object>? stockQuoteDictionary = _finnhubService.GetStockPriceQuote(_tradingOptions.DefaultStockSymbol);


    StockTrade stockTrade = new StockTrade() { StockSymbol = _tradingOptions.DefaultStockSymbol };

    if (companyProfileDictionary != null && stockQuoteDictionary != null)
    {
        stockTrade = new StockTrade() { StockSymbol = Convert.ToString(companyProfileDictionary["ticker"]), StockName = Convert.ToString(companyProfileDictionary["name"]), Price = Convert.ToDouble(stockQuoteDictionary["c"].ToString()) };
    }

    ViewBag.FinnhubToken = _configuration["FinnhubToken"];

    return View(stockTrade);
}

[HttpPost("[action]")]
public IActionResult SellOrder(SellOrderRequest sellOrderRequest)
    {
        sellOrderRequest.DateAndTimeOfOrder = DateTime.Now;

        ModelState.Clear();
        TryValidateModel(sellOrderRequest);


        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(v=>v.Errors).Select(v=>v.ErrorMessage).ToList();
            StockTrade stockTrade = new StockTrade() { StockName = sellOrderRequest.StockName, Quantity = sellOrderRequest.Quantity, StockSymbol = sellOrderRequest.StockSymbol };
            return View("Index", stockTrade);
        }

        SellOrderResponse sellOrderResponse = _stocksService.CreateSellOrder(sellOrderRequest);

        return RedirectToAction(nameof(Orders));
    }

    [HttpPost("[action]")]
    public IActionResult BuyOrder(BuyOrderRequest buyOrderRequest)
    {
        buyOrderRequest.DateAndTimeOfOrder = DateTime.Now;

        ModelState.Clear();
        TryValidateModel(buyOrderRequest);


        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToList();
            StockTrade stockTrade = new StockTrade() { StockName = buyOrderRequest.StockName, Quantity = buyOrderRequest.Quantity, StockSymbol = buyOrderRequest.StockSymbol };
            return View("Index", stockTrade);
        }

        BuyOrderResponse buyOrderResponse = _stocksService.CreateBuyOrder(buyOrderRequest);

        return RedirectToAction(nameof(Orders));
    }

    [Route("[action]")]
public IActionResult Orders()
{
   List<BuyOrderResponse> buyOrderResponses = _stocksService.GetBuyOrders();
   List<SellOrderResponse> sellOrderResponses = _stocksService.GetSellOrders();

    Orders orders = new Orders() { BuyOrders = buyOrderResponses , SellOrders = sellOrderResponses};

    return View(orders);

 }

}


