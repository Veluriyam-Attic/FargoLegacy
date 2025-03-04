using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoLegacy.Content.Projectiles.BossWeapons
{
    public class PhantasmalEyeBoundaryLegacy : PhantasmalEyeHomingLegacy
    {
        public override string Texture => "Terraria/Images/Projectile_452";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % Projectile.MaxUpdates == 0)
                Projectile.position += Main.player[Projectile.owner].position - Main.player[Projectile.owner].oldPosition;

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.570796f;

            if (Projectile.localAI[0] < ProjectileID.Sets.TrailCacheLength[Projectile.type])
                Projectile.localAI[0] += 0.1f;
            else
                Projectile.localAI[0] = ProjectileID.Sets.TrailCacheLength[Projectile.type];

            Projectile.localAI[1] += 0.25f;
        }
    }
}