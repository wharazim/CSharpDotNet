using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp1
{
    public interface IProtectedKey
    {
        string Id { get; }
        string Provider { get; }
        string ProviderType { get; }
        string this[string name] { get; set; }

        Exception Exception { get; }
    }

    public class ProtectedKey : IProtectedKey
    {
        private readonly Dictionary<string, string> _keys = new Dictionary<string, string>();
        private readonly string _provider;
        private readonly string _type;
        public ProtectedKey(string id, string provider, string type)
        {
            _provider = provider;
            _type = type;
            _keys.Add("id", id);
        }

        public ProtectedKey()
        {
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
                return _keys[name];
            }
            else
            {
                return string.Empty;
            }
        }

        public string Id { get => this["id"]; }
        public string UserName { get => this["username"]; }
        public string Password { get => this["password"]; }

        public Exception Exception => GetException();

        public string Provider => _provider;

        public string ProviderType => _type;

        private Exception GetException()
        {
            return null;
        }
    }

    class ProtectedKeySection
    {
        public Exception Exception { get; internal set; }
        private readonly Dictionary<string, IProtectedKey> _keys = new Dictionary<string, IProtectedKey>();
        //private readonly string _provider;

        public IProtectedKey this[string Name] { get => GetProtectedEntity(Name); }

        private IProtectedKey GetProtectedEntity(string name)
        {
            if (_keys.ContainsKey(name))
            {
                return _keys[name];
            }
            else
            {
                return new ProtectedKey();
            }
        }

        public ProtectedKeySection(XmlNode section)
        {
            try
            {
                foreach (XmlNode node in section.ChildNodes)
                {
                    //if (node.Name.ToLower() == "provider")
                    //{
                    //    if (string.IsNullOrEmpty(_provider))
                    //    {
                    //        _provider = node.Attributes["name"].Value;
                    //    }
                    //    else
                    //    {
                    //        throw new ArgumentException("Section can only have a single 'provider' entry");
                    //    }
                    //}
                    if (node.Name.ToLower() == "keys")
                    {
                        var provider = node.Attributes["provider"].Value;
                        var type = node.Attributes["type"].Value;
                        foreach (XmlNode key in node.ChildNodes)
                        {
                            _keys.Add(key.Attributes["name"].Value, new ProtectedKey(key.Attributes["id"].Value, provider, type));
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid config section found: '{node.Name}'");
                    }
                }

                Initialize();
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }

        private void Initialize()
        {
            var grp = _keys.GroupBy(x => new { x.Value.ProviderType, x.Value.Provider });
            foreach (var g in grp)
            {
                var o = GetInstance(g.Key.ProviderType) as IProtectedKeyReader;
                if (o == null)
                {
                    throw new NullReferenceException($"Could not get instance of type: '{g.Key}'");
                }
                o.Initialize(g.First().Value.Provider, g.Select(x => x));
            }
            //{
            //    throw new ArgumentNullException("Missing 'provider' element");
            //}

            //var connectionString = Environment.GetEnvironmentVariable(_provider);
            //if (connectionString != null)
            //{

            //}
            //else
            //{
            //    throw new Exception($"Environment variable not configured: '{_provider}'");
            //}
        }

        public object GetInstance(string strFullyQualifiedName)
        {
            Type type = Type.GetType(strFullyQualifiedName);
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



}
