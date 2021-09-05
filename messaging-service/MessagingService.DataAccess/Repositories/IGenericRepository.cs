using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MessagingService.DataAccess.Repositories
{
    public interface IGenericRepository<TModel>
    {
        List<TModel> GetList();

        TModel GetById(string id);

        TModel Create(TModel model);

        void Update(TModel model);

        void Delete(TModel model);

        void Delete(string id);

        IEnumerable<TModel> Filter(Expression<Func<TModel, bool>> filter);

        TModel GetFirstOrDefault(Expression<Func<TModel, bool>> filter);

        TModel GetSingleOrDefault(Expression<Func<TModel, bool>> filter);

        long Count(Expression<Func<TModel, bool>> filter);

        bool DoesExist(Expression<Func<TModel, bool>> filter);
    }


}
