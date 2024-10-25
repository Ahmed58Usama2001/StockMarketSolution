using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services;

public class StocksService : IStocksService
{
    private readonly List<BuyOrder> _buyOrders;
    private readonly List<SellOrder> _sellOrders;

    public StocksService()
    {
        _buyOrders = new List<BuyOrder>();
        _sellOrders = new List<SellOrder>();
    }

    public BuyOrderResponse CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
    {
        if (buyOrderRequest == null)
            throw new ArgumentNullException(nameof(buyOrderRequest));

        ValidationHelper.ValidateModel(buyOrderRequest);

        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();

        buyOrder.BuyOrderId=Guid.NewGuid();

        _buyOrders.Add(buyOrder);

        return buyOrder.ToBuyOrderResponse();
    }

    public SellOrderResponse CreateSellOrder(SellOrderRequest? sellOrderRequest)
    {
        if (sellOrderRequest == null)
            throw new ArgumentNullException(nameof(sellOrderRequest));

        ValidationHelper.ValidateModel(sellOrderRequest);

        SellOrder sellOrder = sellOrderRequest.ToSellOrder();

        sellOrder.SellOrderId = Guid.NewGuid();

        _sellOrders.Add(sellOrder);

        return sellOrder.ToSellOrderResponse();
    }

    public List<BuyOrderResponse> GetBuyOrders()
    {
        return _buyOrders.OrderByDescending(bo=>bo.DateAndTimeOfOrder)
            .Select(bo=>bo.ToBuyOrderResponse())
            .ToList();
    }

    public List<SellOrderResponse> GetSellOrders()
    {
        return _sellOrders.OrderByDescending(so => so.DateAndTimeOfOrder)
            .Select(so => so.ToSellOrderResponse())
            .ToList();
    }
}
