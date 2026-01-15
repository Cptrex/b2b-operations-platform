using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Platform.Service.Search.Infrastructure.Db;

public class SearchContextFactory : IDesignTimeDbContextFactory<SearchContext>
{
    public SearchContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SearchContext>();
        
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=platform_search_db;Username=postgres;Password=postgres");

        return new SearchContext(optionsBuilder.Options);
    }
}
