using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POCRepository.Extensions;
using System.Linq.Expressions;

namespace POCRepository.Repository
{
    public class GenericRepository<T, TC> : IGenericRepository<T, TC>, IDisposable where T : class where TC : IdentityDbContext ,new()
    {

        private readonly IUnitOfWork<TC> unitOfWork;
        private DbSet<T> _entities;
        private string _errorMessage = string.Empty;
        private bool _isDisposed;
        public GenericRepository(IUnitOfWork<TC> unitOfWork)
        : this(unitOfWork.Context)
        {
            this.unitOfWork = unitOfWork;
        }
        public GenericRepository(TC context)
        {
            _isDisposed = false;
            Context = context;
        }
        public TC Context { get; set; }
        public virtual IQueryable<T> Table
        {
            get { return Entities; }
        }
        protected virtual DbSet<T> Entities
        {
            get { return _entities ?? (_entities = Context.Set<T>()); }
        }
        public void Dispose()
        {
            if (Context != null)
                Context.Dispose();
            _isDisposed = true;
        }


        public virtual IEnumerable<T> GetAll(bool ignoreIsDeleted = false)
        {
            List<T> list;
            list = Entities.ToList();
            return list;

            if (ignoreIsDeleted)
            {
                list = Entities.ToList();
            }

            var props = typeof(T).GetProperties();
            //if T doesn't inherit the base class, so check isdeleted column exist or not
            if (props.Any(p => p.Name == "IsDelete"))
            {
                // where IsDeleted=0
                var expIsDeleted = RepositoryExtensions.EqualExpression<T, bool>("IsDelete", false);
                var list1 = Entities.Where(expIsDeleted);
                //order  by Index desc
                var expIndexOrderByDescending = RepositoryExtensions.PropertyExpression<T, DateTime>("CreatedTime");
                //now execute and load into memory
                list = list1.OrderByDescending(expIndexOrderByDescending).ToList();
            }
            else
            {
                list = Entities.ToList();

                //if (!ignoreIsDeleted)
                //    list = list.Where(x => ((bool?)x.GetType().GetProperty("IsDeleted").GetValue(x)) != true).ToList();
            }

            return list;
            //
            //actual code
            //return Entities.ToList();
        }

        public void RunSql(string sql)
        {
            try
            {
                Context.Database.ExecuteSqlRaw(sql);
                //Context.Database
            }
            catch (Exception ex)
            {

                
            }
        }

        public PT Max<PT>(string fieldName)
        {
            var exp = RepositoryExtensions.PropertyExpression<T, PT>("Index");
            var max = (PT)Entities.Max(exp);
            return max;

        }

        public virtual T GetById(object id)
        {
            var obj = Entities.Find(id);
            return obj;
        }


        /// <summary>
        /// Insert or update entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Add(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                var pkvalue = entity.GetValueObjectID();

                T oldEntity = null;

                if (pkvalue != new Guid())
                    oldEntity = GetById(pkvalue);

                if (oldEntity == null)//add new 
                {
                    Insert(entity);
                }
                else//update
                {
                    var IsDeleted = entity.GetValue<T, bool>("IsDeleted");
                    if (IsDeleted)//update as delete> mark isdeleted=true, then not need to copy all props
                    {
                        oldEntity.SetValue("IsDeleted", IsDeleted);
                    }
                    else// update
                    {

                        //var pasword = "";
                        //if (entity.GetType().Name.Contains(typeof(DICEUser).Name))
                        //{   //jam
                        //    //if diceuser object , then keep old password because user update operation will not change pw.// may need futute work
                        //    pasword = entity.GetValue<T,string>("Password");
                        //    if (string.IsNullOrEmpty(pasword))
                        //    {
                        //        pasword = oldEntity.GetValue<T, string>("Password");
                        //    }
                        //}

                        //no need to assign value into terget object manually.
                        entity.CopyPropertiesTo(oldEntity);

                        ////jam reason , see above jam
                        //if (!string.IsNullOrEmpty(pasword))
                        //{
                        //    oldEntity.SetValue("Password", pasword);
                        //}
                    }
                    Update(oldEntity);
                }
                //  var ctpe = (T) Convert.ChangeType(item, typeof (T));
                //Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                if (Context == null || _isDisposed)
                    Context = (TC)Convert.ChangeType(Activator.CreateInstance(typeof(TC)), typeof(TC));
            }
            catch (Exception dbEx)
            {
                throw;// new Exception(dbEx.Message, dbEx);
            }
        }

        public virtual List<T> Where(Expression<Func<T, bool>> expression)
        {
            return Entities.Where(expression).ToList();
        } 
        public virtual T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return Entities.Where(expression).FirstOrDefault();
        }
        //public List<T> Where(Expression<Func<T, bool>> expression)
        //{
        //    return Context.Set<T>().Where(expression).ToList();
        //}
        public virtual void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");


                //AddDICECRUDLog(entity,DBOperationEnum.CREATE);
                //entity.AddCreatedInfo(this);
                Entities.Add(entity);
                if (Context == null || _isDisposed)
                    Context = new TC();
                //Context.SaveChanges(); commented out call to SaveChanges as Context save changes will be 
                //called with Unit of work
            }
            catch (Exception)
            {
                throw;
            }
        }
        public virtual void BulkInsert(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                {
                    throw new ArgumentNullException("entities");
                }
                //AddDICECRUDLog(entities.FirstOrDefault(), DBOperationEnum.BulkInsert);

                //Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Set<T>().AddRange(entities);
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public virtual void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                if (Context == null || _isDisposed)
                    Context = new TC();
                //if ((entity as BaseModel).IsDeleted)
                //    AddDICECRUDLog(entity, DBOperationEnum.DELETE);
                //else
                //    AddDICECRUDLog(entity, DBOperationEnum.UPDATE);

                SetEntryModified(entity);
                //Context.SaveChanges(); commented out call to SaveChanges as Context save changes will be called with Unit of work
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Obsolete("Never delete any data from application. Update row marking IsDeleted=true.")]
        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                if (Context == null || _isDisposed)
                    Context = new TC();

                Entities.Remove(entity);
                //Context.SaveChanges(); commented out call to SaveChanges as Context save changes will be called with Unit of work
            }

            catch (Exception)
            {
                throw;
            }
        }
        public virtual void SetEntryModified(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

    }
    //public static class EfExtensions
    //{
    //    //sql query statement
    //    public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
    //    {
    //        var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
    //        var relationalCommandCache = Private(enumerator, "_relationalCommandCache");
    //        var selectExpression = Private<SelectExpression>(relationalCommandCache, "_selectExpression");
    //        var factory = Private<IQuerySqlGeneratorFactory>(relationalCommandCache, "_querySqlGeneratorFactory");

    //        var sqlGenerator = factory.Create();
    //        var command = sqlGenerator.GetCommand(selectExpression);

    //        string sql = command.CommandText;
    //        return sql;
    //    }

    //    private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    //    private static TEntity Private<TEntity>(this object obj, string privateField) => (TEntity)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);

    //}
}



