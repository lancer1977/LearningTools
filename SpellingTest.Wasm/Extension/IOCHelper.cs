using System.Diagnostics;
using System.Reflection;
using PolyhydraGames.Extensions;

namespace SpellingTest.Wasm.Extension
{
    public static class IOCHelper
    {
        public static void AddScoped<T, TInt>(this IServiceCollection services, T implementation)
            where T : class, TInt
            where TInt : class
        {
            services.AddScoped(x => implementation);
            services.AddScoped<TInt>(x => x.GetRequiredService<T>());
        }

        public static void RegisterScopedService<T, T2, T3>(this IServiceCollection collection) where T : class, T2, T3
    where T2 : class
    where T3 : class
        {
            collection.AddScoped<T>();

            collection.AddScoped<T2>(x => x.GetRequiredService<T>());
            collection.AddScoped<T3>(x => x.GetRequiredService<T>());

        }

        public static void RegisterService<T, T2, T3>(this IServiceCollection collection) where T : class, T2, T3
            where T2 : class
            where T3 : class
        {
            collection.AddSingleton<T>();
            collection.AddSingleton<T2>(x => x.GetRequiredService<T>());
            collection.AddSingleton<T3>(x => x.GetRequiredService<T>());

        }

        public static void RegisterTypesAsSingletonAndInterfacesEndingWith(this IServiceCollection collection, string name, params Assembly[] assembly)
        {
            var types = assembly.GetTypesEndingWith(name);
            foreach (var item in types)
            {
                try
                {
                    collection.AddSingleton(item);
                    var interfaces = item.GetInterfaces();
                    foreach (var @interface in interfaces)
                    {
                        collection.AddSingleton(@interface, x => x.GetRequiredService(item));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public static void RegisterTypesAsScopedAndInterfacesEndingWith(this IServiceCollection collection, string name, params Assembly[] assembly)
        {
            var types = assembly.GetTypesEndingWith(name);
            foreach (var item in types)
            {
                try
                {
                    collection.AddScoped(item);
                    var interfaces = item.GetInterfaces();
                    foreach (var @interface in interfaces)
                    {
                        collection.AddScoped(@interface, x => x.GetRequiredService(item));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public static void RegisterInstanceAsScopedAndInterfaces<T>(this IServiceCollection collection, T instance) where T : class
        {
            try
            {
                collection.AsScoped(instance).AsInterfaces<T>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static IServiceCollection AsScoped<T>(this IServiceCollection collection, T instance = null) where T : class
        {
            if (instance == null)
            {
                collection.AddScoped<T>();
            }
            else
            {
                collection.AddScoped(x => instance);
            }
            return collection;
        }
        public static void AsInterfaces<T>(this IServiceCollection collection) where T : class
        {

            try
            {
                var interfaces = typeof(T).GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    collection.AddScoped(@interface, x => x.GetRequiredService(typeof(T)));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void RegisterTypeAsSingletonAndInterfaces<T>(this IServiceCollection collection)
        {

            try
            {
                var type = typeof(T);
                collection.AddSingleton(type);
                var interfaces = type.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    collection.AddScoped(@interface, x => x.GetRequiredService(type));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public static void RegisterTypeAsScopedAndInterfaces<T>(this IServiceCollection collection)
        {

            try
            {
                var type = typeof(T);
                collection.AddScoped(type);
                var interfaces = type.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    collection.AddScoped(@interface, x => x.GetRequiredService(type));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public static IEnumerable<Type> GetSQLDatabaseTypes()
        {
            var dataSourceAssembly = Assembly.Load("PolyhydraGames.Pathfinder.Data.SqlServer");
            return dataSourceAssembly.GetTypesEndingWith("Database");
        }
    }

}
