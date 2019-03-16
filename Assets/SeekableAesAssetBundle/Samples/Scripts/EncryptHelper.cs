namespace SeekableAesAssetBundle.Samples.Scripts
{
    using System.IO;
    using SeekableAesAssetBundle.Scripts;

    public static class EncryptHelper
    {
        //** WARNING **: MUST be unique for each stream otherwise there is NO security
        // HACK: 仮
        public static readonly byte[] UniqueSalt = new byte[] {0, 1, 2, 3, 4, 5, 6, 7};
        public const string Password = "Password";

        public static void ExportEncryptBinary(byte[] bytes, string exportPath)
        {
            using(var baseStream = File.Create(exportPath))
            using (var cryptStream = new SeekableAesStream(baseStream, Password, UniqueSalt))
            {
                cryptStream.Write(bytes, 0, bytes.Length);
            }
        }

        public static byte[] DecryptBinary(string decryptFilePath)
        {
            using(var baseStream = new FileStream(decryptFilePath, FileMode.Open, FileAccess.ReadWrite))
            using (var cryptStream = new SeekableAesStream(baseStream, Password, UniqueSalt))
            {
                var fileInfo = new FileInfo(decryptFilePath);
                var decryptedBuffer = new byte[fileInfo.Length];
                cryptStream.Read(decryptedBuffer, 0, (int) fileInfo.Length);
                return decryptedBuffer;
            }
        }

        public static void ExportDecryptBinary(string decryptFilePath, string exportPath)
        {
            File.WriteAllBytes(exportPath, DecryptBinary(decryptFilePath));
        }
    }
}
