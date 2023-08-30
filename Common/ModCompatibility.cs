#nullable enable

using System;
using System.Reflection;
using System.Runtime.Versioning;
using Terraria.ModLoader;

namespace CalamityHunt.Common;

/// <summary>
///     Compile-time- and runtime-consistent mod compatibility helpers.
/// </summary>
public static class ModCompatibility
{
    /// <summary>
    ///     Statically provides a mod's internal name. This is used for taking
    ///     advantage of how CLR generics work.
    /// </summary>
    [RequiresPreviewFeatures]
    public interface IModName
    {
        /// <summary>
        ///     The internal name of the mod.
        /// </summary>
        static abstract string ModName { get; }
    }

    /// <summary>
    ///     A base class for mod compatibility helpers. This is used for taking
    ///     advantage of how CLR generics work.
    /// </summary>
    /// <typeparam name="TModName">The mod name.</typeparam>
    [RequiresPreviewFeatures]
    public abstract class BaseModCompatibility<TModName> where TModName : IModName
    {
        // TODO: We could just use mod != null, but who cares?
        private static readonly Lazy<bool> is_loaded = new(() => ModLoader.HasMod(ModName));
        private static readonly Lazy<Mod?> mod = new(() => ModLoader.TryGetMod(ModName, out var theMod) ? theMod : null);

        /// <summary>
        ///     The internal name of the mod.
        /// </summary>
        public static string ModName => TModName.ModName;

        /// <summary>
        ///     Whether the mod is loaded.
        /// </summary>
        public static bool IsLoaded => is_loaded.Value;

        /// <summary>
        ///     The mod instance, if loaded.
        /// </summary>
        public static Mod? Mod => IsLoaded ? mod.Value : null;

        /// <summary>
        ///     The mod's assembly, if loaded.
        /// </summary>
        public static Assembly? Assembly => Mod?.Code;
    }

    [RequiresPreviewFeatures]
    public abstract class Calamity : BaseModCompatibility<Calamity.CalamityName>
    {
        public abstract class CalamityName : IModName
        {
            static string IModName.ModName => "CalamityMod";
        }
    }
}
