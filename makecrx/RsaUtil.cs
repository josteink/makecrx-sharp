using System;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using System.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace makecrx
{
    public class RsaUtil
    {
        public string KeyFile { get; private set; }

        private AsymmetricCipherKeyPair keyPair;


        public RsaUtil(string keyFile)
        {
            this.KeyFile = keyFile;
        }

        public void GenerateKey()
        {
            const int keySize = 2048;
            var gen = new RsaKeyPairGenerator();
            var secureRandom = new SecureRandom();
            var keyGenParam = new KeyGenerationParameters(secureRandom, keySize);
            gen.Init(keyGenParam);

            this.keyPair = gen.GenerateKeyPair();
            var privateKey = GetPrivateKey();

            using (var fileStream = File.OpenWrite(this.KeyFile))
            using (var textWriter = new StreamWriter(fileStream))
            {
                var pemWriter = new PemWriter(textWriter);
                pemWriter.WriteObject(privateKey);
            }

        }

        public void LoadKey()
        {
            using (var fileStream = File.OpenRead(this.KeyFile))
            using (var textReader = new StreamReader(fileStream))
            {
                var pemReader = new PemReader(textReader);
                this.keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
            }
        }

        private RsaPrivateCrtKeyParameters GetPrivateKey()
        {
            return (RsaPrivateCrtKeyParameters)keyPair.Private;
        }

    }
}