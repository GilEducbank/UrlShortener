using URLShortener.MongoDB;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace URLShortener.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(URLShortenerMongoDbModule),
    typeof(URLShortenerApplicationContractsModule)
    )]
public class URLShortenerDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
