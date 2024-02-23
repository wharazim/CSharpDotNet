using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    interface IProtectedKeyReader
    {
        void Initialize(string providerKey, IEnumerable<KeyValuePair<string, IProtectedKey>> enumerable);
    }

    internal class NetInternalReader : IProtectedKeyReader
    {
        public void Initialize(string providerKey)
        {
      
        }

        public void Initialize(string providerKey, IEnumerable<KeyValuePair<string, IProtectedKey>> enumerable)
        {
            var connectionString = Environment.GetEnvironmentVariable(providerKey);
            if (connectionString != null)
            {

                using (var cn = new SqlConnection(connectionString))
                {
                    using (var cmd = new SqlCommand("", cn))
                    {
                        var ids = enumerable.Select(x => x.Value.Id).ToArray();
                        string[] parameters = new string[ids.Length];
                        for (int i = 0; i < ids.Length; i++)
                        {
                            parameters[i] = string.Format("@Id{0}", i);
                            cmd.Parameters.AddWithValue(parameters[i], ids[i]);
                        }
                        cmd.CommandText = string.Format("SELECT Id, UserName, Password FROM dbo.PasswordTracking WHERE Id IN ({0})", string.Join(", ", parameters));
                        cn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var key = enumerable.Where(x => x.Value.Id.ToLower() == Convert.ToString(reader["Id"]).ToLower())
                                                    .FirstOrDefault();
                                key.Value["username"] = reader.GetString(1);
                                key.Value["password"] = reader.GetString(2);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception($"Provider not configured: '{providerKey}'");
            }
        }
    }

    internal class OAuthReader : IProtectedKeyReader
    {
        public void Initialize(string providerKey)
        {

        }

        public void Initialize(string providerKey, IEnumerable<KeyValuePair<string, IProtectedKey>> enumerable)
        {
            var connectionString = providerKey;
            if (connectionString != null)
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    using (var cmd = new SqlCommand("", cn))
                    {
                        var ids = enumerable.Select(x => x.Value.Id).ToArray();
                        string[] parameters = new string[ids.Length];
                        for (int i = 0; i < ids.Length; i++)
                        {
                            parameters[i] = string.Format("@Id{0}", i);
                            cmd.Parameters.AddWithValue(parameters[i], ids[i]);
                        }
                        cmd.CommandText = string.Format("SELECT ClientId, ClientSecret FROM ClientKeys WHERE ClientId IN ({0})", string.Join(", ", parameters));
                        cn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var key = enumerable.Where(x => x.Value.Id.ToLower() == Convert.ToString(reader["ClientId"]).ToLower())
                                                    .FirstOrDefault();
                                key.Value["client_id"] = Convert.ToString(reader["ClientId"]).ToLower();
                                key.Value["client_secret"] = reader.GetString(1);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception($"Provider not configured: '{providerKey}'");
            }
        }
    }
}
