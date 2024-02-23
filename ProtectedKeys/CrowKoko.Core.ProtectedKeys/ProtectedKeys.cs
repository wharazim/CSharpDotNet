using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CrowKoko.ProtectedKeys;

public interface IProtectedKey
{
    string Id { get; }
    string Provider { get; }
    string ProviderType { get; }
    string this[string name] { get; set; }
}

public class ProtectedKey : IProtectedKey
{
    private readonly Dictionary<string, string?> _keys = [];
    private readonly string _provider;
    private readonly string _type;
    public ProtectedKey(string? id, string? provider, string? type)
    {
        _provider = provider ?? "";
        _type = type ?? "";
        _keys.Add("id", id);
    }

    public string this[string name] { get => GetValue(name); set => SetValue(name, value); }

    private void SetValue(string name, string value)
    {
        if (_keys.ContainsKey(name.ToLower()))
        {
            _keys[name.ToLower()] = value;
        }
        else
        {
            _keys.Add(name.ToLower(), value);
        }
    }

    private string GetValue(string name)
    {
        if (_keys.ContainsKey(name.ToLower()))
        {
            return _keys[name] ?? "";
        }
        else
        {
            return string.Empty;
        }
    }

    public string Id { get => this["id"]; }
    public string UserName { get => this["username"]; }
    public string Password { get => this["password"]; }
    public string Provider => _provider;
    public string ProviderType => _type;
}

