using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq.Expressions;
using System.Reflection;

namespace SDKResolution.AspNetCore.Http
{
    public static class QueryCollectionFactory
    {
        private static readonly Func<IQueryCollection> _empty;
        private static readonly Func<int, IQueryCollection> _capacity;
        private static readonly Func<Dictionary<string, StringValues>, IQueryCollection> _dictionary;
        private static readonly Func<IQueryCollection, IQueryCollection> _fromCollection;


        static QueryCollectionFactory()
        {
            var typeInAssembly = typeof(HeaderDictionary);
            var assembly = typeInAssembly.Assembly;
            Type? loadedType = null;
            try
            {
                //Try finding it in the SDK
                loadedType = assembly.GetType(typeInAssembly.Namespace + ".QueryCollection");
            }
            catch
            {

            }
            if (loadedType == null)
            {
                loadedType = assembly.GetType(typeInAssembly.Namespace + ".Internal.QueryCollection");
                if (loadedType == null)
                {
                    throw new TypeLoadException($"Unable to load {typeInAssembly.Namespace}.(Internal.)QueryCollection");
                }
            }

            Compile(out _empty, Expression.New(loadedType));
            Compile(out _capacity, Expression.New(loadedType.GetConstructor(new Type[] { typeof(int) }), Expression.Parameter(typeof(int))));
            Compile(out _dictionary, Expression.New(loadedType.GetConstructor(new Type[] { typeof(Dictionary<string, StringValues>) }), Expression.Parameter(typeof(Dictionary<string, StringValues>))));
            Compile(out _fromCollection, Expression.New(loadedType.GetConstructor(new Type[] { loadedType }), Expression.Convert(Expression.Parameter(typeof(IQueryCollection)), loadedType)));
        }

        private class ParameterExpressionFinder : ExpressionVisitor
        {
            public List<ParameterExpression> Parameters
            {
                get;
            } = new List<ParameterExpression>();

            protected override Expression VisitParameter(ParameterExpression node)
            {
                Parameters.Add(node);
                return base.VisitParameter(node);
            }
        }

        private static void Compile<T>(out T empty, NewExpression newExpression)
            where T : Delegate
        {
            var pf = new ParameterExpressionFinder();
            pf.Visit(newExpression);
            var lam = Expression.Lambda(typeof(T), newExpression, pf.Parameters);
            empty = (T)lam.Compile();
        }

        public static IQueryCollection CreateQueryCollection()
        {
            return _empty();
        }

        public static IQueryCollection CreateQueryCollection(int capacity)
        {
            return _capacity(capacity);
        }

        public static IQueryCollection CreateQueryCollection(this Dictionary<string, StringValues> store)
        {
            return _dictionary(store);
        }

        public static IQueryCollection CreateQueryCollection(this IQueryCollection collection)
        {
            return _fromCollection(collection);
        }
    }
}