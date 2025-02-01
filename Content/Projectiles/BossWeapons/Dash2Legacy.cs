namespace FargoLegacy.Content.Projectiles.BossWeapons
{
    public class Dash2Legacy : DashLegacy
    {
        public override string Texture => "FargoLegacy/Content/Projectiles/BossWeapons/DashLegacy";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 15 * 60 * (Projectile.extraUpdates + 1);
        }
    }
}