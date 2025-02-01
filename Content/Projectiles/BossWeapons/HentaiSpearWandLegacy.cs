using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Projectiles;
using FargoLegacy.Content.Buffs.Masomode;

namespace FargoLegacy.Content.Projectiles.BossWeapons
{
    public class HentaiSpearWandLegacy : ModProjectile
    {
        public override string Texture => "FargoLegacy/Content/Projectiles/BossWeapons/HentaiSpearLegacy";

        private int syncTimer;
        private Vector2 mousePos;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Penetrator");
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.aiStyle = 19;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.3f;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 0;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;

            Projectile.netImportant = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] > 5
                && player.ownedProjectileCounts[ModContent.ProjectileType<HentaiSpearBigDeathrayLegacy>()] < 1)
            {
                Projectile.Kill();
                return;
            }

            if (player.dead || !player.active)
            {
                Projectile.Kill();
                return;
            }

            if (player.HeldItem.type == ModContent.ItemType<Content.Items.Weapon.HentaiSpearLegacy>())
            {
                Projectile.damage = Main.player[Projectile.owner].GetWeaponDamage(Main.player[Projectile.owner].HeldItem);
                Projectile.knockBack = Main.player[Projectile.owner].GetWeaponKnockback(Main.player[Projectile.owner].HeldItem, Main.player[Projectile.owner].HeldItem.knockBack);
            }

            if (Projectile.localAI[0]++ == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<HentaiSpearBigDeathrayLegacy>(),
                      Projectile.damage, Projectile.knockBack, player.whoAmI, 0, Projectile.identity);
                }
            }

            player.velocity *= 0.9f; //move slower while holding it

            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.Center = ownerMountedCenter;
            Projectile.timeLeft = 2;

            if (Projectile.owner == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;

                if (++syncTimer > 20)
                {
                    syncTimer = 0;
                    Projectile.netUpdate = true;
                }
            }

            const float lerp = 0.06f;
            Projectile.velocity = Vector2.Lerp(Vector2.Normalize(Projectile.velocity),
                Vector2.Normalize(mousePos - player.MountedCenter), lerp); //slowly move towards direction of cursor
            Projectile.velocity.Normalize();

            Projectile.position += Projectile.velocity * 164 * 1.3f / 4f; //offset by part of spear's length

            float extrarotate = ((Projectile.direction * player.gravDir) < 0) ? MathHelper.Pi : 0;
            float itemrotate = Projectile.direction < 0 ? MathHelper.Pi : 0;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135);
            Projectile.position -= Projectile.velocity;
            player.itemRotation = Projectile.velocity.ToRotation() + itemrotate;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
            player.ChangeDir(Math.Sign(Projectile.velocity.X));
        }

        public void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 1; //balanceing

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position + new Vector2(Main.rand.Next(target.width), Main.rand.Next(target.height)),
                    Vector2.Zero, ModContent.ProjectileType<PhantasmalBlastLegacy>(), Projectile.damage, Projectile.knockBack * 3f, Projectile.owner);
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoonLegacy>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}