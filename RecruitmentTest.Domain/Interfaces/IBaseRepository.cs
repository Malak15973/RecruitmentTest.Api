using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null!, Expression<Func<T, IQueryable<T>>> select = null!,
 Expression<Func<T, T>> selector = null!,
 Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!, Expression<Func<T, bool>> includeFilter = null!,
 string includeProperties = null!,
 int? skip = null,
 int? take = null);

        Task<T> AddAsync(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entites);
        void Update(T entity);
        int Count();

    }
}
