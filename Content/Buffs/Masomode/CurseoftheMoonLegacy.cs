using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Core.ModPlayers;

namespace FargoLegacy.Content.Buffs.Masomode
{
    public class CurseoftheMoonLegacy : ModBuff
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Curse of the Moon");
            //Description.SetDefault("The moon's wrath consumes you");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "月之诅咒");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "月亮的愤怒吞噬了你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense -= 20;
            player.endurance -= 0.20f;
            player.GetDamage(DamageClass.Generic) -= 0.2f;
            player.GetCritChance(DamageClass.Generic) -= 20;
            player.GetModPlayer<FargoSoulsPlayer>().CurseoftheMoon = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPCLeagcy>().CurseoftheMoon = true;
        }
    }

    public class FargoSoulsGlobalNPCLeagcy : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool CurseoftheMoon;
    }
}