using System.Threading.Tasks;
using System;
using LS.Core;
using System.Linq;
using LS.Core.Interfaces;
using LS.Domain;
using LS.Repositories;
using System.Collections.Generic;

namespace LS.Services
{
    public class TaxesDataService : BaseDataService<OrdersTaxes, string>
    {
        public override BaseRepository<OrdersTaxes, string> GetDefaultRepository()
        {
            return new OrdersTaxesRepository();
        }

        public async Task<ServiceProcessingResult> AddTaxItemsAsync(IEnterOrderResult OrderData, string OrderID)
        {
            var result = new ServiceProcessingResult<OrdersTaxes>();
            result.IsSuccessful = true;

            var TaxItems = OrderData.TaxItems;
            var AdditionalCharges = OrderData.AdditionalCharges;

            try
            {
                if (OrderData.TaxesApply) {
                    foreach (var taxitem in TaxItems)
                    {
                        var taxModel = new OrdersTaxes
                        {
                            Type = "Tax",
                            OrderID = OrderID,
                            OrderType = "account",
                            Amount = taxitem.TaxItemAmount,
                            Description = taxitem.TaxItemDescription,
                        };
                        var addResult = await AddAsync(taxModel);

                        if (!addResult.IsSuccessful)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("Error inserting taxes.", "Error inserting taxes.", true, false);
                        }
                    }
                }

                if (OrderData.AdditionalChargesApply) {
                    foreach (var additionalchargeitem in AdditionalCharges)
                    {
                        var additionalChargeModel = new OrdersTaxes
                        {
                            Type = "Charge",
                            OrderID = OrderID,
                            OrderType = "account",
                            Amount = additionalchargeitem.AdditionalChargeAmount,
                            Description = additionalchargeitem.AdditionalChargeDescription
                        };
                        var addResult = await AddAsync(additionalChargeModel);

                        if (!addResult.IsSuccessful)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("Error inserting additional charges.", "Error inserting additional charges.", true, false);
                        }
                    }
                }
            }
            catch (Exception ex) {

            }

            return result;
        }
    }
}
