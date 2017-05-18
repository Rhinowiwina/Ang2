using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Logging;
using LS.Core;
using LS.Core.Interfaces;
using LS.Repositories;
using LS.Utilities;
using Numero3.EntityFramework.Implementation;
using Numero3.EntityFramework.Interfaces;
using Exceptionless;
using Exceptionless.Models;

namespace LS.Services
{
    public abstract class BaseDataService<TEntity, TPK> : IDataService<TEntity, TPK>
        where TEntity : class, IEntity<TPK>
    {
        private static readonly string GENERIC_GET_ALL_LOG_MESSAGE = "Base data service GetAll failed for Entity: {0}";
        private static readonly string GENERIC_GET_ALL_WHERE_LOG_MESSAGE = "Base data service GetAllWhere failed for Entity: {0}";
        private static readonly string GENERIC_GET_ALL_WHERE_ORDERED_LOG_MESSAGE = "Base data service GetAllWhereOrdererd failed for Entity: {0}";
        private static readonly string GENERIC_PAGED_GET_ALL_WHERE_ORDERED_LOG_MESSAGE = "Base data service PagedGetAllWhereOrdered failed for Entity: {0}";
        private static readonly string GENERIC_GET_LOG_MESSAGE = "Base data service Get failed for Entity: {0}";
        private static readonly string GENERIC_GET_WHERE_LOG_MESSAGE = "Base data service GetWhere failed for Entity: {0}";
        private static readonly string GENERIC_ADD_LOG_MESSAGE = "Base data service Add failed for Entity: {0}";
        private static readonly string GENERIC_UPDATE_LOG_MESSAGE = "Base data service Update failed for Entity: {0}";
        private static readonly string GENERIC_DELETE_LOG_MESSAGE = "Base data service Delete failed for Entity: {0}";

        protected ILog Logger { get; set; }

        protected BaseRepository<TEntity, TPK> BaseRepository { get; set; }

        protected readonly IDbContextScopeFactory DbContextScopeFactory;

        protected BaseDataService(BaseRepository<TEntity, TPK> repository,
            IDbContextScopeFactory dbContextScopeFactory)
        {
            BaseRepository = repository;
            DbContextScopeFactory = dbContextScopeFactory;
            Logger = LoggerFactory.GetLogger(GetType());
        }

        protected BaseDataService()
        {
            DbContextScopeFactory = new DbContextScopeFactory();
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public abstract BaseRepository<TEntity, TPK> GetDefaultRepository();

        public BaseRepository<TEntity, TPK> GetRepository()
        {
            return BaseRepository ?? GetDefaultRepository();
        }

        public virtual ServiceProcessingResult<List<TEntity>> GetAll(params string[] includes)
        {
            var processingResult = new ServiceProcessingResult<List<TEntity>>();

            using (var dbContextScope = DbContextScopeFactory.CreateReadOnly())
            {
                try
                {
                    processingResult = GetRepository().GetAll(GetCombinedIncludes(includes))
                        .ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_LOG_MESSAGE));
                    }
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_LOG_MESSAGE), ex);  
                }
            }

            return processingResult;
        }

        public async virtual Task<ServiceProcessingResult<List<TEntity>>> GetAllAsync(params string[] includes)
        {
            var processingResult = new ServiceProcessingResult<List<TEntity>>();

            using (var dbContextScope = DbContextScopeFactory.CreateReadOnly())
            {
                try
                {
                    var dataAccessResult = await GetRepository().GetAllAsync(GetCombinedIncludes(includes));

                    processingResult = dataAccessResult.ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_LOG_MESSAGE));
                    }

                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_LOG_MESSAGE), ex);
                }
            }

            return processingResult;
        } 

        protected virtual ServiceProcessingResult<List<TEntity>> GetAllWhere(Expression<Func<TEntity, bool>> predicate,
            params string[] includes)
        {
            var processingResult = new ServiceProcessingResult<List<TEntity>>();

            using (var dbContextScope = DbContextScopeFactory.CreateReadOnly())
            {
                try
                {
                    processingResult = GetRepository().GetAllWhere(predicate, GetCombinedIncludes(includes))
                        .ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_WHERE_LOG_MESSAGE));
                    }
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_WHERE_LOG_MESSAGE), ex);                    
                }
            }

            return processingResult;
        }

        protected async virtual Task<ServiceProcessingResult<List<TEntity>>> GetAllWhereAsync(
            Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var processingResult = new ServiceProcessingResult<List<TEntity>>();

            using (var dbContextScope = DbContextScopeFactory.CreateReadOnly())
            {
                try
                {
                    var dataAccessResult = await GetRepository().GetAllWhereAsync(predicate, GetCombinedIncludes(includes));

                    processingResult =
                        dataAccessResult.ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!dataAccessResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_WHERE_LOG_MESSAGE));
                    }
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_WHERE_LOG_MESSAGE), ex);
                }
            }

            return processingResult;
        }
  

        public virtual ServiceProcessingResult<TEntity> Get(TPK id)
        {
            var processingResult = new ServiceProcessingResult<TEntity>();

            using (var dbContextScope = DbContextScopeFactory.CreateReadOnly())
            {
                try
                {
                    processingResult = GetRepository().Get(id)
                        .ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_LOG_MESSAGE));
                    }
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_LOG_MESSAGE), ex);                    
                }
            }

            return processingResult;
        }

        protected virtual ServiceProcessingResult<TEntity> GetWhere(Expression<Func<TEntity, bool>> predicate,
            params string[] includes)
        {
            var processingResult = new ServiceProcessingResult<TEntity>();

            using (var dbContextScope = DbContextScopeFactory.CreateReadOnly())
            {
                try
                {
                    processingResult = GetRepository().GetWhere(predicate, GetCombinedIncludes(includes))
                        .ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);
                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_WHERE_LOG_MESSAGE));
                    }
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_WHERE_LOG_MESSAGE), ex);
                }
            }

            return processingResult;
        }

        protected async virtual Task<ServiceProcessingResult<TEntity>> GetWhereAsync(
            Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var processingResult = new ServiceProcessingResult<TEntity>();

            using (var dbContextScope = DbContextScopeFactory.CreateReadOnly())
            {
                try
                {
                    var dataAccessResult = await GetRepository().GetWhereAsync(predicate, GetCombinedIncludes(includes));

                    processingResult =
                        dataAccessResult.ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_WHERE_LOG_MESSAGE));
                }
            }

            return processingResult;
        }
        protected async virtual Task<ServiceProcessingResult<TEntity>> GetWhereAsyncWithDeleted(
           Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var processingResult = new ServiceProcessingResult<TEntity>();

            using (var dbContextScope = DbContextScopeFactory.CreateReadOnly())
            {
                try
                {
                    var dataAccessResult = await GetRepository().GetWhereAsyncWithDeleted(predicate, GetCombinedIncludes(includes));

                    processingResult =
                        dataAccessResult.ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_WHERE_LOG_MESSAGE));
                }
            }

            return processingResult;
        }

        public virtual ServiceProcessingResult<TEntity> Add(TEntity entity)
        {
            var processingResult = new ServiceProcessingResult<TEntity>();

            using (var dbContextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    processingResult = GetRepository().Add(entity)
                        .ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_ADD_LOG_MESSAGE));
                        return processingResult;
                    }

                    dbContextScope.SaveChanges();
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_ADD_LOG_MESSAGE), ex);
                }
            }

            return processingResult;
        }

        public async virtual Task<ServiceProcessingResult<TEntity>> AddAsync(TEntity entity)
        {
            var processingResult = new ServiceProcessingResult<TEntity>();

            using (var dbContextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    processingResult = GetRepository().Add(entity)
                        .ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_ADD_LOG_MESSAGE));
                        return processingResult;
                    }

                    await dbContextScope.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_ADD_LOG_MESSAGE), ex);
                }
            }

            return processingResult;
        }

        public virtual ServiceProcessingResult<TEntity> Update(TEntity entity)
        {
            var processingResult = new ServiceProcessingResult<TEntity>();

            using (var dbContextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    processingResult = GetRepository().Update(entity)
                        .ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);
                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_UPDATE_LOG_MESSAGE));
                        return processingResult;
                    }

                    dbContextScope.SaveChanges();
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_UPDATE_LOG_MESSAGE), ex);
                }
            }

            return processingResult;
        }

        public async virtual Task<ServiceProcessingResult<TEntity>> UpdateAsync(TEntity entity)
        {
            var processingResult = new ServiceProcessingResult<TEntity>();

            using (var dbContextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    processingResult = GetRepository().Update(entity)
                        .ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);
                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_UPDATE_LOG_MESSAGE));
                        return processingResult;
                    }

                    await dbContextScope.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_UPDATE_LOG_MESSAGE), ex);
                }
            }

            return processingResult;
        }

        public virtual ServiceProcessingResult Delete(TPK id)
        {
            var processingResult = new ServiceProcessingResult();

            using (var dbContextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    processingResult = GetRepository().Delete(id)
                        .ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);
                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_DELETE_LOG_MESSAGE));
                        return processingResult;
                    }

                    dbContextScope.SaveChanges();
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_DELETE_LOG_MESSAGE), ex);
                }
            }

            return processingResult;
        }

        public async virtual Task<ServiceProcessingResult> DeleteAsync(TPK id)
        {
            var processingResult = new ServiceProcessingResult();

            using (var dbContextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    processingResult = GetRepository().Delete(id)
                        .ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);
                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(GetFormattedGenericLogMessage(GENERIC_DELETE_LOG_MESSAGE));
                    }

                    await dbContextScope.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error(GetFormattedGenericLogMessage(GENERIC_DELETE_LOG_MESSAGE), ex);
                }
            }

            return processingResult;
        }

        protected virtual string[] GetDefaultIncludes()
        {
            return new string[] { };
        }

        private string[] GetCombinedIncludes(IEnumerable<string> includes)
        {
            var completeIncludes = GetDefaultIncludes().Union(includes).ToArray();
            return completeIncludes;
        }

        protected string GetFormattedGenericLogMessage(string logMessage)
        {
            return String.Format(logMessage, typeof(TEntity));
        }
    }
}
