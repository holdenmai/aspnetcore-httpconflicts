using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace SDKResolution.AspNetCore.Http.NugetTests
{
    public class Tests
    {
        [Test]
        public void CreateEmptyConstructor()
        {
            var collection = QueryCollectionFactory.CreateQueryCollection();
            Assert.That(collection, Is.TypeOf(typeof(QueryCollection)));
        }
        [Test]
        public void CreateWithCapacity()
        {
            var collection = QueryCollectionFactory.CreateQueryCollection(1);
            Assert.That(collection, Is.TypeOf(typeof(QueryCollection)));
        }
        [Test]
        public void CreateWithDictionary()
        {
            var collection = QueryCollectionFactory.CreateQueryCollection(new Dictionary<string, StringValues>());
            Assert.That(collection, Is.TypeOf(typeof(QueryCollection)));
        }
        [Test]
        public void CreateFromExisting()
        {
            var collection = QueryCollectionFactory.CreateQueryCollection(new QueryCollection());
            Assert.That(collection, Is.TypeOf(typeof(QueryCollection)));
        }
    }
}