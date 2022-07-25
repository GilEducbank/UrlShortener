using System;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace URLShortener.Url;

public class UrlManager : DomainService
{
    private readonly IRepository<Url, Guid> _urlRepository;

    public UrlManager(IRepository<Url, Guid> urlRepository)
    {
        _urlRepository = urlRepository;
    }

    private Url Create(string originalUrl, string shortenedUrl,DateTime expireDate)
    {
        var id = GuidGenerator.Create();
        var url = new Url(id, originalUrl, shortenedUrl, expireDate);
        return url;
    }

    public async Task<Url> CreateRandom(string originalUrl)
    {
        var shortenedUrl = GetRandomString();
        var url = Create(originalUrl, await shortenedUrl, DateTime.Now);
        return url;
    }

    public async Task<Url> CreatePremium(string originalUrl, string shortenedUrl, DateTime expireDate)
    {

        if (await _urlRepository.AnyAsync(item => item.ShortenedUrl == shortenedUrl))
        {
            throw new BusinessException("ShortenedUrl already exists");
        }

        var url = Create(originalUrl, shortenedUrl, expireDate);
        return url;
    }

    public async Task<Url> Get(string shortenedUrl)
    {
        var url = await _urlRepository.FirstOrDefaultAsync(c => c.ShortenedUrl == shortenedUrl);

        if (url == null)
        {
            throw new BusinessException("Url not found");
        }

        return url;
    }
    
    private async Task<string> GetRandomString()
    {
        var shortenedUrl = GenerateRandomString();

        while (await _urlRepository.AnyAsync(item => item.ShortenedUrl == shortenedUrl))
        {
            shortenedUrl = GenerateRandomString();
        }

        return shortenedUrl;
    }
    private string GenerateRandomString()
    {
        const string keys = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890+-";
        var str = new StringBuilder();
        var length = new Random().Next(5, 11);

        for (var i = 0; i < length; i++)
        {
            var index = new Random().Next(0, keys.Length + 1);
            str.Append(keys[index]);
        }
        
        return str.ToString();
    }
}