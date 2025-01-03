﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using StockMarketSolution.Filters.ActionFilters;
using StockMarketSolution.Models;

namespace StockMarketSolution.Controllers;

[Route("[controller]")]
public class TradeController : Controller
{
    private readonly TradingOptions _tradingOptions;
    private readonly IStocksService _stocksService;
    private readonly IFinnhubService _finnhubService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TradeController> _logger;

    public TradeController(IOptions<TradingOptions> tradingOptions, IStocksService stocksService, IFinnhubService finnhubService,
        IConfiguration configuration, ILogger<TradeController> logger)
    {
        _tradingOptions = tradingOptions.Value;
        _stocksService = stocksService;
        _finnhubService = finnhubService;
        _configuration = configuration;
        _logger = logger;
    }

    [Route("[action]")]
    [Route("[action]/{stockSymbol}")]
    [Route("~/[controller]/{stockSymbol}")]
    public async Task<IActionResult> Index(string stockSymbol)
    {
        _logger.LogInformation("In TradeController.Index() action method");
        _logger.LogDebug("stockSymbol: {stockSymbol}", stockSymbol);
        if (string.IsNullOrEmpty(stockSymbol))
            stockSymbol = "MSFT";


        Dictionary<string, object>? companyProfileDictionary = await _finnhubService.GetCompanyProfile(stockSymbol);

        Dictionary<string, object>? stockQuoteDictionary = await _finnhubService.GetStockPriceQuote(stockSymbol);


        StockTrade stockTrade = new StockTrade() { StockSymbol = stockSymbol };

        if (companyProfileDictionary != null && stockQuoteDictionary != null)
        {
            stockTrade = new StockTrade() { StockSymbol = companyProfileDictionary["ticker"].ToString(), StockName = companyProfileDictionary["name"].ToString(), Quantity = _tradingOptions.DefaultOrderQuantity ?? 0, Price = Convert.ToDouble(stockQuoteDictionary["c"].ToString()) };
        }

        ViewBag.FinnhubToken = _configuration["FinnhubToken"];

        return View(stockTrade);
    }


    [Route("[action]")]
    [HttpPost]
    [TypeFilter(typeof(CreateOrderActionFilter))]

    public async Task<IActionResult> BuyOrder(BuyOrderRequest buyOrderRequest)
    {
        BuyOrderResponse buyOrderResponse = await _stocksService.CreateBuyOrder(buyOrderRequest);

        return RedirectToAction(nameof(Orders));
    }


    [Route("[action]")]
    [HttpPost]
    [TypeFilter(typeof(CreateOrderActionFilter))]

    public async Task<IActionResult> SellOrder(SellOrderRequest sellOrderRequest)
    {
        

        SellOrderResponse sellOrderResponse = await _stocksService.CreateSellOrder(sellOrderRequest);

        return RedirectToAction(nameof(Orders));
    }


    [Route("[action]")]
    public async Task<IActionResult> Orders()
    {
        List<BuyOrderResponse> buyOrderResponses = await _stocksService.GetBuyOrders();
        List<SellOrderResponse> sellOrderResponses = await _stocksService.GetSellOrders();

        Orders orders = new Orders() { BuyOrders = buyOrderResponses, SellOrders = sellOrderResponses };

        ViewBag.TradingOptions = _tradingOptions;

        return View(orders);
    }


    [Route("OrdersPDF")]
    public async Task<IActionResult> OrdersPDF()
    {
        List<IOrderResponse> orders = new List<IOrderResponse>();
        orders.AddRange(await _stocksService.GetBuyOrders());
        orders.AddRange(await _stocksService.GetSellOrders());
        orders = orders.OrderByDescending(temp => temp.DateAndTimeOfOrder).ToList();

        ViewBag.TradingOptions = _tradingOptions;

        return new ViewAsPdf("OrdersPDF", orders, ViewData)
        {
            PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
        };
    }
}