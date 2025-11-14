using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;

namespace PhantomGG.UnitTests.Helpers;

public static class TestDbContextFactory
{
    public static PhantomContext CreateInMemoryContext(string databaseName = "")
    {
        if (string.IsNullOrEmpty(databaseName))
        {
            databaseName = Guid.NewGuid().ToString();
        }

        var options = new DbContextOptionsBuilder<PhantomContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PhantomContext(options);
    }
}
