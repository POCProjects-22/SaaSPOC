using System.Linq.Expressions;

namespace POCRepository.Repository
{
    public interface IGenericRepository<T,TC> where T : class where TC : class
    {
        IEnumerable<T> GetAll(bool ignoreIsDeleted = false);
        T GetById(object id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(T obj);
        void Add(T obj);
        PT Max<PT>(string fieldName);
        List<T> Where(Expression<Func<T, bool>> expression);
        void BulkInsert(IEnumerable<T> entities);
       
    }
}
