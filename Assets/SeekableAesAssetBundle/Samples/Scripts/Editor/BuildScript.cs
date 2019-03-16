namespace SeekableAesAssetBundle.Samples.Scripts.Editor
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    
    public static class BuildScript
    {
        #region // Builds

        [MenuItem("Samples/Build/Build AssetBundle(Uncompressed)")]
        static void ExecuteBuildAssetBundleUncompressed() => BuildAssetBundleInternal(CompressType.Uncompressed);

        [MenuItem("Samples/Build/Build AssetBundle(LZMA)")]
        static void ExecuteBuildAssetBundleLZMA() => BuildAssetBundleInternal(CompressType.LZMA);

        [MenuItem("Samples/Build/Build AssetBundle(LZ4)")]
        static void ExecuteBuildAssetBundleLZ4() => BuildAssetBundleInternal(CompressType.LZ4);

        static void BuildAssetBundleInternal(CompressType type)
        {
            var outputPath = SamplePathUtility.GetAssetBundleDirectory(type);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var options = BuildAssetBundleOptions.None;
            switch (type)
            {
                case CompressType.Uncompressed:
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                    break;
                case CompressType.LZ4:
                    options |= BuildAssetBundleOptions.ChunkBasedCompression;
                    break;
            }

            BuildPipeline.BuildAssetBundles(outputPath, options, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
            Debug.Log($"Complete BuildAssetBundleInternal({type})");
        }

        #endregion // Builds

        #region // Encrypt

        [MenuItem("Samples/Encrypt/Encrypt AssetBundle(Uncompressed)")]
        static void EncryptAssetBundleUncompressed() => EncryptAssetBundleInternal(CompressType.Uncompressed);

        [MenuItem("Samples/Encrypt/Encrypt AssetBundle(LZMA)")]
        static void EncryptAssetBundleLZMA() => EncryptAssetBundleInternal(CompressType.LZMA);

        [MenuItem("Samples/Encrypt/Encrypt AssetBundle(LZ4)")]
        static void EncryptAssetBundleLZ4() => EncryptAssetBundleInternal(CompressType.LZ4);

        static void EncryptAssetBundleInternal(CompressType type)
        {
            // 非暗号化AssetBundleをbyte[]化 → 暗号化を掛けていく
            var path = SamplePathUtility.GetAssetBundlePath(type, false);
            var bytes = File.ReadAllBytes(path);

            var exportDir = SamplePathUtility.GetAssetBundleDirectory(type);
            var exportFileName = SamplePathUtility.EncryptedPrefix + SamplePathUtility.SampleAssetBundleName;

            EncryptHelper.ExportEncryptBinary(bytes, exportDir + exportFileName);
            AssetDatabase.Refresh();
            Debug.Log($"Complete EncryptAssetBundleInternal({type})");
        }

        #endregion // Encrypt
    }
}
