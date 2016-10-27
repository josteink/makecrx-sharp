using System;
using System.IO;

namespace makecrx
{
    /// <summary>
    /// Implementation of CRX package-format according to
    /// https://developer.chrome.com/extensions/crx
    /// </summary>
    public class CrxPackager
    {
        public void Package(string zipFileName, RsaUtil rsaUtil, string targetPath)
        {
            // get parts
            var magic = GetMagic();
            var version = GetVersion();
            var publicKey = rsaUtil.GetPublicKey();
            var zipFile = File.ReadAllBytes(zipFileName);
            var signature = rsaUtil.GetSignature(zipFile);

            // write file
            using (var fs = File.OpenWrite(targetPath))
            {
                WriteBytes(fs, magic);
                WriteBytes(fs, version);
                WriteInt32(fs, publicKey.Length);
                WriteInt32(fs, signature.Length);
                WriteBytes(fs, publicKey);
                WriteBytes(fs, signature);
                WriteBytes(fs, zipFile);
            }
        }

        private byte[] GetMagic()
        {
            return new byte[] { 0x43, 0x72, 0x32, 0x34 }; // Cr24
        }

        private byte[] GetVersion()
        {
            return new byte[] { 0x02, 0x00, 0x00, 0x00 }; // Cr24
        }

        private void WriteBytes(Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        private void WriteInt32(Stream stream, int value)
        {
            byte b1, b2, b3, b4;
            b1 = (byte)(value & 255);
            b2 = (byte)((value >> 8) & 255);
            b3 = (byte)((value >> 16) & 255);
            b4 = (byte)((value >> 24) & 255);

            var bytes = new byte[] { b1, b2, b3, b4 };
            WriteBytes(stream, bytes);
        }
    }
}