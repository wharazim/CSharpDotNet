using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;

namespace BouncyCastle.EncryptionTests
{
    public class EncryptionClass
    {
        public string RsaEncryptWithPublic(string clearText, string publicKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);

            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(publicKey))
            {
                var keyParameter = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();

                encryptEngine.Init(true, keyParameter);
            }

            var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            return encrypted;

        }

        public string RsaEncryptWithPrivate(string clearText, string privateKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);

            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(privateKey))
            {
                var keyPair = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();

                encryptEngine.Init(true, keyPair.Private);
            }

            var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            return encrypted;
        }


        // Decryption:

        public string RsaDecryptWithPrivate(string base64Input, string privateKey)
        {
            var bytesToDecrypt = Convert.FromBase64String(base64Input);

            AsymmetricCipherKeyPair keyPair;
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(privateKey))
            {
                keyPair = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();

                decryptEngine.Init(false, keyPair.Private);
            }

            var decrypted = Encoding.UTF8.GetString(decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
            return decrypted;
        }

        public string RsaDecryptWithPublic(string base64Input, string publicKey)
        {
            var bytesToDecrypt = Convert.FromBase64String(base64Input);

            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(publicKey))
            {
                var keyParameter = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();

                decryptEngine.Init(false, keyParameter);
            }

            var decrypted = Encoding.UTF8.GetString(decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
            return decrypted;
        }
    }

    [TestClass]
    public class BouncyCastleTests
    {
        [TestMethod]
        public void Encrypt_compare_public_pem_w_public_extracted_from_private()
        {
            AsymmetricKeyParameter keyParameterFromPub;
            AsymmetricKeyParameter keyParameterFromPriv;
            AsymmetricCipherKeyPair keyPair;

            var publicKey = File.ReadAllText(@"C:\Temp/public.pem");
            var privateKey = File.ReadAllText(@"C:\Temp\private.key");

            using (var txtreader = new StringReader(publicKey))
            {
                keyParameterFromPub = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();
            }

            using (var txtreader = new StringReader(privateKey))
            {
                keyPair = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();
                keyParameterFromPriv = keyPair.Public;
            }

            Assert.AreEqual(keyParameterFromPub, keyParameterFromPriv); // returns true;
        }

        [TestMethod]
        public void EncryptTest_encrypt_with_public_and_private()
        {
            // Set up 
            var input = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            var enc = new EncryptionClass();
            var publicKey = File.ReadAllText(@"C:\Temp/public.pem");
            var privateKey = File.ReadAllText(@"C:\Temp\private.key");

            // Encrypt it
            var encryptedWithPublic = enc.RsaEncryptWithPublic(input, publicKey);

            var encryptedWithPrivate = enc.RsaEncryptWithPrivate(input, privateKey);

            // Decrypt
            var output1 = enc.RsaDecryptWithPrivate(encryptedWithPublic, privateKey);

            var output2 = enc.RsaDecryptWithPublic(encryptedWithPrivate, publicKey);

            Assert.AreEqual(output1, output2);
            Assert.AreEqual(output2, input);
        }
    }
}
