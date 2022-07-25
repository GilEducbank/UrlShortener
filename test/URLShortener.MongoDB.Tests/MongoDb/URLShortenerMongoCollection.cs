using Xunit;

namespace URLShortener.MongoDB;

[CollectionDefinition(URLShortenerTestConsts.CollectionDefinitionName)]
public class URLShortenerMongoCollection : URLShortenerMongoDbCollectionFixtureBase
{

}
