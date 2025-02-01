using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace FargoLegacy.Core.Toggler.Content
{
    public class TerraHeaderLegacy : EnchantHeader
    {
        public override int Item => ModContent.ItemType<TerraForce>();
        public override float Priority => 10f;
    }
    public class SpiritHeaderLegacy : EnchantHeader
    {
        public override int Item => ModContent.ItemType<SpiritForce>();
        public override float Priority => 10.1f;
    }
}
