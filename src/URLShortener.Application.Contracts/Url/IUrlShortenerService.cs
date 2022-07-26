using System;
using System.Threading.Tasks;
using URLShortener.ServiceResponse;
using Volo.Abp.Application.Services;

namespace URLShortener.Url;

public interface IUrlShortenerService : IApplicationService
{
    Task<string> CreateAsync(string originalUrl);
    Task<string> GetAsync(string shortUrl);
    Task<string> CreatePremiumAsync(UrlCreateDto input);
}