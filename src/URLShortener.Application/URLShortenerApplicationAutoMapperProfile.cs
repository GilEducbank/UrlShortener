using AutoMapper;
using URLShortener.Url;

namespace URLShortener;

public class URLShortenerApplicationAutoMapperProfile : Profile
{
    public URLShortenerApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<Url.Url, GetUrlDto>();
        CreateMap<Url.Url, CreateUrlDto>();
    }
}
