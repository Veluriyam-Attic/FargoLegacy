using FargoLegacy.Projectiles.MutantBoss;
using FargowiltasSouls.Content.Projectiles;
using Terraria;
using Terraria.ModLoader;
using FargoLegacy.Content.Projectiles.BossWeapons;
using FargoLegacy.Content.Buffs.Masomode;

namespace FargoLegacy.Content.Projectiles.BossWeapons
{
    public class HentaiSphereRingLegacy : MutantSphereRingLegacy
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            if (Projectile.timeLeft % Projectile.MaxUpdates == 0)
                Projectile.position += Main.player[Projectile.owner].position - Main.player[Projectile.owner].oldPosition;

            if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<HentaiSpearWandLegacy>()] < 1)
            {
                Projectile.Kill();
                return;
            }
        }

        public void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoonLegacy>(), 600);
            target.immune[Projectile.owner] = 1;
        }
    }
}