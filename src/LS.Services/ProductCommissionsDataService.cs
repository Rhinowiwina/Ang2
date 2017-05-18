using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using LS.Utilities;
using System.Configuration;

namespace LS.Services
{
    public class ProductCommissionsDataService : BaseDataService<ProductCommissions, string>
    {
        public override BaseRepository<ProductCommissions, string> GetDefaultRepository()
        {
            return new ProductCommissionsRepository();
        }

        public async Task<ServiceProcessingResult<List<ProductCommissions>>> GetCommissionsBySalesTeam(string SalesTeamID)
        {
            var processingResult = new ServiceProcessingResult<List<ProductCommissions>>();

            var includes = new[] { "RecipientUser", "RecipientUser.roles.role" };

            var result = await GetAllWhereAsync(pc => pc.SalesTeamID == SalesTeamID && pc.IsDeleted == false, includes);

            if (!result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Problem retrieving product commissions.", "Problem retrieving product commissions.", false, false);
            }
            processingResult.Data = result.Data;

            processingResult.IsSuccessful = true;
            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<ProductCommissions>>> GetCommissionsByUserID(string UserID)
        {
            var processingResult = new ServiceProcessingResult<List<ProductCommissions>>();

            var includes = new[] { "RecipientUser", "RecipientUser.roles.role" };

            var result = await GetAllWhereAsync(pc => pc.RecipientUserId == UserID && pc.IsDeleted == false, includes);

            if (!result.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Problem retrieving product commissions.", "Problem retrieving product commissions.", false, false);
            }
            processingResult.Data = result.Data;

            processingResult.IsSuccessful = true;
            return processingResult;
        }

        public async Task<ServiceProcessingResult<bool>> ValidateCommissionAvailable(string ID, string ProductType, string RecipientType, string SalesTeamID, string RecipientUserID)
        {
            var processingResult = new ServiceProcessingResult<bool>();
            
            var result = new ServiceProcessingResult<List<ProductCommissions>>();

            if (RecipientType == "User") {
                result = await GetAllWhereAsync(pc => pc.Id != ID && pc.ProductType == ProductType && pc.RecipientUserId == RecipientUserID && pc.RecipientType == RecipientType && pc.IsDeleted == false);
            } else {
                result = await GetAllWhereAsync(pc => pc.Id != ID && pc.ProductType == ProductType && pc.SalesTeamID == SalesTeamID && pc.RecipientType == RecipientType && pc.IsDeleted == false);
            }
            
            if (!result.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Problem retrieving product commissions availability.", "Problem retrieving product commissions availability.", false, false);
            }

            processingResult.IsSuccessful = true;

            if (result.Data.Count > 0){
                processingResult.Data = false;
            } else {
                processingResult.Data = true;
            }

            return processingResult;
        }


        public async Task<ServiceProcessingResult<ProductCommissions>> GetCommissionForUpdate(string CommissionID)
        {
            var processingResult = new ServiceProcessingResult<ProductCommissions>();

            var result = await GetWhereAsync(pc => pc.Id == CommissionID && pc.IsDeleted == false);

            if (!result.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Problem retrieving product commissions for update.", "Problem retrieving product commissions for update.", false, false);
            }
            processingResult.Data = result.Data;

            processingResult.IsSuccessful = true;
            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<CommissionLog>>> LogProductCommission(string ProductType, string SalesTeamID, string OrderID, string UserID)
        {
            var processingResult = new ServiceProcessingResult<List<CommissionLog>>() { IsSuccessful = true };

            var productCommissionList = await GetProductCommissionsByProductTypeSalesTeam(ProductType, SalesTeamID);
            if (!productCommissionList.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = productCommissionList.Error;
                return processingResult;
            }

            var productCommissionListUser = await GetProductCommissionsByProductTypeUserID(ProductType, UserID);
            if (!productCommissionListUser.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = productCommissionListUser.Error;
                return processingResult;
            }

            productCommissionList.Data.AddRange(productCommissionListUser.Data);
            var commissionLogDataService = new CommissionLogDataService();
            processingResult = await commissionLogDataService.InsertCommissionLog(productCommissionList.Data, OrderID);

            return processingResult;
        }


        public async Task<ServiceProcessingResult<List<ProductCommissions>>> GetProductCommissionsByProductTypeSalesTeam(string ProductType, string SalesTeamID)
        {
            var processingResult = new ServiceProcessingResult<List<ProductCommissions>>();

            var result = await GetAllWhereAsync(pc => pc.ProductType == ProductType && pc.SalesTeamID == SalesTeamID && pc.IsDeleted == false);

            if (!result.IsSuccessful)
            {
                processingResult.Error = new ProcessingError("Problem retrieving product commissions for logging.", "Problem retrieving product commissions for logging.", false, false);
            }
            processingResult.Data = result.Data;

            processingResult.IsSuccessful = true;
            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<ProductCommissions>>> GetProductCommissionsByProductTypeUserID(string ProductType, string UserID)
        {
            var processingResult = new ServiceProcessingResult<List<ProductCommissions>>();

            var result = await GetAllWhereAsync(pc => pc.ProductType==ProductType && pc.RecipientUserId==UserID && pc.RecipientType=="User" && pc.IsDeleted==false);

            if (!result.IsSuccessful)
            {
                processingResult.Error = new ProcessingError("Problem retrieving user product commissions for logging.", "Problem retrieving user product commissions for logging.", false, false);
            }
            processingResult.Data = result.Data;

            processingResult.IsSuccessful = true;
            return processingResult;
        }

    }
}
