using CalamityHunt.Common.Graphics.SlimeMonsoon;
using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityHunt
{
	public class CalamityHunt : Mod
	{
        public override void Load()
        {
            Ref<Effect> stellarblackhole = new Ref<Effect>(ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SpaceHole", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["HuntOfTheOldGods:StellarBlackHole"] = new Filter(new ScreenShaderData(stellarblackhole, "BlackHolePass"), EffectPriority.VeryHigh);
            Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Load();

            Ref<Effect> distort = new Ref<Effect>(ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/RadialDistortion", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"] = new Filter(new ScreenShaderData(distort, "DistortionPass"), EffectPriority.Medium);
            Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].Load();

            SkyManager.Instance["HuntOfTheOldGods:SlimeMonsoon"] = new SlimeMonsoonBackground();
            SkyManager.Instance["HuntOfTheOldGods:SlimeMonsoon"].Load();

            bool exists = ModContent.RequestIfExists($"{nameof(CalamityHunt)}/Charcoal", out Asset<Texture2D> asset, AssetRequestMode.ImmediateLoad);
            if (exists)
            {
                Vector2 keySize = new Vector2(283, 238);
                SurfaceFormat keyFormat = SurfaceFormat.Color;
                int keyLevelCount = 1;

                Texture2D texture = asset.Value;
                bool checkSize = texture.Width != keySize.X || texture.Height != keySize.Y;
                bool checkFormat = texture.Format != keyFormat;
                bool checkLevels = texture.LevelCount != keyLevelCount;
                if (checkSize || checkFormat || checkLevels)
                    throw new DataMisalignedException();
            }
            else
                throw new DataMisalignedException();

        }

        public override void PostSetupContent()
        { 
            // Kill Old Duke and inject Goozma into boss rush
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod cal = ModLoader.GetMod("CalamityMod");
                List<(int, int, Action<int>, int, bool, float, int[], int[])> brEntries = (List<(int, int, Action<int>, int, bool, float, int[], int[])>)cal.Call("GetBossRushEntries");
                int[] slimeIDs = { ModContent.NPCType<EbonianBehemuck>(), ModContent.NPCType<CrimulanGlopstrosity>(), ModContent.NPCType<DivineGargooptuar>(), ModContent.NPCType<StellarGeliath>() };
                int[] goozmaID = { ModContent.NPCType<Goozma>() };
                Action<int> pr = delegate (int npc)
                {
                    NPC.SpawnOnPlayer(Main.myPlayer, ModContent.NPCType<Goozma>());
                };
                int ODID = cal.Find<ModNPC>("OldDuke").Type;

                for (int i = 0; i < brEntries.Count(); i++)
                {
                    if (brEntries[i].Item1 == ODID)
                    {
                        brEntries.RemoveAt(i);
                        ODID = i;
                        break;
                    }
                }

                brEntries.Insert(ODID, (ModContent.NPCType<Goozma>(), -1, pr, 180, false, 0f, slimeIDs, goozmaID));
                cal.Call("SetBossRushEntries", brEntries);
            }
        }        
    }
}