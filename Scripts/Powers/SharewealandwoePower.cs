using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Combat;
using BaseLib.Extensions;

namespace Test.Scripts.Powers;

public sealed class SharewealandwoePower : CustomPowerModel
{
    // 标记：是否已经触发过格挡分享（防递归）
	private bool _hasAlreadyBeenGivenBlock;

    // 能力类型：增益（BUFF）
	public override PowerType Type => PowerType.Buff;
    // 堆叠类型：计数器（层数可叠加，越高效果越强）
	public override PowerStackType StackType => PowerStackType.Single;
    private bool HasAlreadyBeenGivenBlock
	{
		get
		{
			return _hasAlreadyBeenGivenBlock;
		}
		set
		{
			AssertMutable();
			_hasAlreadyBeenGivenBlock = value;
		}
	}

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
{
    // 1. 过滤无效触发条件（不满足则直接退出）
	if (creature == base.Owner
        || base.CombatState.CurrentSide != base.Owner.Side  // 必须是【拥有者的回合】
        || HasAlreadyBeenGivenBlock)  // 防止重复触发
	{
		return;
	}
    // 3. 计算分享给自己
	decimal amountToGive = amount * (decimal)base.Amount;
    // 4. 执行格挡分享
    HasAlreadyBeenGivenBlock = true;  // 锁定标记，防止递归
    await CreatureCmd.GainBlock(base.Owner,amountToGive,ValueProp.Unpowered,null);
    HasAlreadyBeenGivenBlock = false; // 解锁标记
    }



    // 内部数据类：专门存储【被掩护的队友列表】
	private class Data
	{
		public readonly List<Creature> 
        coveredCreatures = new List<Creature>();
	}
    // 初始化内部数据容器

    protected override object InitInternalData()
	{
		return new Data();
	}
    public void AddCoveredCreature(Creature c)
    {
    // 获取内部存储的掩护列表
	List<Creature> coveredCreatures = GetInternalData<Data>().coveredCreatures;
    // 自动去重：不重复添加同一个队友
	if (!GetInternalData<Data>().coveredCreatures.Contains(c))
		{
			coveredCreatures.Add(c);
		}

    // 更新UI文本：拼接被掩护的玩家昵称
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

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != base.Owner)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack_()) 
		{
			return 1m;
		}
		return GetInternalData<Data>().coveredCreatures.Count + 1;
	}
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == CombatSide.Enemy)
		{
			await PowerCmd.Remove(this);
		}
	}
}
