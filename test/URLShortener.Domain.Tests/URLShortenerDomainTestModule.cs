using URLShortener.MongoDB;
using Volo.Abp.Modularity;

namespace URLShortener;

[DependsOn(
    typeof(URLShortenerMongoDbTestModule)
    )]
public class URLShortenerDomainTestModule : AbpModule
{

}
