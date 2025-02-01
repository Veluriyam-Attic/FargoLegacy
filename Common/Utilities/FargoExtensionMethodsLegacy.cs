using FargoLegacy.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using FargoSoulsPlayer = FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer;

namespace FargoLegacy //lets everything access it without using
{
    public static partial class FargoExtensionMethodsLegacy
    {
        public static TungstenGlobalProjectileLegacy FargoSouls(this Projectile projectile)
            => projectile.GetGlobalProjectile<TungstenGlobalProjectileLegacy>();

        public static FargoSoulsPlayer FargoSoulsLegacy(this Player player)
        {
            return player.GetModPlayer<FargoSoulsPlayer>();
        }
        public static bool ForceEffect<T>(this Player player) where T : AccessoryEffect
        {
            Item item = player.EffectItem<T>();
            if (item == null || item.ModItem == null)
                return false;
            return player.FargoSouls().ForceEffect(item.ModItem);
        }
        public static FargoSoulsPlayer FargoSouls(this Player player)=> player.GetModPlayer<FargoSoulsPlayer>();
    }
}