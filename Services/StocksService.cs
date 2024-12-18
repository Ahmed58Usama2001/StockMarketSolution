using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services;

public class StocksService : IStocksService
{
    private readonly ApplicationDbContext _dbContext;

    public StocksService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
    {
        if (buyOrderRequest == null)
            throw new ArgumentNullException(nameof(buyOrderRequest));

        ValidationHelper.ValidateModel(buyOrderRequest);

        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();

        buyOrder.BuyOrderId=Guid.NewGuid();

        await _dbContext.BuyOrders.AddAsync(buyOrder);
        await _dbContext.SaveChangesAsync();

        return buyOrder.ToBuyOrderResponse();
    }

    public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
    {
        if (sellOrderRequest == null)
            throw new ArgumentNullException(nameof(sellOrderRequest));

        ValidationHelper.ValidateModel(sellOrderRequest);

        SellOrder sellOrder = sellOrderRequest.ToSellOrder();

        sellOrder.SellOrderId = Guid.NewGuid();

        await _dbContext.SellOrders.AddAsync(sellOrder);
        await _dbContext.SaveChangesAsync();

        return sellOrder.ToSellOrderResponse();
    }

    public async Task<List<BuyOrderResponse>> GetBuyOrders()
    {
        List<BuyOrder> buyOrders = await _dbContext.BuyOrders.OrderByDescending(bo => bo.DateAndTimeOfOrder).ToListAsync();

        return buyOrders.Select(bo=>bo.ToBuyOrderResponse()).ToList();
    }

    public async Task<List<SellOrderResponse>> GetSellOrders()
    {
        List<SellOrder> sellOrders = await _dbContext.SellOrders.OrderByDescending(so => so.DateAndTimeOfOrder).ToListAsync();

        return sellOrders.Select(so => so.ToSellOrderResponse()).ToList();
    }
}
