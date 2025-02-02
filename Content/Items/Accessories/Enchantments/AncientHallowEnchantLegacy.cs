using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargoLegacy.Core.Toggler.Content;
using FargoLegacy.Content.Projectiles.Minions;
using FargowiltasSouls.Content.Projectiles.Minions;

namespace FargoLegacy.Content.Items.Accessories.Enchantments
{
    public class AncientHallowEnchantLegacy : BaseEnchant
    {
        public override Color nameColor => new Color(150, 133, 100);

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = 180000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AncientHallowEffect(player, Item);
        }

        public static void AncientHallowEffect(Player player, Item item)
        {
            FargoSoulsPlayerLegacy modPlayerLegacy = player.GetModPlayer<FargoSoulsPlayerLegacy>();
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            bool minion = player.AddEffect<AncientHallowMinionLegacy>(item);
            modPlayerLegacy.AncientHallowEnchantActive = true;
            modPlayer.AddMinion(item, minion, ModContent.ProjectileType<HallowSwordLegacy>(), 350, 2f);

                const int focusRadius = 50;

                float num14 = Main.GlobalTimeWrappedHourly % 3f / 3f;
                Color fairyQueenWeaponsColor = GetFairyQueenWeaponsColor(0f, 0f, num14);

                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * focusRadius);
                    offset.Y += (float)(Math.Cos(angle) * focusRadius);
                    Dust dust = Main.dust[Dust.NewDust(
                        player.Center + offset - new Vector2(4, 4), 0, 0,
                        DustID.WhiteTorch, 0, 0, 100, fairyQueenWeaponsColor, 1f
                        )];
                    dust.velocity = player.velocity;
                    dust.noGravity = true;
                }

                Main.projectile.Where(x => x.active && x.hostile && x.damage > 0 && Vector2.Distance(x.Center, player.Center) <= focusRadius + Math.Min(x.width, x.height) / 2 && FargoSoulsUtil.CanDeleteProjectile(x)).ToList().ForEach(x =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int dustId = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.WhiteTorch, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100, fairyQueenWeaponsColor, 3f);
                        Main.dust[dustId].noGravity = true;
                    }

                    // Set ownership
                    x.hostile = false;
                    x.friendly = true;
                    x.owner = player.whoAmI;

                    // Turn around
                    x.velocity *= -1f;

                    // Flip sprite
                    if (x.Center.X > player.Center.X)
                    {
                        x.direction = 1;
                        x.spriteDirection = 1;
                    }
                    else
                    {
                        x.direction = -1;
                        x.spriteDirection = -1;
                    }

                    // Don't know if this will help but here it is
                    x.netUpdate = true;

                    //player.AddBuff(mod.BuffType("HallowCooldown"), 600);
                });

        }

        public static Color GetFairyQueenWeaponsColor(float alphaChannelMultiplier, float lerpToWhite, float rawHueOverride)
        {
            float num = rawHueOverride;

            float num2 = (num + 0.5f) % 1f;
            float saturation = 1f;
            float luminosity = 0.5f;

            Color color3 = Main.hslToRgb(num2, saturation, luminosity, byte.MaxValue);
            //color3 *= this.Opacity;
            if (lerpToWhite != 0f)
            {
                color3 = Color.Lerp(color3, Color.White, lerpToWhite);
            }
            color3.A = (byte)((float)color3.A * alphaChannelMultiplier);
            return color3;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            int shield1Type = ModContent.ItemType<AncientHallowEnchantLegacy>();
            int shield2Type = ModContent.ItemType<AncientHallowEnchant>();

            if ((equippedItem.type == shield1Type && incomingItem.type == shield2Type) ||
                (equippedItem.type == shield2Type && incomingItem.type == shield1Type))
            {
                return false;
            }

            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyAncientHallowHead")
                .AddIngredient(ItemID.AncientHallowedPlateMail)
                .AddIngredient(ItemID.AncientHallowedGreaves)
                .AddIngredient(ItemID.RainbowRod)
                .AddIngredient(ItemID.SwordWhip) //durendal
                .AddIngredient(ItemID.HolyWater, 50)
                .AddTile(TileID.CrystalBall)
                .Register();

            Recipe convert = CreateRecipe();
            convert.AddIngredient(ModContent.ItemType<AncientHallowEnchant>());
            convert.Register();
        }
    }

    public partial class FargoSoulsPlayerLegacy : ModPlayer
    {
        public bool AncientHallowEnchantActive;
    }
    public class AncientHallowMinionLegacy : AccessoryEffect
    {
        public override int ToggleItemType => ModContent.ItemType<AncientHallowEnchantLegacy>();
        public override Header ToggleHeader => Header.GetHeader<SpiritHeaderLegacy>();
        public override bool MinionEffect => true;
    }

}