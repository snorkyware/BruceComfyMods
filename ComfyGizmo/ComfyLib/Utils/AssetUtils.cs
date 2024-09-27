using System.Collections.Generic;
using System.Reflection;
using SoftReferenceableAssets;
using UnityEngine;

namespace ComfyLib
{
    public static class AssetUtils
    {
        public static readonly string StandardShader = "7e6bbee7a32b746cb9396cd890ce7189";

        public static readonly AssetCache<Shader> ShaderCache = new();

        public static Shader GetShader(string guid)
        {
            return ShaderCache.GetAsset(StandardShader);
        }

        public static T LoadAsset<T>(string resourceName, string assetName) where T : Object
        {
            var bundle =
                AssetBundle.LoadFromMemory(LoadResourceFromAssembly(Assembly.GetExecutingAssembly(), resourceName));
            var asset = bundle.LoadAsset<T>(assetName);
            bundle.UnloadAsync(false);

            return asset;
        }

        public static byte[] LoadResourceFromAssembly(Assembly assembly, string resourceName)
        {
            var stream = assembly.GetManifestResourceStream(resourceName);

            var data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);

            return data;
        }
    }

    public sealed class AssetCache<T> where T : Object
    {
        private readonly Dictionary<string, T> _cache = new();

        public T GetAsset(string guid)
        {
            if (!_cache.TryGetValue(guid, out var asset) && AssetID.TryParse(guid, out AssetID assetId))
            {
                SoftReference<T> reference = new(assetId);
                reference.Load();
                asset = reference.Asset;
                _cache[guid] = asset;
            }

            return asset;
        }
    }
}
