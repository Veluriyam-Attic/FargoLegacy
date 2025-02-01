using Terraria;
using Terraria.ModLoader;

namespace FargoLegacy.Content.Buffs.Souls
{
    public class HallowCooldownBuffLegacy : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hallowed Shield Cooldown");
            // Description.SetDefault("Your shield cannot reflect projectiles yet");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}