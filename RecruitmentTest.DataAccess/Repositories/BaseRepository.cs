using Microsoft.EntityFrameworkCore;
using RecruitmentTest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.DataAccess.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly AppDbContext db;
        public BaseRepository(AppDbContext db)
        {
            this.db = db;
        }
        public async Task<T> GetByIdAsync(int id) =>
    (await db.Set<T>().FindAsync(id))!;

        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes =null)
        {
            IQueryable<T> query = db.Set<T>();

            if (includes != null)
                for (int i = 0; i < includes.Length; i++)
                    query = query.Include(includes[i]).AsSplitQuery().AsNoTracking();

            return await query.FirstOrDefaultAsync(criteria);
        }

        public async Task<T> AddAsync(T entity)
        {
            await db.Set<T>().AddAsync(entity);
            return entity;
        }
        public void Delete(T entity)
        {
            db.Set<T>().Remove(entity);
        }
        public void DeleteRange(IEnumerable<T> entites)
        {
            db.Set<T>().RemoveRange(entites);
        }
        public void Update(T entity)
        {
            db.Update(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
    Expression<Func<T, bool>> filter = null!,
    Expression<Func<T, IQueryable<T>>> select = null!,
    Expression<Func<T, T>> selector = null!,
    Func<IQueryable<T>,
    IOrderedQueryable<T>> orderBy = null!,
    Expression<Func<T, bool>> includeFilter = null!,
    string includeProperties = null!,
    int? skip = null,
    int? take = null)
        {
            IQueryable<T> query = db.Set<T>().AsNoTracking();

            if (includeFilter is not null)
                query = query.Include(includeFilter);

            if (select != null)
                query = (IQueryable<T>)query.Select(select);
            if (filter != null)
                query = query.Where(filter);

            if (includeProperties != null)
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public int Count()
        {
            IQueryable<T> query = db.Set<T>().AsNoTracking();
            return query.Count();
        }

    }
}
