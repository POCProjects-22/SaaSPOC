using System.Reflection;

namespace POCRepository.Extensions
{
    public static class CommonExtensions
    {
        public static void SetPropValue<T>(T entity, string propName, object value)
        {
            //CreatedByIP
            var p2 = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(x => x.CanWrite && x.Name.ToLower().Contains(propName.ToLower()));
            if (p2 != null)
                p2.SetValue(entity, value, null);
        }

        public static void CopyPropertiesTo<T, TU>(this T source, TU dest) where T : class
        {

            var destProps = typeof(TU).GetProperties()
                     .Where(x => x.CanWrite)
                     .ToList();
            List<string> dpnamelist = new List<string>();

            foreach (PropertyInfo p in destProps)
            {
                dpnamelist.Add(p.Name);
            }
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead && dpnamelist.ToList().Where(y => y.ToLower() == x.Name.ToLower()).Any()).ToList();
            foreach (var sourceProp in sourceProps)
            {
                //do not copy auto generated value column
                if (destProps.Any(x => x.Name.ToLower() == sourceProp.Name.ToLower() && x.Name.ToLower() != "ObjectID".ToLower() //&& x.Name.ToLower() != "Password".ToLower()
                && x.Name.ToLower() != "Index".ToLower() && x.Name.ToLower() != "DBUserIndex".ToLower() && x.Name.ToLower() != "DBDateTime".ToLower()))
                {
                    var p = destProps.FirstOrDefault(x => x.Name.ToLower() == sourceProp.Name.ToLower());
                    if (p.CanWrite)
                    {
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                    }
                }

            }
        }
        public static Guid GetValueObjectID<T>(this T entity)
        {
            try
            {
                var sourceProp = entity.GetType().GetProperty("ObjectID");
                if (sourceProp == null)
                    return new Guid();
                var guid = sourceProp.GetValue(entity, null);
                return Guid.Parse(guid.ToString());
            }
            catch (Exception)
            {
                return new Guid();
            }
        }
        public static TR GetValue<T, TR>(this T entity, string propertyName)
        {
            try
            {
                var sourceProp = entity.GetType().GetProperty(propertyName);
                var pvalue = sourceProp.GetValue(entity, null);
                return (TR)pvalue;
            }
            catch (Exception)
            {
                return default;
            }
        }
        public static void SetValue<T>(this T entity, string propertyName, object value)
        {
            try
            {
                var sourceProp = entity.GetType().GetProperty(propertyName);
                sourceProp.SetValue(entity, value, null);
                return;
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
