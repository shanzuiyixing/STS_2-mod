using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Test.Scripts.Powers;
using MegaCrit.Sts2.Core.ValueProps;


namespace Test.Scripts.Relics;

[Pool(typeof(IroncladRelicPool))]
public class LifeswapRelic : CustomRelicModel
{
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Common;

    // 遗物的数值。替换本地化中的{Cards}。
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(8m,ValueProp.Unpowered),
        new PowerVar<LifeswapPower>(8m)
        ];
    // 小图标
    public override string PackedIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 大图标
    protected override string BigIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}.png";

    public override bool HasUponPickupEffect => true;

    public override async Task BeforeCombatStart()
	{
		Flash();
        await PowerCmd.Apply<LifeswapPower>(base.Owner.Creature,base.DynamicVars["LifeswapPower"].BaseValue,base.Owner.Creature,null);
	}


}