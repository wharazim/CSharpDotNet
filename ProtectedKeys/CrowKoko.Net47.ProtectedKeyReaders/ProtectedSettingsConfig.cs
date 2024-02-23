using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CrowKoko.ProtectedKeys
{
    public class ProtectedKeySection
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
                if (!(GetInstance(g.Key.ProviderType) is IProtectedKeyReader o))
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

        private object GetInstance(string strFullyQualifiedName)
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

    public class ProtectedKeySectionConfigurationHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return new ProtectedKeySection(section);
        }
    }

}
