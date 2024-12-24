using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using StockMarketSolution.Controllers;
using StockMarketSolution.Models;


namespace StockMarketSolution.Filters.ActionFilters;

public class CreateOrderActionFilter : IAsyncActionFilter
{
    public CreateOrderActionFilter()
    {
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {

        if (context.Controller is TradeController tradeController)
        {
            var orderRequest = context.ActionArguments["orderRequest"] as IOrderRequest;

            if (orderRequest != null)
            {

                orderRequest.DateAndTimeOfOrder = DateTime.Now;

                tradeController.ModelState.Clear();
                tradeController.TryValidateModel(orderRequest);


                if (!tradeController.ModelState.IsValid)
                {
                    tradeController.ViewBag.Errors = tradeController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();



                    StockTrade stockTrade = new StockTrade() { StockName = orderRequest.StockName, Quantity = orderRequest.Quantity, StockSymbol = orderRequest.StockSymbol };

                    context.Result = tradeController.View(nameof(TradeController.Index), stockTrade); 
                }

                else
                {
                    await next();
                }
            }
            else
            {
                await next(); 
            }
        }
        else
        {
            await next();
        }

    }
}