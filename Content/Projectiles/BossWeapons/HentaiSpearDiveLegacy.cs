namespace FargoLegacy.Content.Projectiles.BossWeapons
{
    public class HentaiSpearDiveLegacy : HentaiSpearLegacy
    {
        public override string Texture => "FargoLegacy/Content/Projectiles/BossWeapons/HentaiSpearLegacy";
        
        public override void AI()
        {
            base.AI();
            Projectile.localAI[0]++;
        }

        public override bool? CanDamage()
        {
            if (Projectile.localAI[0] > 2)
                return true;
            return null;
        }
    }
}