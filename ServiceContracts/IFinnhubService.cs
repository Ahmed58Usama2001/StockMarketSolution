using System;
using System.Collections.Generic;

namespace ServiceContracts
{
 public interface IFinnhubService
 {
  Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol);


  Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol);
 }
}
