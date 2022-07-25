using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson.Serialization.IdGenerators;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace URLShortener.Url;


public class UrlShortenerService : URLShortenerAppService, IUrlShortenerService
{
    private readonly IRepository<Url, Guid> _urlRepository;

    public UrlShortenerService(IRepository<Url, Guid> urlRepository)
    {
        _urlRepository = urlRepository;
    }
    
    public async Task<string> CreateAsync(string originalUrl)
    {
        Guid id = new Guid();
        string shortenedUrl = GenerateRandomString();

        while (await _urlRepository.AnyAsync(item => item.ShortenedUrl == shortenedUrl))
        {
            shortenedUrl = GenerateRandomString();
        }
        Url url = new Url(id, originalUrl, shortenedUrl, DateTime.Now.Add(TimeConstants.TimeConstants.DefaultAddDays));

        await _urlRepository.InsertAsync(url);
        
        return shortenedUrl;
    }

    private string GenerateRandomString()
    {
        string keys = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890+-";
        StringBuilder str = new StringBuilder();
        int length = new Random().Next(5, 11);

        for (int i = 0; i < length; i++)
        {
            int index = new Random().Next(0, keys.Length + 1);
            str.Append(keys[index]);
        }
        
        return str.ToString();
    }

    
    public async Task<string> GetAsync(string shortUrl)
    {
        var url = await _urlRepository.FirstOrDefaultAsync(c => c.ShortenedUrl == shortUrl);

        if (url == null)
        {
            throw new BusinessException("Url not found");
        }

        return url.OriginalUrl;
    }
}