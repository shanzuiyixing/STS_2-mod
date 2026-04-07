using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Test.Scripts.Powers;

public class LifeswapPower : CustomPowerModel
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;
    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;
    // 显示格挡UI
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block),];
    // 自定义图标路径，自己指定，或者创建一个基类来统一指定图标路径
    public override string? CustomPackedIconPath => "res://Test/images/powers/test-lifeswap_power.png";
    public override string? CustomBigIconPath => "res://Test/images/powers/test-lifeswap_power.png";
    private const string _selfDamageKey = "SelfDamage";

    // protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar("SelfDamage", 0m, ValueProp.Unblockable | ValueProp.Unpowered)];

    public override async Task BeforeTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == base.Owner.Side)
		{
			Flash(); // BUFF闪光特效
			// 获得格挡 = 层数，固定数值（不受任何加成）
			await CreatureCmd.GainBlock(base.Owner,base.Amount, ValueProp.Unpowered, null);
		}
	}

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != base.Owner.Side)
		{
			return;
		}
        decimal currentHp = base.Owner.CurrentHp;
        decimal currentBlock = base.Owner.Block;
        decimal diff = base.Owner.Block;
        
        if(base.Owner.Block > base.Owner.CurrentHp)
        {
            await CreatureCmd.LoseBlock(Owner, Owner.Block); // 清空所有格挡
            diff = currentBlock - currentHp;
            await CreatureCmd.Heal(base.Owner, diff); // 回复
            await CreatureCmd.GainBlock(base.Owner,currentHp,ValueProp.Unpowered,null); // 
        }
        if(base.Owner.Block < base.Owner.CurrentHp)
        {
            await CreatureCmd.LoseBlock(Owner, Owner.Block); // 清空所有格挡
            diff = currentHp - currentBlock;
            await CreatureCmd.Damage(choiceContext,base.Owner,diff,ValueProp.Unpowered,base.Owner,null); 
            await CreatureCmd.GainBlock(base.Owner,currentHp,ValueProp.Unpowered,null); // 
        }
    }
}