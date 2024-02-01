using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using UnitTestProject1;

namespace DotNet.EncryptionTests
{
    public class EncryptionClass
    {
        public string RsaEncryptWithPublic(string clearText, string publicKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);

            //var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            //using (var txtreader = new StringReader(publicKey))
            //{
            //    var keyParameter = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();

            //    encryptEngine.Init(true, keyParameter);
            //}

            //var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            //return encrypted;

            return "";
        }

        public string RsaEncryptWithPrivate(string clearText, string privateKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);

            //var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            //using (var txtreader = new StringReader(privateKey))
            //{
            //    var keyPair = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();

            //    encryptEngine.Init(true, keyPair.Private);
            //}

            //var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            //return encrypted;

            return "";
        }


        // Decryption:

        public string RsaDecryptWithPrivate(string base64Input, string privateKey)
        {
            var bytesToDecrypt = Convert.FromBase64String(base64Input);

            //AsymmetricCipherKeyPair keyPair;
            //var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            //using (var txtreader = new StringReader(privateKey))
            //{
            //    keyPair = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();

            //    decryptEngine.Init(false, keyPair.Private);
            //}

            //var decrypted = Encoding.UTF8.GetString(decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
            //return decrypted;

            return "";
        }

        public string RsaDecryptWithPublic(string base64Input, string publicKey)
        {
            var bytesToDecrypt = Convert.FromBase64String(base64Input);

            //var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            //using (var txtreader = new StringReader(publicKey))
            //{
            //    var keyParameter = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();

            //    decryptEngine.Init(false, keyParameter);
            //}

            //var decrypted = Encoding.UTF8.GetString(decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
            //return decrypted;

            return "";
        }
    }

    [TestClass]
    public class DotNetTests
    {
        public const string TEST_PATH = @"C:\Users\Public\Repos\X509SigningAndEncryption_Tests";

        public DotNetTests()
        {
            if (!Directory.Exists(TEST_PATH))
            {
                Directory.CreateDirectory(TEST_PATH);
            }
        }

        [TestMethod]
        public void EncryptTest_encrypt_with_public_and_private()
        {
            // Set up 
            var input = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            var enc = new EncryptionClass();
            var publicKey = File.ReadAllText(Path.Combine(TEST_PATH, "public.pem"));
            var privateKey = File.ReadAllText(Path.Combine(TEST_PATH, "private.key"));

            // Encrypt it
            var encryptedWithPublic = enc.RsaEncryptWithPublic(input, publicKey);

            var encryptedWithPrivate = enc.RsaEncryptWithPrivate(input, privateKey);

            // Decrypt
            var output1 = enc.RsaDecryptWithPrivate(encryptedWithPublic, privateKey);

            var output2 = enc.RsaDecryptWithPublic(encryptedWithPrivate, publicKey);

            //Assert.AreEqual(output1, output2);
            //Assert.AreEqual(output2, input);
        }

        [TestMethod]
        public void Jwt_sign_with_hs256_private_key_verify_with_public_key()
        {
            string key = "secretsecretsecretsecretsecretsecret";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, "HS256");
            //SecurityAlgorithms.HmacSha256Signature

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload
            {
               { "some ", "hello "},
               { "scope", "http://dummy.com/"},
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            var tokenString = handler.WriteToken(secToken);

            Console.WriteLine(tokenString);

            var token = handler.ReadJwtToken(tokenString);
        }

        [TestMethod]
        public void RSA_create_new_keypair_as_pem()
        {
            var csp = new RSACryptoServiceProvider(2048);

            var sb = new StringBuilder();
            PEMExporter.ExportKey(csp, new StringWriter(sb), true);
            File.WriteAllText(Path.Combine(TEST_PATH, "private.key"), sb.ToString());

            sb.Clear();
            PEMExporter.ExportKey(csp, new StringWriter(sb), false);
            File.WriteAllText(Path.Combine(TEST_PATH, "public.pem"), sb.ToString());
        }

        private void RSABlobToPem(RSACryptoServiceProvider csp, string fileName, bool includePrivate)
        {
            var sb = new StringBuilder();
            if (includePrivate)
            {
                PEMExporter.ExportKey(csp, new StringWriter(sb), true);
                File.WriteAllText(fileName, sb.ToString());
                sb.Clear();
            }

            PEMExporter.ExportKey(csp, new StringWriter(sb), false);
            File.WriteAllText(fileName, sb.ToString());
        }

        [TestMethod]
        public void RSA_bytes_to_pem()
        {
            var bytes = File.ReadAllBytes(Path.Combine(TEST_PATH, "test.pvk"));
            var csp = new RSACryptoServiceProvider();
            csp.ImportCspBlob(bytes);

            RSABlobToPem(csp, Path.Combine(TEST_PATH, "private.key"), true);
            RSABlobToPem(csp, Path.Combine(TEST_PATH, "public.pem"), false);
        }

        [TestMethod]
        public void RSA_blob_to_pem()
        {
            var bytes = Convert.FromBase64String(File.ReadAllText(Path.Combine(TEST_PATH, "private.blob")));
            var csp = new RSACryptoServiceProvider();
            csp.ImportCspBlob(bytes);

            RSABlobToPem(csp, Path.Combine(TEST_PATH, "private.key"), true);
            RSABlobToPem(csp, Path.Combine(TEST_PATH, "public.pem"), false);
        }

        [DataContract]
        public class JsonWebKeySet
        {
            [DataMember(Name = "keys")]
            public JsonWebKey[] Keys { get; set; }
        }

        [DataContract]
        public class JsonWebKey
        {
            [DataMember(Name = "alg")]
            public string Algorithm { get; set; }

            [DataMember(Name = "kty")]
            public string KeyType { get; set; }

            [DataMember(Name = "use")]
            public string IntendedUse { get; set; }

            [DataMember(Name = "x5c")]
            public string Cert509Chain { get; set; }

            [DataMember(Name = "n")]
            public string Modulus { get; set; }

            [DataMember(Name = "e")]
            public string Exponent { get; set; }

            [DataMember(Name = "kid")]
            public string KeyIdentifier { get; set; }

            [DataMember(Name = "x5t")]
            public string Cert509Thumbprint { get; set; }
        }

        [TestMethod]
        public void RSA_public_blob_to_jwk()
        {
            var bytes = Convert.FromBase64String(File.ReadAllText(Path.Combine(TEST_PATH, "public.blob")));
            var csp = new RSACryptoServiceProvider();
            csp.ImportCspBlob(bytes);

            var parameters = csp.ExportParameters(false);
            var keyset = new JsonWebKeySet
            {
                Keys = new JsonWebKey[]
                {
                    new JsonWebKey
                    {
                        Algorithm = "RS256",
                        KeyType = "RSA",
                        IntendedUse = "sig",
                        Exponent = Convert.ToBase64String(parameters.Exponent),
                        Modulus = Convert.ToBase64String(parameters.Modulus),
                        KeyIdentifier = "1"
                    }
                }
            };

            Console.WriteLine(JsonConvert.SerializeObject(keyset));

            var jwk = keyset.Keys.Where(k => k.KeyType == "RSA" && k.IntendedUse == "sig").First();
            csp.ImportParameters(new RSAParameters
            {
                Exponent = Convert.FromBase64String(jwk.Exponent),
                Modulus = Convert.FromBase64String(jwk.Modulus)
            });
            bytes = csp.ExportCspBlob(false);
            File.WriteAllText(Path.Combine(TEST_PATH, "public.from_jwk.blob"), Convert.ToBase64String(bytes));
        }

        [TestMethod]
        public void RSA_create_new_keypair_as_blob()
        {
            var csp = new RSACryptoServiceProvider(2048);

            var bytes = csp.ExportCspBlob(true);
            File.WriteAllText(Path.Combine(TEST_PATH, "private.blob"), Convert.ToBase64String(bytes));

            bytes = csp.ExportCspBlob(false);
            File.WriteAllText(Path.Combine(TEST_PATH, "public.blob"), Convert.ToBase64String(bytes));
        }

        [TestMethod]
        public void Jwt_sign_with_rs256_private_key_verify_with_public_key()
        {
            var bytes = Convert.FromBase64String(File.ReadAllText(Path.Combine(TEST_PATH, "private.blob")));
            var csp = new RSACryptoServiceProvider();
            csp.ImportCspBlob(bytes);

            var securityKey = new RsaSecurityKey(csp);
            var credentials = new SigningCredentials(securityKey, "RS256");
            //SecurityAlgorithms.HmacSha256Signature

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload
            {
               { "some ", "hello "},
               { "scope", "http://dummy.com/"},
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            var tokenString = handler.WriteToken(secToken);

            Console.WriteLine(tokenString);
            var sb = new StringBuilder();
            PEMExporter.ExportKey(csp, new StringWriter(sb), false);
            Console.WriteLine(string.Empty);
            Console.WriteLine(sb.ToString());

            var token = handler.ReadJwtToken(tokenString);
        }

        [TestMethod]
        public void X509_read_cer_from_file()
        {
            var bytes = File.ReadAllBytes(Path.Combine(TEST_PATH, "test.cer"));
            var x509 = new X509Certificate2();
            x509.Import(bytes);

            var x509chain = new X509Chain();
            x509chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            x509chain.Build(x509);

            var keyset = new JsonWebKeySet
            {
                Keys = new JsonWebKey[]
                {
                    new JsonWebKey
                    {
                        Algorithm = "RS256", //x509.GetRSAPublicKey().SignatureAlgorithm,
                        KeyType = x509.GetRSAPublicKey().SignatureAlgorithm,  // key type? RSA
                        IntendedUse = "sig",
                        Cert509Chain = "", //Convert.ToBase64String(x509.RawData),
                        Exponent = Convert.ToBase64String(x509.GetRSAPublicKey().ExportParameters(false).Exponent),
                        Modulus = Convert.ToBase64String(x509.GetRSAPublicKey().ExportParameters(false).Modulus),
                        KeyIdentifier = x509.Thumbprint,        // ??
                        Cert509Thumbprint = x509.Thumbprint
                    }
                }
            };

            Console.WriteLine(JsonConvert.SerializeObject(keyset));
        }

        [TestMethod]
        public void X509_export_public_key_pem_from_cer_file()
        {
            var bytes = File.ReadAllBytes(Path.Combine(TEST_PATH, "test.cer"));
            var x509 = new X509Certificate2();
            x509.Import(bytes);

            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(x509.GetRSAPublicKey().ExportParameters(false));
            RSABlobToPem(csp, Path.Combine(TEST_PATH, "test.pem"), false);
        }

        [TestMethod]
        public void X509_output_rsa_public_key()
        {
            var store = new X509Store();
            store.Open(OpenFlags.OpenExistingOnly);

            //var collection = store.Certificates.Find(X509FindType.FindByThumbprint, "1de461700c77f33befb880d967f5a8dbfd8fa412", false);
            var collection = store.Certificates.Find(X509FindType.FindByIssuerName, "localhost", false);
            var x509 = collection[0];

            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(x509.GetRSAPublicKey().ExportParameters(false));
            RSABlobToPem(csp, Path.Combine(TEST_PATH, "x509.rsa_public.pem"), false);
        }

        [TestMethod]
        public void Jwt_sign_with_x509()
        {
            var store = new X509Store();
            store.Open(OpenFlags.OpenExistingOnly);

            //var collection = store.Certificates.Find(X509FindType.FindByThumbprint, "1de461700c77f33befb880d967f5a8dbfd8fa412", false);
            var collection = store.Certificates.Find(X509FindType.FindByIssuerName, "localhost", false);
            var x509 = collection[0];

            var securityKey = new RsaSecurityKey(x509.GetRSAPrivateKey());
            var credentials = new SigningCredentials(securityKey, "RS256");
            var header = new JwtHeader(credentials);

            var payload = new JwtPayload
            {
               { "some ", "hello "},
               { "scope", "http://dummy.com/"},
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            var tokenString = handler.WriteToken(secToken);
            Console.WriteLine(tokenString);
        }
    }
}
