
using System;
using Volo.Abp.EventBus;

namespace URLShortener.Url;
[EventName("UrlShortener.Url.Create")]
public class UrlCreateEto
{
    public Guid UrlId { get; set; }
}