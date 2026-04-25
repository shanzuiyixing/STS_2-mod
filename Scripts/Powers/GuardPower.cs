using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Test.Scripts.Powers;

public class GuardPower : CustomPowerModel
{
    private class Data
	{
		public readonly List<Creature> coveredCreatures = new List<Creature>();
	}
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;
    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Single;
    // 自定义图标路径，自己指定，或者创建一个基类来统一指定图标路径
    public override string? CustomPackedIconPath => $"res://Test/images/powers/{Id.Entry.ToLowerInvariant()}_small.png";
    public override string? CustomBigIconPath => $"res://Test/images/powers/{Id.Entry.ToLowerInvariant()}_big.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
    new StringVar("Covering")
    ];
    protected override object InitInternalData()
	{
		return new Data();
	}
    public void AddCoveredCreature(Creature c)
	{
		List<Creature> coveredCreatures = GetInternalData<Data>().coveredCreatures;
		if (!GetInternalData<Data>().coveredCreatures.Contains(c))
		{
			coveredCreatures.Add(c);
		}
		StringVar stringVar = (StringVar)base.DynamicVars["Covering"];
		stringVar.StringValue = "";
		for (int i = 0; i < coveredCreatures.Count; i++)
		{
			stringVar.StringValue += PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, coveredCreatures[i].Player.NetId);
			if (i == coveredCreatures.Count - 2)
			{
				stringVar.StringValue += ", and ";
			}
			else if (i < coveredCreatures.Count - 2)
			{
				stringVar.StringValue += ", ";
			}
		}
	}
    public override decimal ModifyDamageMultiplicative(
    Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 条件1：伤害目标必须是【能力拥有者自己】
        if (target != base.Owner) return 1m;
        // 条件2：必须是【强化攻击】（普通攻击不生效）
        if (!props.IsPoweredAttack()) return 1m;
        // 核心：伤害倍率 = 被掩护单位数量 + 1
        return GetInternalData<Data>().coveredCreatures.Count + 1;
    }
public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // 敌方回合结束时，立即移除这个能力
        if (side == CombatSide.Enemy)
            await PowerCmd.Remove(this);
    }


}