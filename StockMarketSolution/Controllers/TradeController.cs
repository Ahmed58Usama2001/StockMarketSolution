﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using StockMarketSolution.Models;

namespace StockMarketSolution.Controllers;

[Route("[controller]")]
public class TradeController : Controller
{
    private readonly TradingOptions _tradingOptions;
    private readonly IStocksService _stocksService;
    private readonly IFinnhubService _finnhubService;
    private readonly IConfiguration _configuration;


    public TradeController(IOptions<TradingOptions> tradingOptions, IStocksService stocksService, IFinnhubService finnhubService, IConfiguration configuration)
    {
        _tradingOptions = tradingOptions.Value;
        _stocksService = stocksService;
        _finnhubService = finnhubService;
        _configuration = configuration;
    }


    [Route("[action]/{stockSymbol}")]
    [Route("~/[controller]/{stockSymbol}")]
    public async Task<IActionResult> Index(string stockSymbol)
    {
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
    public async Task<IActionResult> BuyOrder(BuyOrderRequest buyOrderRequest)
    {
        buyOrderRequest.DateAndTimeOfOrder = DateTime.Now;

        ModelState.Clear();
        TryValidateModel(buyOrderRequest);


        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            StockTrade stockTrade = new StockTrade() { StockName = buyOrderRequest.StockName, Quantity = buyOrderRequest.Quantity, StockSymbol = buyOrderRequest.StockSymbol };
            return View("Index", stockTrade);
        }

        BuyOrderResponse buyOrderResponse = await _stocksService.CreateBuyOrder(buyOrderRequest);

        return RedirectToAction(nameof(Orders));
    }


    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> SellOrder(SellOrderRequest sellOrderRequest)
    {
        sellOrderRequest.DateAndTimeOfOrder = DateTime.Now;

        ModelState.Clear();
        TryValidateModel(sellOrderRequest);

        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            StockTrade stockTrade = new StockTrade() { StockName = sellOrderRequest.StockName, Quantity = sellOrderRequest.Quantity, StockSymbol = sellOrderRequest.StockSymbol };
            return View("Index", stockTrade);
        }

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