using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.IdGenerators;
using URLShortener.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;

namespace URLShortener.Url;


public class UrlShortenerService : URLShortenerAppService, IUrlShortenerService
{
    private readonly IRepository<Url, Guid> _urlRepository;
    private readonly UrlManager _urlManager;
    public UrlShortenerService(IRepository<Url, Guid> urlRepository, UrlManager urlManager)
    {
        _urlRepository = urlRepository;
        _urlManager = urlManager;
    }
    
    public async Task<string> CreateAsync(string originalUrl)
    {
        var url = await _urlManager.CreateRandom(originalUrl);
        await _urlRepository.InsertAsync(url);
        return url.ShortenedUrl;
    }

    [HttpGet("{shortenedUrl}")]
   public async Task<string> GetAsync(string shortenedUrl)
   {
       var url = await _urlRepository.FirstOrDefaultAsync(c => c.ShortenedUrl == shortenedUrl);
       
       if (url == null)
       {
           throw new BusinessException("Url not found");
       }
       return url.OriginalUrl;
   }

    [Authorize(UrlShortenerPermissions.CreateUrl)]
    public async Task<string> CreatePremiumAsync(UrlCreateDto input)
    {
        var url = await _urlManager.CreatePremium(input.OriginalUrl, input.ShortenedUrl, input.ExpireDate);
        await _urlRepository.InsertAsync(url);
        return url.ShortenedUrl;
    }

}