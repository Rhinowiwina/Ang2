using System.Collections.Generic;
using System.Linq;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System.Threading.Tasks;
using LS.Utilities;

namespace LS.Services {
    public class TpivProofDocumentTypeDataService : BaseDataService<TpivProofDocumentType, string> {
        private static readonly string TexasLifelineSystem = "TEXAS";
        private static readonly string NladLifelineSystem = "NLAD";
        private static readonly string CaliforniaLifelineSystem = "DAP";

        public override BaseRepository<TpivProofDocumentType, string> GetDefaultRepository() {
            return new TpivProofDocumentTypeRepository();
        }

        public async Task<ServiceProcessingResult<List<TpivProofDocumentType>>> GetByStateCode(string stateCode) {
            if (stateCode == StatesHelper.TexasStateCode) {
                return await GetAllWhereAsync(t => t.LifelineSystem == TexasLifelineSystem);
            }
            if (stateCode == StatesHelper.CaliforniaStateCode) {
                return await GetAllWhereAsync(t => t.LifelineSystem == CaliforniaLifelineSystem);
            }

            return await GetAllWhereAsync(t => t.LifelineSystem == NladLifelineSystem);
        }
        public async Task<ServiceProcessingResult<List<TpivProofDocumentType>>> GetAllWhereAsync(string documentTypeID) {
            return await GetAllWhereAsync(t => t.Id == documentTypeID);
        }
    }
}
