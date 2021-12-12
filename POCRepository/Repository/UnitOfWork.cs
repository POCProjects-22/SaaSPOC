using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using POCRepository.Extensions;
using System.Net;

namespace POCRepository.Repository
{

    public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IDisposable
where TContext : IdentityDbContext, new()
    {

        private readonly TContext _context;
        private bool _disposed;
        private string _errorMessage = string.Empty;
        private IDbContextTransaction _objTran;
        private HttpContext _httpContext;
        private Dictionary<string, object> _repositories;
        private string _IP="";
        public UnitOfWork()
        {
            _context = new TContext();
        }
        public UnitOfWork(HttpContext httpContext) :this()
        {
            _httpContext = httpContext;

            IPAddress remoteIpAddress = _httpContext.Connection.RemoteIpAddress;
            _IP= remoteIpAddress.ToString();
        }

        

        //The Dispose() method is used to free unmanaged resources like files, 
        //database connections etc. at any time.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //This Context property will return the DBContext object.
        public TContext Context
        {
            get { return _context; }
        }
        //This CreateTransaction() method will create a database Trnasaction so that we can do database operations by
        //applying do evrything and do nothing principle
        public void CreateTransaction()
        { 
            _objTran = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        }
        //If all the Transactions are completed successfuly then we need to call this Commit() 
        //method to Save the changes permanently in the database
        public void Commit()
        {
            _objTran.Commit();
        }
        //If atleast one of the Transaction is Failed then we need to call this Rollback() 
        //method to Rollback the database changes to its previous state
        public void Rollback()
        {
            _objTran.Rollback();
            _objTran.Dispose();
        }
        //This Save() Method Implement IdentityDbContext Class SaveChanges method so whenever we do a transaction we need to
        //call this Save() method so that it will make the changes in the database
        public void Save()
        {
            try
            { 
                var changes = _context.ChangeTracker.Entries().ToList();
                foreach (dynamic obj in changes)
                {
                    if (obj.State ==  EntityState.Added)
                    {
                        CommonExtensions.SetPropValue(obj.Entity, "CreatedByIP", _IP);
                        CommonExtensions.SetPropValue(obj.Entity, "CreatedTime",DateTime.UtcNow.AddHours(6));
                    }
                    else if (obj.State == EntityState.Modified)
                    {
                        CommonExtensions.SetPropValue(obj.Entity, "UpdatedByIP", _IP);
                        CommonExtensions.SetPropValue(obj.Entity, "UpdateTime", DateTime.UtcNow.AddHours(6));
                    }

                }
                
                _context.SaveChanges(); 
            }
            catch (Exception dbEx)
            {
                throw;// new Exception(dbEx.Message, dbEx);
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();
            _disposed = true;
        }
        public GenericRepository<T,TC> GenericRepository<T,TC>() where T : class where TC : IdentityDbContext,new()
        {
            if (_repositories == null)
                _repositories = new Dictionary<string, object>();
            var type = typeof(T).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<T,TC>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                _repositories.Add(type, repositoryInstance);
            }
            return (GenericRepository<T,TC>)_repositories[type];
        }
    }
}
