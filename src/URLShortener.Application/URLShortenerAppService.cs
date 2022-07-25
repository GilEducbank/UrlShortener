using System;
using System.Collections.Generic;
using System.Text;
using URLShortener.Localization;
using Volo.Abp.Application.Services;

namespace URLShortener;

/* Inherit your application services from this class.
 */
public abstract class URLShortenerAppService : ApplicationService
{
    protected URLShortenerAppService()
    {
        LocalizationResource = typeof(URLShortenerResource);
    }
}
