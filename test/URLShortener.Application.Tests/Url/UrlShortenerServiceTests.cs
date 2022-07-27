
using System;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.IdGenerators;
using Shouldly;
using Xunit;

namespace URLShortener.Url;

public sealed class UrlShortenerServiceTests : URLShortenerApplicationTestBase
{
    private readonly IUrlShortenerService _urlShortenerService;
    private readonly UrlManager _urlManager;

    public UrlShortenerServiceTests()
    {
        _urlShortenerService = GetRequiredService<IUrlShortenerService>();
        _urlManager = GetRequiredService<UrlManager>();
    }
   
    [Fact]
    public async Task ShouldNotBeEmptyUrl()
    {
        var result = await _urlShortenerService.CreateAsync("www.educbank.com.br");

        result.ShouldNotBeNullOrEmpty();
        result.ShouldNotBeNullOrWhiteSpace();
        result.Length.ShouldBeInRange(1, 10);
    }
    
    [Fact]
    public async Task ShouldNotBeEmptyPremiumUrl()
    {
        DateTime date = DateTime.Now.AddDays(30);
        var result = await _urlShortenerService.CreatePremiumAsync(new UrlCreateDto()
        {
            OriginalUrl = "www.educbank.com.br",
            ShortenedUrl = "Educbank",
            ExpireDate = date
        });

        result.ShouldNotBeNullOrEmpty();
        result.ShouldNotBeNullOrWhiteSpace();
        result.Length.ShouldBeInRange(1, 10);
    }

    [Fact]
    public async Task ShouldGetUrl()
    {
        DateTime date = DateTime.Now.AddDays(30);
        var url = await _urlShortenerService.CreatePremiumAsync(new UrlCreateDto()
        {
            OriginalUrl = "www.educbank.com.br",
            ShortenedUrl = "Educbank",
            ExpireDate = date
        });
        var result = await _urlShortenerService.GetAsync("Educbank");

        result.ShouldNotBeNull();
        result.ShouldBe("www.educbank.com.br");
    }
}