using CalamityHunt.Common.Graphics.SlimeMonsoon;
using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Misc;
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
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace CalamityHunt
{
	public class CalamityHunt : Mod
	{
        public static bool UndercoverMode = false;

        public override void Load()
        {
            UndercoverMode = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space);
            if (UndercoverMode)
                SoundEngine.PlaySound(SoundID.Unlock);

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

            //Hide the mod from /modlist
            Type modlistCommand = typeof(ModCommand).Assembly.GetType("Terraria.ModLoader.Default.ModlistCommand");
            if (modlistCommand != null)
                HideFromModListIL = new ILHook(modlistCommand.GetMethod("Action"), HideModFromModListCommand);
        }

        public void HideModFromModListCommand(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(MoveType.After,
                i => i.MatchStloc(0)))
            {
                return;
            }

            cursor.Emit(OpCodes.Ldloc, 0);
            cursor.EmitDelegate(SkipCalamityHunt);
            cursor.Emit(OpCodes.Stloc, 0);
        }

        public IEnumerable<Mod> SkipCalamityHunt(IEnumerable<Mod> mods)
        {
            if (!ModLoader.HasMod("CalamityMod"))
                return mods;

            List<Mod> modsWithoutHunt = new List<Mod>();
            foreach (Mod mod in mods)
            {
                if (mod != this)
                    modsWithoutHunt.Add(mod);
            }
             return modsWithoutHunt;
        }

        public static ILHook HideFromModListIL;

        public override void Unload()
        {
            HideFromModListIL?.Undo();
        }

        public override void PostSetupContent()
        { 
            // Kill Old Duke and inject Goozma into boss rush
            if (ModLoader.HasMod("CalamityMod"))
            {
                BossRushInjection(ModLoader.GetMod("CalamityMod"));
            }
            if (ModLoader.HasMod("BossChecklist"))
            {
                BossChecklist(ModLoader.GetMod("BossChecklist"));
            } 
            // Add this whenever Slime Cane is added
            /*if (ModLoader.HasMod("SummonersAssociation"))
            {
                Mod sAssociation = ModLoader.GetMod("SummonersAssociation");
                sAssociation.Call("AddMinionInfo", ItemType<SlimeCane>(), BuffType<SlimeBabyBuff>(), new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>()
                    {
                        ["ProjID"] = ProjectileType<EbonianBaby>()
                    },
                    new Dictionary<string, object>()
                    {
                        ["ProjID"] = ProjectileType<CrimsonBaby>()
                    },
                    new Dictionary<string, object>()
                    {
                        ["ProjID"] = ProjectileType<HallowBaby>()
                    },
                    new Dictionary<string, object>()
                    {
                        ["ProjID"] = ProjectileType<AstralBaby>()
                    }
                });
            }*/
        }
        
        public static void BossRushInjection(Mod cal)
        {
            // Goozma
            List<(int, int, Action<int>, int, bool, float, int[], int[])> brEntries = (List<(int, int, Action<int>, int, bool, float, int[], int[])>)cal.Call("GetBossRushEntries");
            int[] slimeIDs = { ModContent.NPCType<EbonianBehemuck>(), ModContent.NPCType<CrimulanGlopstrosity>(), ModContent.NPCType<DivineGargooptuar>(), ModContent.NPCType<StellarGeliath>(), ModContent.NPCType<Goozmite>() };
            int[] goozmaID = { ModContent.NPCType<Goozma>() };
            Action<int> pr = delegate (int npc)
            {
                SoundStyle roar = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaAwaken");
                int whomst = Player.FindClosest(new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f * 0.5f, 1, 1);
                Player guy = Main.player[whomst];
                SoundEngine.PlaySound(roar, guy.Center);
                NPC.SpawnOnPlayer(whomst, ModContent.NPCType<Goozma>());
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

            brEntries.Insert(ODID, (ModContent.NPCType<Goozma>(), -1, pr, 180, true, 0f, slimeIDs, goozmaID));
            cal.Call("SetBossRushEntries", brEntries);
        }

        public void BossChecklist(Mod bossChecklist)
        {
            int sludge = ModContent.ItemType<OverloadedSludge>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                sludge = ModLoader.GetMod("CalamityMod").Find<ModItem>("OverloadedSludge").Type;
            }
            Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityHunt/Assets/Textures/Goozma/GoozmaBC").Value;
                Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };
            bossChecklist.Call("LogBoss", this, "Goozma", 23.6, () => BossDownedSystem.downedBoss["Goozma"], ModContent.NPCType<Goozma>(), new Dictionary<string, object>()
            {
                ["spawnItems"] = sludge,
                ["customPortrait"] = portrait,
                ["despawnMessage"] = DespawnMessage
            });
        }

        Func<NPC, LocalizedText> DespawnMessage = delegate (NPC npc)
        {
            int numberOfAdjectives = 31;
            int numberOfNouns = 26;
            int numberOfSpecial = 8;
            int specialChance = 20;

            string adjective = "Mods.CalamityHunt.NPCs.Goozma.Titles.Adjective" + Main.rand.Next(1, numberOfAdjectives + 1);
            string noun = "Mods.CalamityHunt.NPCs.Goozma.Titles.Noun" + Main.rand.Next(1, numberOfNouns + 1);
            LocalizedText final = Language.GetText("Mods.CalamityHunt.NPCs.Goozma.BossChecklistIntegration.DespawnMessage").WithFormatArgs(Language.GetText(adjective), Language.GetText(noun));
            if (Main.rand.NextBool(specialChance))
            {
                string special = "Mods.CalamityHunt.NPCs.Goozma.Titles.Specific" + Main.rand.Next(1, numberOfSpecial + 1);
                final = Language.GetText("Mods.CalamityHunt.NPCs.Goozma.BossChecklistIntegration.DespawnMessage").WithFormatArgs(Language.GetText(special), Language.GetOrRegister(""));
            }
            return final;
        };
    }
}