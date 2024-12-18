using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace Tests;


public class StocksServiceTest
{
    private readonly IStocksService _stocksService;

    public StocksServiceTest()
    {
        _stocksService = new StocksService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options));
    }



    #region CreateBuyOrder


    [Fact]
    public async Task CreateBuyOrder_NullBuyOrder_ToBeArgumentNullException()
    {
        BuyOrderRequest? buyOrderRequest = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _stocksService.CreateBuyOrder(buyOrderRequest);
        });
    }



    [Theory] 
    [InlineData(0)] 
    public async Task CreateBuyOrder_QuantityIsLessThanMinimum_ToBeArgumentException(uint buyOrderQuantity)
    {
        BuyOrderRequest? buyOrderRequest = new BuyOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = 1, Quantity = buyOrderQuantity };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateBuyOrder(buyOrderRequest);
        });
    }


    [Theory] 
    [InlineData(100001)] 
    public async Task CreateBuyOrder_QuantityIsGreaterThanMaximum_ToBeArgumentException(uint buyOrderQuantity)
    {

        BuyOrderRequest? buyOrderRequest = new BuyOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = 1, Quantity = buyOrderQuantity };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateBuyOrder(buyOrderRequest);
        });
    }


    [Theory] 
    [InlineData(0)]
    public async Task CreateBuyOrder_PriceIsLessThanMinimum_ToBeArgumentException(uint buyOrderPrice)
    {
        BuyOrderRequest? buyOrderRequest = new BuyOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = buyOrderPrice, Quantity = 1 };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateBuyOrder(buyOrderRequest);
        });
    }


    [Theory] 
    [InlineData(10001)] 
    public async Task CreateBuyOrder_PriceIsGreaterThanMaximum_ToBeArgumentException(uint buyOrderPrice)
    {
        BuyOrderRequest? buyOrderRequest = new BuyOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = buyOrderPrice, Quantity = 1 };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateBuyOrder(buyOrderRequest);
        });
    }


    [Fact]
    public async Task CreateBuyOrder_StockSymbolIsNull_ToBeArgumentException()
    {
        BuyOrderRequest? buyOrderRequest = new BuyOrderRequest() { StockSymbol = null, StockName = "Microsoft", Price = 1, Quantity = 1 };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateBuyOrder(buyOrderRequest);
        });
    }


    [Fact]
    public async Task CreateBuyOrder_DateOfOrderIsLessThanYear2000_ToBeArgumentException()
    {
        BuyOrderRequest? buyOrderRequest = new BuyOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", DateAndTimeOfOrder = Convert.ToDateTime("1999-12-31"), Price = 1, Quantity = 1 };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateBuyOrder(buyOrderRequest);
        });
    }


    [Fact]
    public async Task CreateBuyOrder_ValidData_ToBeSuccessful()
    {
        BuyOrderRequest? buyOrderRequest = new BuyOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", DateAndTimeOfOrder = Convert.ToDateTime("2024-12-31"), Price = 1, Quantity = 1 };

        
        BuyOrderResponse buyOrderResponseFromCreate = await _stocksService.CreateBuyOrder(buyOrderRequest);

        Assert.NotEqual(Guid.Empty, buyOrderResponseFromCreate.BuyOrderID);
    }


    #endregion




    #region CreateSellOrder


    [Fact]
    public async Task CreateSellOrder_NullSellOrder_ToBeArgumentNullException()
    {
        SellOrderRequest? sellOrderRequest = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _stocksService.CreateSellOrder(sellOrderRequest);
        });
    }



    [Theory] 
    [InlineData(0)] 
    public async Task CreateSellOrder_QuantityIsLessThanMinimum_ToBeArgumentException(uint sellOrderQuantity)
    {
        SellOrderRequest? sellOrderRequest = new SellOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = 1, Quantity = sellOrderQuantity };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateSellOrder(sellOrderRequest);
        });
    }


    [Theory]
    [InlineData(100001)] 
    public async Task CreateSellOrder_QuantityIsGreaterThanMaximum_ToBeArgumentException(uint sellOrderQuantity)
    {
        SellOrderRequest? sellOrderRequest = new SellOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = 1, Quantity = sellOrderQuantity };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateSellOrder(sellOrderRequest);
        });
    }


    [Theory] 
    [InlineData(0)] 
    public async Task CreateSellOrder_PriceIsLessThanMinimum_ToBeArgumentException(uint sellOrderPrice)
    {
        SellOrderRequest? sellOrderRequest = new SellOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = sellOrderPrice, Quantity = 1 };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateSellOrder(sellOrderRequest);
        });
    }


    [Theory] 
    [InlineData(10001)]
    public async Task CreateSellOrder_PriceIsGreaterThanMaximum_ToBeArgumentException(uint sellOrderPrice)
    {
        SellOrderRequest? sellOrderRequest = new SellOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = sellOrderPrice, Quantity = 1 };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateSellOrder(sellOrderRequest);
        });
    }


    [Fact]
    public async Task CreateSellOrder_StockSymbolIsNull_ToBeArgumentException()
    {
        SellOrderRequest? sellOrderRequest = new SellOrderRequest() { StockSymbol = null, Price = 1, Quantity = 1 };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateSellOrder(sellOrderRequest);
        });
    }


    [Fact]
    public async Task CreateSellOrder_DateOfOrderIsLessThanYear2000_ToBeArgumentException()
    {
        SellOrderRequest? sellOrderRequest = new SellOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", DateAndTimeOfOrder = Convert.ToDateTime("1999-12-31"), Price = 1, Quantity = 1 };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _stocksService.CreateSellOrder(sellOrderRequest);
        });
    }


    [Fact]
    public async Task CreateSellOrder_ValidData_ToBeSuccessful()
    {
        SellOrderRequest? sellOrderRequest = new SellOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", DateAndTimeOfOrder = Convert.ToDateTime("2024-12-31"), Price = 1, Quantity = 1 };

        SellOrderResponse sellOrderResponseFromCreate = await _stocksService.CreateSellOrder(sellOrderRequest);

        Assert.NotEqual(Guid.Empty, sellOrderResponseFromCreate.SellOrderID);
    }


    #endregion




    #region GetBuyOrders

    [Fact]
    public async Task GetAllBuyOrders_DefaultList_ToBeEmpty()
    {
        List<BuyOrderResponse> buyOrdersFromGet = await _stocksService.GetBuyOrders();

        Assert.Empty(buyOrdersFromGet);
    }


    [Fact]
    public async Task GetAllBuyOrders_WithFewBuyOrders_ToBeSuccessful()
    {

        BuyOrderRequest buyOrder_request_1 = new BuyOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = 1, Quantity = 1, DateAndTimeOfOrder = DateTime.Parse("2023-01-01 9:00") };

        BuyOrderRequest buyOrder_request_2 = new BuyOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = 1, Quantity = 1, DateAndTimeOfOrder = DateTime.Parse("2023-01-01 9:00") };

        List<BuyOrderRequest> buyOrder_requests = new List<BuyOrderRequest>() { buyOrder_request_1, buyOrder_request_2 };

        List<BuyOrderResponse> buyOrder_response_list_from_add = new List<BuyOrderResponse>();

        foreach (BuyOrderRequest buyOrder_request in buyOrder_requests)
        {
            BuyOrderResponse buyOrder_response = await _stocksService.CreateBuyOrder(buyOrder_request);
            buyOrder_response_list_from_add.Add(buyOrder_response);
        }

        List<BuyOrderResponse> buyOrders_list_from_get = await _stocksService.GetBuyOrders();


        foreach (BuyOrderResponse buyOrder_response_from_add in buyOrder_response_list_from_add)
        {
            Assert.Contains(buyOrder_response_from_add, buyOrders_list_from_get);
        }
    }

    #endregion




    #region GetSellOrders

    [Fact]
    public async Task GetAllSellOrders_DefaultList_ToBeEmpty()
    {
        List<SellOrderResponse> sellOrdersFromGet = await _stocksService.GetSellOrders();

        Assert.Empty(sellOrdersFromGet);
    }


    [Fact]
    public async Task GetAllSellOrders_WithFewSellOrders_ToBeSuccessful()
    {

        SellOrderRequest sellOrder_request_1 = new SellOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = 1, Quantity = 1, DateAndTimeOfOrder = DateTime.Parse("2023-01-01 9:00") };

        SellOrderRequest sellOrder_request_2 = new SellOrderRequest() { StockSymbol = "MSFT", StockName = "Microsoft", Price = 1, Quantity = 1, DateAndTimeOfOrder = DateTime.Parse("2023-01-01 9:00") };

        List<SellOrderRequest> sellOrder_requests = new List<SellOrderRequest>() { sellOrder_request_1, sellOrder_request_2 };

        List<SellOrderResponse> sellOrder_response_list_from_add = new List<SellOrderResponse>();

        foreach (SellOrderRequest sellOrder_request in sellOrder_requests)
        {
            SellOrderResponse sellOrder_response = await _stocksService.CreateSellOrder(sellOrder_request);
            sellOrder_response_list_from_add.Add(sellOrder_response);
        }

        List<SellOrderResponse> sellOrders_list_from_get = await _stocksService.GetSellOrders();


        foreach (SellOrderResponse sellOrder_response_from_add in sellOrder_response_list_from_add)
        {
            Assert.Contains(sellOrder_response_from_add, sellOrders_list_from_get);
        }
    }

    #endregion

}