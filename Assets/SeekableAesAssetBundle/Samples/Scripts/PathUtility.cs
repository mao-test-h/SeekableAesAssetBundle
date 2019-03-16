namespace SeekableAesAssetBundle.Samples.Scripts
{
    using System.Text;
    using UnityEngine;

    public static class SamplePathUtility
    {
        public const string EncryptedPrefix = "encrypted_";
        public const string SampleAssetBundleName = "sample";
        
        public static string GetAssetBundleDirectory(CompressType type)
        {
            var builder =
                new StringBuilder()
                    .Append(Application.streamingAssetsPath)
                    .Append("/")
                    .Append(type.ToString())
                    .Append("/");
            return builder.ToString();
        }

        public static string GetAssetBundlePath(CompressType type, bool isEncrypt)
        {
            var builder =
                new StringBuilder()
                    .Append(GetAssetBundleDirectory(type))
                    .Append(isEncrypt ? EncryptedPrefix : "")
                    .Append(SampleAssetBundleName);
            return builder.ToString();
        }
    }
}
