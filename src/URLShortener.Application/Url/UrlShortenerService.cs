using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MongoDB.Bson.Serialization.IdGenerators;
using URLShortener.Localization;
using URLShortener.Permissions;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.UI.Navigation.Urls;

namespace URLShortener.Url;


public class UrlShortenerService : URLShortenerAppService, IUrlShortenerService
{
    private readonly IRepository<Url, Guid> _urlRepository;
    private readonly UrlManager _urlManager;
    private readonly IConfiguration _configuration;
    private readonly IDistributedCache<CreateUrlDto, string> _cache;
    public UrlShortenerService(IRepository<Url, Guid> urlRepository, 
        UrlManager urlManager, 
        IConfiguration configuration,
        IDistributedCache<CreateUrlDto, string> cache)
    {
        _urlRepository = urlRepository;
        _urlManager = urlManager;
        _configuration = configuration;
        _cache = cache;
    }
    
    public async Task<CreateUrlDto> CreateAsync(string originalUrl)
    {
        return await _cache.GetOrAddAsync(
            originalUrl,
            async () => await InsertUrlAtDataBase(originalUrl),
            () => new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
            }
            );
    }
    [HttpGet("{shortenedUrl}")]
   public async Task<GetUrlDto> GetAsync(string shortenedUrl)
   {
       var url = await _urlRepository.FirstOrDefaultAsync(c => c.ShortenedUrl == shortenedUrl);
       
       if (url == null)
       {
           throw new BusinessException("Exception:UrlNotFound");
       }
       
       return ObjectMapper.Map<Url, GetUrlDto>(url);
   }
   /*
   public async Task<GetUrlDto> GetUrlAsync(string shortenedUrl)
   {
       var url = await _urlRepository.FirstOrDefaultAsync(c => ConcatenateRootUrl(c.ShortenedUrl) == shortenedUrl);
       
       if (url == null)
       {
           throw new BusinessException("Exception:UrlNotFound");
       }
       
       return ObjectMapper.Map<Url, GetUrlDto>(url);
   }
   */
    [Authorize(UrlShortenerPermissions.CreateUrl)]
    public async Task<CreateUrlDto> CreatePremiumAsync(CreateUrlDto input)
    {
        var url = await _urlManager.CreatePremium(input.OriginalUrl, input.ShortenedUrl, input.ExpireDate);
        await _urlRepository.InsertAsync(url);
        var response = ObjectMapper.Map<Url, CreateUrlDto>(url);
        
        response.ShortenedUrl = ConcatenateRootUrl(response.ShortenedUrl);
        return response;
    }
    //private methods
    private string ConcatenateRootUrl(string shortenedUrl) => _configuration["App:SelfUrl"] + '/' + shortenedUrl;

    private async Task<CreateUrlDto> InsertUrlAtDataBase(string originalUrl)
    {
        var url = await _urlManager.CreateRandom(originalUrl);
        await _urlRepository.InsertAsync(url);
        var response = ObjectMapper.Map<Url, CreateUrlDto>(url);
        
        response.ShortenedUrl = ConcatenateRootUrl(response.ShortenedUrl);
        return response;
    }
}