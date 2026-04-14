using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;

namespace Test.Scripts.Powers;

public class AbsorbPower : CustomPowerModel
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;
    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;
    // 显示格挡UI
    // 自定义图标路径，自己指定，或者创建一个基类来统一指定图标路径
    public override string? CustomPackedIconPath => $"res://Test/images/powers/{Id.Entry.ToLowerInvariant()}_small.png";
    public override string? CustomBigIconPath => $"res://Test/images/powers/{Id.Entry.ToLowerInvariant()}_big.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(0.2m)
        ];

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
	{
		if (dealer != null && (dealer == base.Owner || dealer.PetOwner?.Creature == base.Owner) && props.IsPoweredAttack_() && result.TotalDamage > 0)
		{
			await CreatureCmd.Heal(base.Owner,result.TotalDamage *base.DynamicVars.Heal.BaseValue * base.Amount,true);
		}
	}

}