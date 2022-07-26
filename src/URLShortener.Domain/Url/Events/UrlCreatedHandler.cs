using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace URLShortener.Url;

public class UrlCreatedHandler : IDistributedEventHandler<UrlCreateEto>, ITransientDependency
{
    private readonly IRepository<Url, Guid> _urlRepository;
    
    public UrlCreatedHandler(IRepository<Url, Guid> urlRepository)
    {
        _urlRepository = urlRepository;
    }
    [UnitOfWork]
    public virtual async Task HandleEventAsync(UrlCreateEto eventData)
    {
        
        try
        {
            var url = await _urlRepository.FirstAsync(item => item.Id == eventData.UrlId);
            Console.WriteLine($"A new Shortened Url was created: ");
            Console.WriteLine($"id = {url.Id}");
            Console.WriteLine($"Original Url = {url.OriginalUrl}");
            Console.WriteLine($"Shortened Url = {url.ShortenedUrl}");
            Console.WriteLine($"Expiration date = {url.ExpireDate}");
        }
        catch
        {
            Console.WriteLine("Shortened url was not created.");
        }

    }
}