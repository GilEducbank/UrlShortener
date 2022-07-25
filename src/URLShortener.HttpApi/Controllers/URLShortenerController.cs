using URLShortener.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace URLShortener.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class URLShortenerController : AbpControllerBase
{
    protected URLShortenerController()
    {
        LocalizationResource = typeof(URLShortenerResource);
    }
}
