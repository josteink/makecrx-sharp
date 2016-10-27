using System;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using System.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;

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
            var privateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;

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

        public byte[] GetPublicKey()
        {
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(this.keyPair.Public);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            return serializedPublicBytes;
        }

        public byte[] GetSignature(byte[] data)
        {
            ISigner sig = SignerUtilities.GetSigner("SHA1withRSA");
            sig.Init(true, this.keyPair.Private);

            sig.BlockUpdate(data, 0, data.Length);
            byte[] signature = sig.GenerateSignature();
            return signature;
        }
    }
}