using FargoLegacy.Core.Toggler.Content;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargoLegacy.Content.Items.Accessories.Enchantments
{
    public class TungstenEnchantLegacy : BaseEnchant
    {

        public override Color nameColor => new(176, 210, 178);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TungstenEffectLegacy>(Item);
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            int shield1Type = ModContent.ItemType<TungstenEnchantLegacy>();
            int shield2Type = ModContent.ItemType<TungstenEnchant>();

            if ((equippedItem.type == shield1Type && incomingItem.type == shield2Type) ||
                (equippedItem.type == shield2Type && incomingItem.type == shield1Type))
            {
                return false;
            }

            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient(ItemID.TungstenHelmet);
            recipe.AddIngredient(ItemID.TungstenChainmail);
            recipe.AddIngredient(ItemID.TungstenGreaves);
            recipe.AddIngredient(ItemID.TungstenBroadsword);
            recipe.AddIngredient(ItemID.Ruler);
            recipe.AddIngredient(ItemID.Katana);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();

            Recipe convert = CreateRecipe(1);
            convert.AddIngredient(ModContent.ItemType<TungstenEnchant>());
            convert.Register();
        }
    }

    public class TungstenEffectLegacy : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TerraHeaderLegacy>();
        public override int ToggleItemType => ModContent.ItemType<TungstenEnchantLegacy>();
        public override void ModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if ((player.FargoSoulsLegacy().ForceEffect<TungstenEnchantLegacy>() || item.shoot == ProjectileID.None))
            {
                TungstenModifyDamage(player, ref modifiers);
            }
        }
        public override void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.FargoSouls().TungstenScaleLegacy != 1)
            {
                TungstenModifyDamage(player, ref modifiers);
            }
        }
        public override void PostUpdateMiscEffects(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.TungstenCD > 0)
                modPlayer.TungstenCD--;
        }
        public static float TungstenIncreaseWeaponSize(FargoSoulsPlayer modPlayer)
        {
            return 1f + (modPlayer.ForceEffect<TungstenEnchantLegacy>() ? 2.5f : 1.5f);
        }

        public static List<int> TungstenAlwaysAffectProjType =
        [
                ProjectileID.MonkStaffT2,
                ProjectileID.Arkhalis,
                ProjectileID.Terragrim,
                ProjectileID.JoustingLance,
                ProjectileID.HallowJoustingLance,
                ProjectileID.ShadowJoustingLance,
                ModContent.ProjectileType<PrismaRegaliaProj>(),
                ModContent.ProjectileType<BaronTuskShrapnel>(),
        ];
        public static List<int> TungstenAlwaysAffectProjStyle =
        [
            ProjAIStyleID.Spear,
            ProjAIStyleID.Yoyo,
            ProjAIStyleID.ShortSword,
            ProjAIStyleID.Flail
        ];
        public static bool TungstenAlwaysAffectProj(Projectile projectile)
        {
            return ProjectileID.Sets.IsAWhip[projectile.type] ||
                TungstenAlwaysAffectProjType.Contains(projectile.type) ||
                TungstenAlwaysAffectProjStyle.Contains(projectile.aiStyle);
        }
        public static List<int> TungstenNeverAffectProjType =
        [
            ModContent.ProjectileType<FishStickProjTornado>(),
            ModContent.ProjectileType<FishStickWhirlpool>(),
            ProjectileID.ButchersChainsaw,
        ];
        public static List<int> TungstenNeverAffectProjStyle = [];
        public static bool TungstenNeverAffectsProj(Projectile projectile)
        {
            return TungstenNeverAffectProjType.Contains(projectile.type) ||
                TungstenNeverAffectProjStyle.Contains(projectile.type);
        }

        public static void TungstenIncreaseProjSize(Projectile projectile, FargoSoulsPlayer modPlayer, IEntitySource source)
        {
            if (TungstenNeverAffectsProj(projectile))
            {
                return;
            }
            bool canAffect = false;
            bool hasCD = true;
            if (TungstenAlwaysAffectProj(projectile))
            {
                canAffect = true;
                hasCD = false;
            }
            else if (FargoSoulsUtil.OnSpawnEnchCanAffectProjectile(projectile, false))
            {
                if (FargoSoulsUtil.IsProjSourceItemUseReal(projectile, source))
                {
                    if (modPlayer.TungstenCD == 0)
                        canAffect = true;
                }
                else if (source is EntitySource_Parent parent && parent.Entity is Projectile sourceProj)
                {
                    if (sourceProj.GetGlobalProjectile<TungstenGlobalProjectileLegacy>().TungstenScaleLegacy != 1)
                    {
                        canAffect = true;
                        hasCD = false;
                    }
                    else if (sourceProj.minion || sourceProj.sentry || ProjectileID.Sets.IsAWhip[sourceProj.type])
                    {
                        if (modPlayer.TungstenCD == 0)
                            canAffect = true;
                    }
                }
            }

            if (canAffect)
            {
                bool forceEffect = modPlayer.ForceEffect<TungstenEnchantLegacy>();
                float scale = forceEffect ? 3f : 2f;
                projectile.position = projectile.Center;
                projectile.scale *= scale;
                projectile.width = (int)(projectile.width * scale);
                projectile.height = (int)(projectile.height * scale);
                projectile.Center = projectile.position;
                TungstenGlobalProjectileLegacy globalProjectile = projectile.GetGlobalProjectile<TungstenGlobalProjectileLegacy>();
                globalProjectile.TungstenScaleLegacy = scale;

                if (projectile.aiStyle == ProjAIStyleID.Spear || projectile.aiStyle == ProjAIStyleID.ShortSword)
                    projectile.velocity *= scale;

                if (hasCD)
                {
                    modPlayer.TungstenCD = 40;

                    if (modPlayer.Eternity)
                        modPlayer.TungstenCD = 0;
                    else if (forceEffect)
                        modPlayer.TungstenCD /= 2;
                }
            }
        }

        public static void TungstenModifyDamage(Player player, ref NPC.HitModifiers modifiers)
        {
            FargoSoulsPlayer modPlayer = player.FargoSoulsLegacy();

            bool forceBuff = modPlayer.ForceEffect<TungstenEnchantLegacy>();

            modifiers.FinalDamage *= forceBuff ? 1.2f : 1.1f;
            player.GetAttackSpeed(DamageClass.Melee) -= forceBuff ? 0.2f : 0.4f;
        }
    }

    public class TungstenGlobalItemLegacy : GlobalItem
    {
        public static List<int> TungstenAlwaysAffects =
        [
            ItemID.TerraBlade,
            ItemID.NightsEdge,
            ItemID.TrueNightsEdge,
            ItemID.Excalibur,
            ItemID.TrueExcalibur,
            //ItemID.PiercingStarlight,
            ItemID.TheHorsemansBlade,
            ModContent.ItemType<TheBaronsTusk>(),
            ItemID.LucyTheAxe,
            ModContent.ItemType<SlimeKingsSlasher>(),
            ItemID.TheAxe
        ];

        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (!item.IsAir && ((item.IsWeapon() && !item.noMelee) || TungstenAlwaysAffects.Contains(item.type)))
            {
                if (player.HasEffect<TungstenEffectLegacy>())
                {
                    scale *= TungstenEffectLegacy.TungstenIncreaseWeaponSize(modPlayer);
                    if (item.type == ItemID.TheAxe && player.name.ToLower().Contains("gonk"))
                        scale *= 2.5f;
                }
                if (modPlayer.Atrophied)
                    scale *= 0.5f;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.PiercingStarlight)
                tooltips.Add(new TooltipLine(Mod, "StarlightTungsten", Language.GetTextValue("Mods.FargowiltasSouls.Items.Extra.StarlightTungsten")));

        }
    }

    public class TungstenGlobalProjectileLegacy : GlobalProjectile
    {
        public int DeletionImmuneRank;
        public float TungstenScaleLegacy = 1;
        public bool TimeFreezeImmune;
        public bool IsAHeldProj;
        public bool CanSplit = true;

        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.HasEffect<TungstenEffectLegacy>())
            {
                TungstenEffectLegacy.TungstenIncreaseProjSize(projectile, modPlayer, source);
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            bool retVal = true;
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (projectile.owner == Main.myPlayer)
            {
                //reset tungsten size
                if (TungstenScaleLegacy != 1 && !player.HasEffect<TungstenEffectLegacy>())
                {
                    projectile.position = projectile.Center;
                    projectile.scale /= TungstenScaleLegacy;
                    projectile.width = (int)(projectile.width / TungstenScaleLegacy);
                    projectile.height = (int)(projectile.height / TungstenScaleLegacy);
                    projectile.Center = projectile.position;

                    TungstenScaleLegacy = 1;
                }
            }
            return retVal;
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            switch (projectile.type)
            {
                case ProjectileID.PiercingStarlight:
                    if (TungstenScaleLegacy != 1)
                    {
                        float swordScaleModifier = TungstenScaleLegacy;
                        float slashScaleModifier = TungstenScaleLegacy * 1.25f;

                        int num = 3;
                        int num2 = 2;
                        Vector2 value = projectile.Center - projectile.rotation.ToRotationVector2() * num2;

                        float num3 = Main.rand.NextFloat();
                        float scale = Utils.GetLerpValue(0f, 0.3f, num3, clamped: true) * Utils.GetLerpValue(1f, 0.5f, num3, clamped: true);
                        Color color = projectile.GetAlpha(Lighting.GetColor(projectile.Center.ToTileCoordinates())) * scale;
                        Texture2D value2 = TextureAssets.Item[4923].Value;
                        Vector2 origin = value2.Size() / 2f;
                        float num4 = Main.rand.NextFloatDirection();
                        float scaleFactor = 8f + MathHelper.Lerp(0f, 20f, num3) + Main.rand.NextFloat() * 6f;
                        scaleFactor *= swordScaleModifier;
                        float num5 = projectile.rotation + num4 * ((float)Math.PI * 2f) * 0.04f;
                        float num6 = num5 + (float)Math.PI / 4f;
                        Vector2 position = value + num5.ToRotationVector2() * scaleFactor + Main.rand.NextVector2Circular(8f, 8f) - Main.screenPosition;
                        SpriteEffects spriteEffects = SpriteEffects.None;
                        if (projectile.rotation < -(float)Math.PI / 2f || projectile.rotation > (float)Math.PI / 2f)
                        {
                            num6 += (float)Math.PI / 2f;
                            spriteEffects |= SpriteEffects.FlipHorizontally;
                        }

                        Main.spriteBatch.Draw(value2, position, null, color, num6, origin, swordScaleModifier, spriteEffects, 0f);


                        for (int j = 0; j < num; j++)
                        {
                            float num7 = Main.rand.NextFloat();
                            float num8 = Utils.GetLerpValue(0f, 0.3f, num7, clamped: true) * Utils.GetLerpValue(1f, 0.5f, num7, clamped: true);
                            float amount = Utils.GetLerpValue(0f, 0.3f, num7, clamped: true) * Utils.GetLerpValue(1f, 0.5f, num7, clamped: true);
                            float scaleFactor2 = MathHelper.Lerp(0.6f, 1f, amount);
                            scaleFactor2 *= slashScaleModifier;
                            Microsoft.Xna.Framework.Color fairyQueenWeaponsColor = projectile.GetFairyQueenWeaponsColor(0.25f, 0f, (Main.rand.NextFloat() * 0.33f + Main.GlobalTimeWrappedHourly) % 1f);
                            Texture2D value3 = TextureAssets.Projectile[projectile.type].Value;
                            Microsoft.Xna.Framework.Color color2 = fairyQueenWeaponsColor;
                            color2 *= num8 * 0.5f;
                            Vector2 origin2 = value3.Size() / 2f;
                            Microsoft.Xna.Framework.Color value4 = Microsoft.Xna.Framework.Color.White * num8;
                            value4.A /= 2;
                            Microsoft.Xna.Framework.Color color3 = value4 * 0.5f;
                            float num9 = 1f;
                            float num10 = Main.rand.NextFloat() * 2f;
                            float num11 = Main.rand.NextFloatDirection();
                            Vector2 vector = new Vector2(2.8f + num10, 1f) * num9 * scaleFactor2;
                            _ = new Vector2(1.5f + num10 * 0.5f, 1f) * num9 * scaleFactor2;
                            int num12 = 50;
                            Vector2 value5 = projectile.rotation.ToRotationVector2() * ((j >= 1) ? 56 : 0);
                            float num13 = 0.03f - (float)j * 0.012f;
                            float scaleFactor3 = 30f + MathHelper.Lerp(0f, num12, num7) + num10 * 16f;
                            scaleFactor3 *= slashScaleModifier;
                            float num14 = projectile.rotation + num11 * ((float)Math.PI * 2f) * num13;
                            float rotation = num14;
                            Vector2 position2 = value + num14.ToRotationVector2() * scaleFactor3 + Main.rand.NextVector2Circular(20f, 20f) + value5 - Main.screenPosition;
                            color2 *= num9;
                            color3 *= num9;
                            SpriteEffects effects = SpriteEffects.None;
                            Main.spriteBatch.Draw(value3, position2, null, color2, rotation, origin2, vector, effects, 0f);
                            Main.spriteBatch.Draw(value3, position2, null, color3, rotation, origin2, vector * 0.6f, effects, 0f);
                        }

                        return false;
                    }
                    break;
            }
            return base.PreDraw(projectile, ref lightColor);
        }

        public static List<Projectile> SplitProj(Projectile projectile, int number, float maxSpread, float damageRatio, bool allowMoreSplit = false)
        {

            List<Projectile> projList = [];
            Projectile split;
            double spread = maxSpread / number;
            for (int i = 0; i < number / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int factor = j == 0 ? 1 : -1;
                    split = FargoSoulsUtil.NewProjectileDirectSafe(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity.RotatedBy(factor * spread * (i + 1)), projectile.type, (int)(projectile.damage * damageRatio), projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                    if (split != null)
                    {
                        split.ai[2] = projectile.ai[2];
                        split.localAI[0] = projectile.localAI[0];
                        split.localAI[1] = projectile.localAI[1];
                        split.localAI[2] = projectile.localAI[2];

                        split.friendly = projectile.friendly;
                        split.hostile = projectile.hostile;
                        split.timeLeft = projectile.timeLeft;
                        split.DamageType = projectile.DamageType;

                        //split.FargoSouls().numSplits = projectile.FargoSouls().numSplits;
                        if (!allowMoreSplit)
                            split.FargoSouls().CanSplit = false;
                        split.FargoSouls().TungstenScaleLegacy = projectile.FargoSouls().TungstenScaleLegacy;

                        projList.Add(split);
                    }
                }
            }

            return projList;
        }

        public override void PostAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (projectile.whoAmI == player.heldProj
                || projectile.aiStyle == ProjAIStyleID.HeldProjectile
                || projectile.type == ProjectileID.LastPrismLaser)
            {
                DeletionImmuneRank = 2;
                TimeFreezeImmune = true;
                IsAHeldProj = true;
                if (player.HasEffect<TungstenEffectLegacy>() && TungstenScaleLegacy == 1)
                {
                    TungstenEffectLegacy.TungstenIncreaseProjSize(projectile, modPlayer, null);
                }
            }
        }

        public override bool TileCollideStyle(Projectile projectile, ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (TungstenScaleLegacy != 1)
            {
                width = (int)(width / TungstenScaleLegacy);
                height = (int)(height / TungstenScaleLegacy);
            }

            return base.TileCollideStyle(projectile, ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (TungstenScaleLegacy != 1 && projectile.type == ProjectileID.PiercingStarlight)
                modifiers.FinalDamage *= 0.4f;
        }
    }
}
