using System;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Utilities;

/// <summary>
///     Various asset-loading- and asset-manipulation-related utilities.
/// </summary>
public static class AssetUtilities
{
    /// <summary>
    ///     Helper system which provides some instanced data.
    /// </summary>
    private class AssetUtilitiesSystem : ModSystem
    {
        internal class AssetManager<T> : IDisposable where T : class
        {
            private Dictionary<string, Asset<T>> _assets = new();

            public Asset<T> GetOrRequestAsset(string path, AssetRequestMode requestMode)
            {
                if (_assets.TryGetValue(path, out var asset))
                    return asset;

                return _assets[path] = ModContent.Request<T>(path, requestMode);
            }

            public void Dispose()
            {
                _assets.Clear();
            }
        }

        private Dictionary<Type, IDisposable> _assetManagers = new();

        public AssetManager<T> GetAssetManager<T>() where T : class
        {
            if (_assetManagers.TryGetValue(typeof(T), out var manager))
                return (AssetManager<T>) manager;

            manager = new AssetManager<T>();
            return (AssetManager<T>) (_assetManagers[typeof(T)] = manager);
        }

        public override void Unload()
        {
            base.Unload();

            foreach (var manager in _assetManagers.Values)
                manager.Dispose();

            _assetManagers.Clear();
        }
    }

    /// <summary>
    ///     Makes a cacheable request for an asset.
    /// </summary>
    /// <param name="path">The path to the asset.</param>
    /// <typeparam name="T">The asset type.</typeparam>
    /// <returns>The cached asset pointer.</returns>
    public static Asset<T> RequestImmediate<T>(string path) where T : class => Request<T>(path, AssetRequestMode.ImmediateLoad);

    public static Asset<T>[] RequestArrayImmediate<T>(string path, int count, int start = 0) where T : class
    {
        Asset<T>[] assets = new Asset<T>[count];
        for (int i = 0; i < assets.Length; i++)
            assets[i] = RequestImmediate<T>(path + (i + start));
        return assets;
    }

    private static Asset<T> Request<T>(string path, AssetRequestMode requestMode) where T : class
    {
        // In the event that this system for some reason has not yet loaded, we
        // can simply handle it like this.
        if (ModContent.GetInstance<AssetUtilitiesSystem>() is { } system)
            return system.GetAssetManager<T>().GetOrRequestAsset(path, requestMode);

        return ModContent.Request<T>(path, requestMode);
    }
}
