using Microsoft.Extensions.Configuration;

namespace CrowKoko.ProtectedKeys;

public class ProtectedKeyConfigurationContext(Dictionary<string, IProtectedKey> keys) : IDisposable
{
    //private readonly string _connectionString = connectionString ?? "";
    private bool disposedValue;
    public Dictionary<string, IProtectedKey> Keys = keys;

    public IDictionary<string, string?> Settings = new Dictionary<string, string?>();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~ProtectedKeyConfigurationContext()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class ProtectedKeyConfigurationSource(Dictionary<string, IProtectedKey> keys) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new ProtectedKeyConfigurationProvider(keys);
}

public class ProtectedKeyConfigurationProvider(Dictionary<string, IProtectedKey> keys) : ConfigurationProvider
{
    public override void Load()
    {
        using var dbContext = new ProtectedKeyConfigurationContext(keys);

        dbContext.Settings = CreateAndSaveDefaultValues(dbContext);

        Data = dbContext.Settings;
          
    }

    static Dictionary<string, string?> CreateAndSaveDefaultValues(
        ProtectedKeyConfigurationContext context)
    {
        var settings = new Dictionary<string, string?>(
            StringComparer.OrdinalIgnoreCase);
        foreach (var k in context.Keys)
        {
            settings.Add($"ProtectedKeys:{k.Key}:Id", k.Value["id"]);
            settings.Add($"ProtectedKeys:{k.Key}:UserName", k.Value["username"]);
            settings.Add($"ProtectedKeys:{k.Key}:Password", k.Value["password"]);
            settings.Add($"ProtectedKeys:{k.Key}:ClientSecret", k.Value["client_secret"]);
        }

        return settings;
    }

}

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddProtectedKeyConfiguration(
        this IConfigurationBuilder builder)
    {
        var tempConfig = builder.Build();
        var section = tempConfig.GetSection("ProtectedKeys");
        var _keys = new Dictionary<string, IProtectedKey>();

        foreach (var path in section.GetChildren())
        {
            foreach (var p2 in path.GetChildren())
            {
                foreach (var p3 in p2.GetChildren())
                {
                    var provider = path["Provider"];
                    var type = path["Type"];
                    _keys.Add(p3["name"] ?? "", new ProtectedKey(p3["id"], provider, type));
                }
            }
        }

        var grp = _keys.GroupBy(x => new { x.Value.ProviderType, x.Value.Provider });
        foreach (var g in grp)
        {
            if (GetInstance(g.Key.ProviderType) is not IProtectedKeyReader o)
            {
                throw new NullReferenceException($"Could not get instance of type: '{g.Key}'");
            }
            o.Initialize(g.First().Value.Provider, g.Select(x => x));
        }

        return builder.Add(new ProtectedKeyConfigurationSource(_keys));
    }

    public static object? GetInstance(string strFullyQualifiedName)
    {
        Type? type = Type.GetType(strFullyQualifiedName);
        if (type != null)
        {
            return Activator.CreateInstance(type);
        }

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = asm.GetType(strFullyQualifiedName);
            if (type != null)
            {
                return Activator.CreateInstance(type);
            }
        }

        return null;
    }
}