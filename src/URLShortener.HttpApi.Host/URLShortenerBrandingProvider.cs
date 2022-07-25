using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace URLShortener;

[Dependency(ReplaceServices = true)]
public class URLShortenerBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "URLShortener";
}
