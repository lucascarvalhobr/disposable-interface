using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace DisposableInterface.Tests;

public class GetDateTests
{
    private readonly IConfiguration _config;

    public string ConnectionString
    {
        get
        {
            return _config?.GetConnectionString("db");
        }
    }

    public GetDateTests()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);

        _config = configBuilder.Build();
    }


    [Fact]
    public void WithUsing()
    {
        using var database = new Database(ConnectionString);
        Debug.WriteLine($"GetDate: {database.GetDate()}");
        Wait();
    }

    [Fact]
    public void WithoutUsing()
    {
        var database = new Database(ConnectionString);
        Debug.WriteLine($"GetDate: {database.GetDate()}");
        Wait();
    }

    [Fact]
    public void LoopWithUsing()
    {
        for (int i = 0; i < 1000; i++)
        {
            using var database = new Database(ConnectionString);
            Debug.WriteLine($"GetDate: {database.GetDate()}");
        }

        Wait();
    }

    [Fact]
    public void LoopWithCatch()
    {
        try
        {
            for (int i = 0; i < 1000; i++)
            {
                var database = new Database(ConnectionString);
                Debug.WriteLine($"GetDate: {database.GetDate()}");
            }

            Wait();
        }
        catch (Exception)
        {
            GC.Collect();
        }       
    }

    [Fact]
    public void LoopWithoutUsing()
    {
        for (int i = 0; i < 1000; i++)
        {
            var database = new Database(ConnectionString);
            Debug.WriteLine($"GetDate: {database.GetDate()}");
        }

        Wait();
    }

    private void Wait()
    {
        Debug.WriteLine("Waiting...");
        Thread.Sleep(TimeSpan.FromSeconds(5));
    }
}