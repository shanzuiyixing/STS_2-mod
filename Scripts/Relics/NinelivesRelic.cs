using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Test.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class NinelivesRelic : CustomRelicModel
{
    public override bool IsAllowed(IRunState runState)
    {
        return CustomRelicModel.IsBeforeAct3TreasureChest(runState);
    }
    private bool _wasUsed;

    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Common;

    // 遗物的数值。替换本地化中的{Cards}。
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Lives", 9m)
        ];
    public override bool IsUsedUp => _wasUsed;

    public bool WasUsed
	{
		get
		{
			return _wasUsed;
		}
		set
		{
			AssertMutable();
			_wasUsed = value;
			if (IsUsedUp)
			{
				base.Status = RelicStatus.Disabled;
			}
		}
	}
    // 小图标
    public override string PackedIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_min.png";
    // 大图标
    protected override string BigIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_big.png";

    public override bool HasUponPickupEffect => true;

    public override bool ShouldDieLate(Creature creature)
	{
		if (creature != base.Owner.Creature)
		{
			return true;
		}
		if (WasUsed)
		{
			return true;
		}
		return false;
	}
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        // 1. 只对持有者生效
        if (player != base.Owner)
            return count;

        // 2. 只在【第一回合】生效（RoundNumber > 1 = 第二回合及以后）
        if (player.Creature.CombatState.RoundNumber > 1)
            return count;

        // 3. 只在【精英房间（Elite）】生效
        AbstractRoom? currentRoom = player.RunState.CurrentRoom;
        if (currentRoom == null || currentRoom.RoomType != RoomType.Elite)
            return count;

        // 满足所有条件：基础抽牌 + 2张
        return count + (decimal)base.DynamicVars.Cards.IntValue;
    }
}
    public override async Task AfterPreventingDeath(Creature creature)
	{
		Flash();
        AbstractRoom room;
        if (!(room is CombatRoom combatRoom))
        {
            return;
        }
        if (combatRoom.Encounter.RoomType != RoomType.Monster)
        {
            return;
        }
		WasUsed = true;
		await CreatureCmd.Heal(creature, 1m);
	}
}