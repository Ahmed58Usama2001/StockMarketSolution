using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services;

public class StocksService : IStocksService
{
    private readonly IStockRepository _stocksRepository;


  
    public StocksService(IStockRepository stocksRepository)
    {
        _stocksRepository = stocksRepository;
    }


    public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
    {
        if (buyOrderRequest == null)
            throw new ArgumentNullException(nameof(buyOrderRequest));

        ValidationHelper.ModelValidation(buyOrderRequest);

        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();

        buyOrder.BuyOrderId = Guid.NewGuid();

        BuyOrder buyOrderFromRepo = await _stocksRepository.CreateBuyOrder(buyOrder);

        return buyOrder.ToBuyOrderResponse();
    }


    public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
    {
        if (sellOrderRequest == null)
            throw new ArgumentNullException(nameof(sellOrderRequest));

        ValidationHelper.ModelValidation(sellOrderRequest);

        SellOrder sellOrder = sellOrderRequest.ToSellOrder();

        sellOrder.SellOrderId = Guid.NewGuid();

        SellOrder SellOrderFromRepo = await _stocksRepository.CreateSellOrder(sellOrder);

        return sellOrder.ToSellOrderResponse();
    }


    public async Task<List<BuyOrderResponse>> GetBuyOrders()
    {
        List<BuyOrder> buyOrders = await _stocksRepository.GetBuyOrders();

        return buyOrders.Select(temp => temp.ToBuyOrderResponse()).ToList();
    }


    public async Task<List<SellOrderResponse>> GetSellOrders()
    {
        List<SellOrder> sellOrders = await _stocksRepository.GetSellOrders();

        return sellOrders.Select(temp => temp.ToSellOrderResponse()).ToList();
    }
}
