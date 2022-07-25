using System.Threading.Tasks;

namespace URLShortener.Data;

public interface IURLShortenerDbSchemaMigrator
{
    Task MigrateAsync();
}
