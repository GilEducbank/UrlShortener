using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace URLShortener.Url;

public sealed class UrlShortenerManagerTests : URLShortenerDomainTestBase
{
    private readonly UrlManager _urlManager;
    private readonly IRepository<Url, Guid> _urlRepository;
    public UrlShortenerManagerTests()
    {
        _urlManager = GetRequiredService<UrlManager>();
        _urlRepository = GetRequiredService<IRepository<Url, Guid>>();
    }
    
    [Fact]
    public async Task ShouldCreateUrl()
    {
        var url = await _urlManager.CreateRandom(randomUrl);
        url.Id.ShouldNotBe(Guid.Empty);
        url.ShouldNotBeNull();
        url.OriginalUrl.ShouldBe(randomUrl);
        url.ShortenedUrl.ShouldNotBeNullOrEmpty();
    }  
    [Fact]
    public async Task ShouldCreatePremiumUrl()
    {
        //Act

        var fake = await  CreateFakeUrl();
        var url = await _urlManager.CreatePremium(fake.OriginalUrl, fake.ShortenedUrl, fake.ExpireDate);
        
        //Assert
        url.Id.ShouldNotBe(Guid.Empty);
        url.ShouldNotBeNull();
        url.OriginalUrl.ShouldBe(fake.OriginalUrl);
        url.ShortenedUrl.ShouldBe(fake.ShortenedUrl);
        url.ExpireDate.ShouldBe(fake.ExpireDate);
    }

    [Fact]
    public async Task ShouldNotCreateEmptyOriginalUrl()
    {
        var date = DateTime.Now.AddDays(30);
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _urlManager.CreateRandom("");
        });
    }
    [Fact]
    public async Task ShouldNotCreateEmptyOriginalPremiumUrl()
    {
        var date = DateTime.Now.AddDays(30);
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _urlManager.CreatePremium("", "Empty", date);
        });
    }
    
  [Fact]
    public async Task ShouldNotCreatePastDates()
    {
        var date = DateTime.Now.Subtract(TimeConstants.TimeConstants.DefaultAddDays);
        var fake = await CreateFakeUrl();

        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _urlManager.CreatePremium(fake.OriginalUrl, fake.ShortenedUrl, date);
        });
    }

    [Fact]
    public async Task ShouldNotCreateGreaterThan10()
    {
        var fake = await CreateFakeUrl();

        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _urlManager.CreatePremium(fake.OriginalUrl, "stringLengthExaggerated", fake.ExpireDate);
        });
    }

    [Theory]
    [InlineData("aaaaaaa")]
    [InlineData("imNotanUrl")]
    [InlineData("javascript:alert('Hack me!')")]
    [InlineData("www.invalidCaracters.com/\\")]
    [InlineData("www.invalidCaracters.com/#")]
    [InlineData("www.invalidCaracters.com/~")]
    [InlineData("www.invalidCaracters.com/>")]
    public async Task ShouldNotCreateInvalidUrl(string invalidUrl)
    {
        var fake = await CreateFakeUrl();
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _urlManager.CreatePremium(invalidUrl, fake.ShortenedUrl, fake.ExpireDate);
        });
    }

    [Fact]
    public async Task ShouldNotCreateExistingShortenedUrl()
    {
        var fake = await CreateFakeUrl();
        await _urlRepository.InsertAsync(fake);
        var url = await _urlRepository.FirstAsync();

        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _urlManager.CreatePremium(fake.OriginalUrl, url.ShortenedUrl, fake.ExpireDate);
        });
    }
    
    //private methods
    private static string randomUrl = "www.random.com";
    private async Task<Url> CreateFakeUrl()
    {
        var expireDate = DateTime.Now.AddDays(30);
        var url = await _urlManager.CreatePremium("www.trololo.com", "Trololo", expireDate);

        return url;
    }
}