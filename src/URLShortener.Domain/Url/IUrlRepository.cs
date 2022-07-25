using System;
using Volo.Abp.Domain.Repositories;

namespace URLShortener.Url;

public interface IUrlRepository : IRepository<Url, Guid>
{
    
}