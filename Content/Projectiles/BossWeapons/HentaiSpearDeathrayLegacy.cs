using System;
using System.Collections.Generic;
using FargoLegacy.Content.Buffs.Masomode;
using FargoLegacy.Content.Projectiles.Deathrays;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargoLegacy.Content.Projectiles.BossWeapons
{
    public class HentaiSpearDeathrayLegacy : BaseDeathrayLegacy
    {
        public HentaiSpearDeathrayLegacy() : base(90, "PhantasmalDeathrayML") { }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Phantasmal Deathray");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CooldownSlot = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            //Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;

            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundStyle soundStyle = new SoundStyle("FargoLegacy/Sounds/Zombie_104");
                if (!Main.dedServ)
                    Terraria.Audio.SoundEngine.PlaySound(soundStyle, Projectile.Center);
                //Terraria.Audio.SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                //Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCKilled, (int)Projectile.Center.X, (int)Projectile.Center.Y, 6, 1f, 0);
            }
            float num801 = .5f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            //Projectile.scale = num801;
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 2.5f * num801;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            //float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            //Projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)Projectile.ai[1]].ai[3] - 1.57079637f;
            //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //Projectile.rotation = num804;
            //num804 += 1.57079637f;
            //Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = Projectile.localAI[0] * Projectile.ai[1];
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            Projectile.position -= Projectile.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57079637f;

            if (++Projectile.frameCounter > 3)
            {
                if (++Projectile.frame > 15)
                    Projectile.frame = 0;

                switch (Projectile.frame)
                {
                    case 1:
                    case 3:
                    case 9:
                    case 11: Projectile.frameCounter = 2; break;
                    default: Projectile.frameCounter = 0; break;
                }
            }
        }
        
        public void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoonLegacy>(), 600);
            target.immune[Projectile.owner] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture2D19 = ModContent.Request<Texture2D>("FargoLegacy/Content/Projectiles/Deathrays/Mutant/MutantDeathray_" + Projectile.frame.ToString()+"Legacy", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D20 = ModContent.Request<Texture2D>("FargoLegacy/Content/Projectiles/Deathrays/Mutant/MutantDeathray2_" + Projectile.frame.ToString()+"Legacy", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D21 = ModContent.Request<Texture2D>("FargoLegacy/Content/Projectiles/Deathrays/" + texture + "3"+"Legacy", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            float num223 = Projectile.localAI[1];
            Color color44 = new Color(255, 255, 255, 0) * 0.95f;
            Texture2D arg_ABD8_1 = texture2D19;
            Vector2 arg_ABD8_2 = Projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            Main.EntitySpriteDraw(arg_ABD8_1, arg_ABD8_2, sourceRectangle2, color44, Projectile.rotation, texture2D19.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            num223 -= (texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
            Vector2 value20 = Projectile.Center;
            value20 += Projectile.velocity * Projectile.scale * texture2D19.Height / 2f;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = new Rectangle(0, 0, texture2D20.Width, 30);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }

                    Main.EntitySpriteDraw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), color44, Projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                    num224 += rectangle7.Height * Projectile.scale;
                    value20 += Projectile.velocity * rectangle7.Height * Projectile.scale;
                    rectangle7.Y += 30;
                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            Texture2D arg_AE2D_1 = texture2D21;
            Vector2 arg_AE2D_2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;
            Main.EntitySpriteDraw(arg_AE2D_1, arg_AE2D_2, sourceRectangle2, color44, Projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}