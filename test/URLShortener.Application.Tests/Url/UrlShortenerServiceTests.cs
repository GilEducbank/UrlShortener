
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace URLShortener.Url;

public sealed class UrlShortenerServiceTests : URLShortenerApplicationTestBase
{
    private readonly IUrlShortenerService _urlShortenerService;
    private readonly IConfiguration _configuration;

    public UrlShortenerServiceTests()
    {
        _urlShortenerService = GetRequiredService<IUrlShortenerService>();
        _configuration = GetRequiredService<IConfiguration>();
    }
   
    [Fact]
    public async Task ShouldNotBeEmptyUrl()
    {
        var url = await _urlShortenerService.CreateAsync(randomUrl);//insert database
        var url2 = await _urlShortenerService.CreateAsync(randomUrl);//get from cache
        
        url.OriginalUrl.ShouldNotBeNullOrEmpty();
        (url.ShortenedUrl.Length - (_configuration["App:SelfUrl"] +"/").Length).ShouldBeInRange(1, 10);
        
        JsonConvert.SerializeObject(url).ShouldBe(JsonConvert.SerializeObject(url2));
    }
    
    [Fact]
    public async Task ShouldNotBeEmptyPremiumUrl()
    {
        DateTime date = DateTime.Now.AddDays(30);
        var result = await _urlShortenerService.CreatePremiumAsync(new CreateUrlDto()
        {
            OriginalUrl = "www.educbank.com.br",
            ShortenedUrl = "Educbank",
            ExpireDate = date
        });
        result.ExpireDate.ShouldBe(date);
        result.OriginalUrl.ShouldBe("www.educbank.com.br");
        result.ShortenedUrl.ShouldBe(ConcatenateRootUrl("Educbank"));
    }

    [Fact]
    public async Task ShouldGetUrl()
    {
        DateTime date = DateTime.Now.AddDays(30);
        var url = await _urlShortenerService.CreatePremiumAsync(new CreateUrlDto()
        {
            OriginalUrl = "www.educbank.com.br",
            ShortenedUrl = "Educbank",
            ExpireDate = date
        });
        var result = await _urlShortenerService.GetAsync("Educbank");

        result.ShouldNotBeNull();
        result.OriginalUrl.ShouldBe("www.educbank.com.br");
    }

    [Fact]
    public async Task ShouldNotGetNonExistingUrl()
    {
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _urlShortenerService.GetAsync(randomShortenedUrl);
        });
    }
    
    //private methods
    private const string randomUrl = "www.educbank.com.br";
    private const string randomShortenedUrl = "Educbank";
    private string ConcatenateRootUrl(string shortenedUrl) => _configuration["App:SelfUrl"] + '/' + shortenedUrl;
}