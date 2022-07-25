using Volo.Abp.Modularity;

namespace URLShortener;

[DependsOn(
    typeof(URLShortenerApplicationModule),
    typeof(URLShortenerDomainTestModule)
    )]
public class URLShortenerApplicationTestModule : AbpModule
{

}
