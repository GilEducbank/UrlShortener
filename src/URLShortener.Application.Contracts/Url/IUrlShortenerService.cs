using System;
using System.Threading.Tasks;
using URLShortener.ServiceResponse;
using Volo.Abp.Application.Services;

namespace URLShortener.Url;

public interface IUrlShortenerService : IApplicationService
{
    Task<CreateUrlDto> CreateAsync(string originalUrl);
    Task<GetUrlDto> GetAsync(string shortUrl);
    Task<CreateUrlDto> CreatePremiumAsync(CreateUrlDto input);
}