#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalamityHunt.Common.Utilities;
using log4net;
using log4net.Core;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems;

public sealed class GaslightSystem : ModSystem
{
    private sealed class SilentLog : ILog
    {
        ILogger ILoggerWrapper.Logger => null!;

        void ILog.Debug(object message)
        { }

        void ILog.Debug(object message, Exception exception)
        { }

        void ILog.DebugFormat(string format, params object[] args)
        { }

        void ILog.DebugFormat(string format, object arg0)
        { }

        void ILog.DebugFormat(string format, object arg0, object arg1)
        { }

        void ILog.DebugFormat(string format, object arg0, object arg1, object arg2)
        { }

        void ILog.DebugFormat(IFormatProvider provider, string format, params object[] args)
        { }

        void ILog.Info(object message)
        { }

        void ILog.Info(object message, Exception exception)
        { }

        void ILog.InfoFormat(string format, params object[] args)
        { }

        void ILog.InfoFormat(string format, object arg0)
        { }

        void ILog.InfoFormat(string format, object arg0, object arg1)
        { }

        void ILog.InfoFormat(string format, object arg0, object arg1, object arg2)
        { }

        void ILog.InfoFormat(IFormatProvider provider, string format, params object[] args)
        { }

        void ILog.Warn(object message)
        { }

        void ILog.Warn(object message, Exception exception)
        { }

        void ILog.WarnFormat(string format, params object[] args)
        { }

        void ILog.WarnFormat(string format, object arg0)
        { }

        void ILog.WarnFormat(string format, object arg0, object arg1)
        { }

        void ILog.WarnFormat(string format, object arg0, object arg1, object arg2)
        { }

        void ILog.WarnFormat(IFormatProvider provider, string format, params object[] args)
        { }

        void ILog.Error(object message)
        { }

        void ILog.Error(object message, Exception exception)
        { }

        void ILog.ErrorFormat(string format, params object[] args)
        { }

        void ILog.ErrorFormat(string format, object arg0)
        { }

        void ILog.ErrorFormat(string format, object arg0, object arg1)
        { }

        void ILog.ErrorFormat(string format, object arg0, object arg1, object arg2)
        { }

        void ILog.ErrorFormat(IFormatProvider provider, string format, params object[] args)
        { }

        void ILog.Fatal(object message)
        { }

        void ILog.Fatal(object message, Exception exception)
        { }

        void ILog.FatalFormat(string format, params object[] args)
        { }

        void ILog.FatalFormat(string format, object arg0)
        { }

        void ILog.FatalFormat(string format, object arg0, object arg1)
        { }

        void ILog.FatalFormat(string format, object arg0, object arg1, object arg2)
        { }

        void ILog.FatalFormat(IFormatProvider provider, string format, params object[] args)
        { }

        bool ILog.IsDebugEnabled => false;

        bool ILog.IsInfoEnabled => false;

        bool ILog.IsWarnEnabled => false;

        bool ILog.IsErrorEnabled => false;

        bool ILog.IsFatalEnabled => false;
    }

    private ILHook? modlistCommandActionHook;

    public override void Load()
    {
        base.Load();

        // We just stop edits from being logged instead of stopping events from
        // being raised entirely, since tML has a reason to be aware of our
        // edits regardless. We're trying to trick the user, not the mod loader.
        var loggingType = typeof(Logging);
        var tmlProperty = loggingType.GetProperty("tML", BindingFlags.Static | BindingFlags.NonPublic);

        // We rely on implementation details here, but Logging::tML only has a
        // getter. This utility generates getters and setters based on backing
        // fields with GetMethod or SetMethod are null.
        var (tmlGetter, tmlSetter) = tmlProperty.ResolvePropertyGetterAndSetter();

        // If we couldn't resolve the getter or setter/couldn't create our own,
        // give up.
        if (tmlGetter is null || tmlSetter is null)
        {
            DoGaslighting();
            return;
        }

        var tmlValue = tmlGetter(null);
        tmlSetter(null, new SilentLog());
        DoGaslighting();
        tmlSetter(null, tmlValue);
    }

    public override void Unload()
    {
        base.Unload();

        modlistCommandActionHook?.Dispose();
    }

    private void DoGaslighting()
    {
        var tmlAsm = typeof(ModLoader).Assembly;

        var modlistCommandType = tmlAsm.GetType("Terraria.ModLoader.Default.ModlistCommand");
        var modlistCommandActionMethod = modlistCommandType!.GetMethod("Action", BindingFlags.Public | BindingFlags.Instance)!;
        modlistCommandActionHook = new ILHook(modlistCommandActionMethod, HideModFromModListCommand);
    }

    private void HideModFromModListCommand(ILContext il)
    {
        var c = new ILCursor(il);

        int modListStloc = -1;

        if (!c.TryGotoNext(x => x.MatchCall("Terraria.ModLoader.ModLoader", "get_Mods")))
            return;

        if (!c.TryGotoNext(MoveType.After, x => x.MatchStloc(out modListStloc)))
            return;

        c.Emit(OpCodes.Ldloc, modListStloc);
        c.EmitDelegate((IEnumerable<Mod> mods) =>
        {
            if (!ModLoader.HasMod("CalamityMod"))
                return mods;

            // TODO: Should we use GetMod here instead of GetInstance?
            var calamityHunt = ModContent.GetInstance<CalamityHunt>();

            // We can also check internal names instead of pointer equality.
            return mods.Where(x => x != calamityHunt);
        });
        c.Emit(OpCodes.Stloc, modListStloc);
    }
}
