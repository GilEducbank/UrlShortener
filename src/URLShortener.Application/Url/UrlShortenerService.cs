using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MongoDB.Bson.Serialization.IdGenerators;
using URLShortener.Localization;
using URLShortener.Permissions;
using Volo.Abp;
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
    private readonly IStringLocalizer<ExceptionResource> _exceptionLocalizer;
    public UrlShortenerService(IRepository<Url, Guid> urlRepository, 
        UrlManager urlManager, 
        IConfiguration configuration, 
        IStringLocalizer<ExceptionResource> exceptionLocalizer)
    {
        _urlRepository = urlRepository;
        _urlManager = urlManager;
        _configuration = configuration;
        _exceptionLocalizer = exceptionLocalizer;
    }
    
    public async Task<CreateUrlDto> CreateAsync(string originalUrl)
    {
        var url = await _urlManager.CreateRandom(originalUrl);
        await _urlRepository.InsertAsync(url);
        var response = ObjectMapper.Map<Url, CreateUrlDto>(url);
        
        response.ShortenedUrl = ConcatenateRootUrl(response.ShortenedUrl);
        return response;
    }
    [HttpGet("{shortenedUrl}")]
   public async Task<GetUrlDto> GetAsync(string shortenedUrl)
   {
       var url = await _urlRepository.FirstOrDefaultAsync(c => c.ShortenedUrl == shortenedUrl);
       
       if (url == null)
       {
           throw new BusinessException(_exceptionLocalizer["Exception:UrlNotFound"]);
       }
       
       return ObjectMapper.Map<Url, GetUrlDto>(url);
   }

    [Authorize(UrlShortenerPermissions.CreateUrl)]
    public async Task<CreateUrlDto> CreatePremiumAsync(CreateUrlDto input)
    {
        var url = await _urlManager.CreatePremium(input.OriginalUrl, input.ShortenedUrl, input.ExpireDate);
        await _urlRepository.InsertAsync(url);
        var response = ObjectMapper.Map<Url, CreateUrlDto>(url);
        
        response.ShortenedUrl = ConcatenateRootUrl(response.ShortenedUrl);
        return response;
    }

    private string ConcatenateRootUrl(string shortenedUrl) => _configuration["App:SelfUrl"] + '/' + shortenedUrl;
}