using URLShortener.MongoDB;
using Xunit;

namespace URLShortener;

[CollectionDefinition(URLShortenerTestConsts.CollectionDefinitionName)]
public class URLShortenerDomainCollection : URLShortenerMongoDbCollectionFixtureBase
{

}
