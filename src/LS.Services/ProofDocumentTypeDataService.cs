using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services {
    public class ProofDocumentTypeDataService : BaseDataService<ProofDocumentType, string> {
        public override BaseRepository<ProofDocumentType, string> GetDefaultRepository() {
            return new ProofDocumentTypeRepository();
        }

        // What's the best way to manage this value globally?
        private const string ProgramProofType = "program";
        private const string IdProofType = "id";
        private const string GenericIdProofTypesStateCode = "";

        public async Task<ServiceProcessingResult<List<ProofDocumentType>>> GetProofDocumentTypesByStateCodeAsync(string stateCode) {
            var processingResult = await GetAllWhereAsync(pdt => pdt.StateCode == stateCode && (pdt.DateStart <= DateTime.Now && pdt.DateEnd >= DateTime.Now));

            if (!processingResult.IsSuccessful || processingResult.Data == null) {
                if (processingResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving Proof Document Types for StateCode: {0}", stateCode);
                    //Logger.Fatal(logMessage);
                 ExceptionlessClient.Default.CreateLog(typeof(ProofDocumentTypeDataService).FullName,String.Format("A fatal error occurred while retrieving Proof Document Types for StateCode: {0}",stateCode),"Error").AddTags("Data Service Error").Submit();
                    }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_PROOF_DOCUMENT_TYPES_ERROR;
                return processingResult;
            }

            // If state has ID proofs setup, return set.  Otherwise continue below and get the generic ID proofs
            if (processingResult.Data.Any(pdt => pdt.ProofType == IdProofType)) {
                processingResult.Data = processingResult.Data.OrderBy(pdt => pdt.Name).ToList();
                return processingResult;
            }

            var idProofTypesResult = await GetGenericIdProofDocumentTypesAsync();
            if (!idProofTypesResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_PROOF_DOCUMENT_TYPES_ERROR;
                return idProofTypesResult;
            }

            processingResult.Data = processingResult.Data.Union(idProofTypesResult.Data).OrderBy(pdt => pdt.Name).ToList();

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<ProofDocumentType>>> GetProgramProofDocumentTypesByStateCodeAsync(string stateCode) {
            return await GetAllWhereAsync(pdt => pdt.StateCode == stateCode && pdt.ProofType == ProgramProofType);
        }
        public async Task<ServiceProcessingResult<List<ProofDocumentType>>> GetWhereAsync(string proofTypeID) {
            return await GetAllWhereAsync(pdt => pdt.Id == proofTypeID);
        }

        public async Task<ServiceProcessingResult<List<ProofDocumentType>>> GetGenericIdProofDocumentTypesAsync() {
            return
                await GetAllWhereAsync(pdt => pdt.StateCode == GenericIdProofTypesStateCode && pdt.ProofType == IdProofType);
        }
    }
}
