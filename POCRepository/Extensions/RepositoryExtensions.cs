using System.Linq.Expressions;

namespace POCRepository.Extensions
{
    public static class RepositoryExtensions
    {
       
        public static Expression<Func<EntityType, PropType>> EqualExpression<EntityType,PropType>(string propertyName,PropType value)
        {
            try
            {
                var props = typeof(EntityType).GetProperties();
                if (props.Any(p => p.Name == propertyName))
                {
                    ConstantExpression valExpression = Expression.Constant(value, typeof(PropType));
                    ParameterExpression pe = Expression.Parameter(typeof(EntityType), "x");
                    MemberExpression member = Expression.Property(pe, propertyName);
                    Expression predicateBody = Expression.Equal(member, valExpression);
                    var exp = Expression.Lambda<Func<EntityType, PropType>>(body: predicateBody, parameters: pe);

                    return exp;
                }
                else
                {
                    throw new Exception("Property not found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static Func<EntityType,PropType> PropertyExpression<EntityType,PropType>(string propertyName)
        {
            try
            {
                var props = typeof(EntityType).GetProperties();
                if (props.Any(p => p.Name == propertyName))
                {
                    var pe = Expression.Parameter(typeof(EntityType), "p");
                    var propertyReference = Expression.Property(pe, propertyName);
                    var constantReference = Expression.Constant(typeof(PropType));
                    var final = Expression.Lambda<Func<EntityType, PropType>>(body: propertyReference, parameters: pe);
                    var exp = final.Compile();

                    return exp;
                }
                else
                {
                    throw new Exception("Property not found");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        } 

        //public static T AddCreatedInfo<T>(this T entity, GenericRepository<T> repository) where T : class
        //{
            

        //    var p2 = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(x => x.CanWrite && x.Name.ToLower().Contains("ObjectID".ToLower()));
        //    if (p2 != null)
        //        p2.SetValue(entity, Guid.NewGuid(), null);
            
        //    p2 = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(x => x.CanWrite && x.Name.ToLower().Contains("Index".ToLower())); 

        //    long nextIndex = 0;
        //    try
        //    {
        //        var maxval = repository.Max<long>("Index");
        //        nextIndex = maxval;
        //    }
        //    catch (Exception ex)
        //    { 
        //    }
        //    nextIndex++;
            

        //    if (p2 != null)
        //        p2.SetValue(entity, nextIndex, null);


        //    int userId = 0;

        //    p2 = typeof(T).GetProperties().FirstOrDefault(x => x.CanWrite && x.Name.ToLower().Contains("DBUserIndex".ToLower()));
        //    if (p2 != null)
        //        p2.SetValue(entity, userId, null);

        //    p2 = typeof(T).GetProperties().FirstOrDefault(x => x.CanWrite && x.Name.ToLower().Contains("DBDateTime".ToLower()));
        //    if (p2 != null)
        //        p2.SetValue(entity, DateTime.Now, null);
        //    p2 = typeof(T).GetProperties().FirstOrDefault(x => x.CanWrite && x.Name.ToLower().Contains("IsDeleted".ToLower()));
        //    if (p2 != null)
        //        p2.SetValue(entity, false, null);
 
        //    return entity;
        //}

      
    }
}
