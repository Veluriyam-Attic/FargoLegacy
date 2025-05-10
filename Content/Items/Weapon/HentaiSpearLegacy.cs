using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items;
using FargoLegacy.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Items.Weapons.FinalUpgrades;

namespace FargoLegacy.Content.Items.Weapon
{
    public class HentaiSpearLegacy : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 10));

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 1700;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.shootSpeed = 6f;
            Item.knockBack = 7f;
            Item.width = 72;
            Item.height = 72;
            //Item.scale = 1.3f;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.HentaiSpearLegacy>();
            Item.value = Item.sellPrice(0, 70);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.useTurn = false;

            if (player.altFunctionUse == 2)
            {
                if (player.controlUp && player.controlDown)
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearWandLegacy>();
                    Item.shootSpeed = 6f;
                    Item.useAnimation = 16;
                    Item.useTime = 16;
                }
                else if (player.controlUp && !player.controlDown)
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearSpinThrownLegacy>();
                    Item.shootSpeed = 6f;
                    Item.useAnimation = 16;
                    Item.useTime = 16;
                }
                else if (player.controlDown && !player.controlUp)
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearSpinBoundaryLegacy>();
                    Item.shootSpeed = 1f;
                    Item.useAnimation = 16;
                    Item.useTime = 16;
                    Item.useTurn = true;
                }
                else
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearThrownLegacy>();
                    Item.shootSpeed = 25f;
                    Item.useAnimation = 85;
                    Item.useTime = 85;
                }

                Item.DamageType = DamageClass.Ranged;
            }
            else
            {
                if (player.controlUp && !player.controlDown)
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearSpinLegacy>();
                    Item.shootSpeed = 1f;
                    Item.useTurn = true;
                }
                else if (player.controlDown && !player.controlUp)
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearDiveLegacy>();
                    Item.shootSpeed = 6f;
                }
                else
                {
                    Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.HentaiSpearLegacy>();
                    Item.shootSpeed = 6f;
                }

                Item.useAnimation = 16;
                Item.useTime = 16;

                Item.DamageType = DamageClass.Melee;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) // Right-click
            {
                if (player.controlUp)
                {
                    if (player.controlDown) // Giga-beam
                        return player.ownedProjectileCounts[Item.shoot] < 1;

                    if (player.ownedProjectileCounts[Item.shoot] < 1) // Remember to transfer any changes here to hentaispearspinthrown!
                    {
                        Vector2 speed = Main.MouseWorld - player.MountedCenter;

                        if (speed.Length() < 360)
                            speed = Vector2.Normalize(speed) * 360;

                        Projectile.NewProjectile(source, position, Vector2.Normalize(speed), Item.shoot, damage, knockback, player.whoAmI, speed.X, speed.Y);
                    }

                    return false;
                }

                return true;
            }

            if (player.ownedProjectileCounts[Item.shoot] < 1)
            {
                if (player.controlUp && !player.controlDown)
                    return true;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<DashLegacy>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<Dash2Legacy>()] < 1)
                {
                    float dashAI = 0;
                    float speedModifier = 2f;
                    int dashType = ModContent.ProjectileType<DashLegacy>();

                    if (player.controlUp && player.controlDown) // Super-dash
                    {
                        dashAI = 1;
                        speedModifier = 2.5f;
                    }

                    Vector2 speed = velocity;

                    if (player.controlDown && !player.controlUp) //dive
                    {
                        dashAI = 2;
                        speed = new Vector2(Math.Sign(velocity.X) * 0.0001f, speed.Length());
                        dashType = ModContent.ProjectileType<Dash2Legacy>();
                    }

                    int p = Projectile.NewProjectile(source, position, Vector2.Normalize(speed) * speedModifier * Item.shootSpeed,
                        dashType, damage, knockback, player.whoAmI, speed.ToRotation(), dashAI);
                    if (p != Main.maxProjectiles)
                        Projectile.NewProjectile(source, position, speed, Item.shoot, damage, knockback, player.whoAmI, Main.projectile[p].identity, 1f);
                }
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Penetrator>())
                .Register();
        }
    }
}