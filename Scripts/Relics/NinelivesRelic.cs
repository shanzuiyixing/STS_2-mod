using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Random;

namespace Test.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class NinelivesRelic : CustomRelicModel
{
    private bool _wasUsed;

    // 剩余生命次数（初始9条命）
	[SavedProperty] 
    private int _remainingLives = 9;

    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Common;

    // 遗物的数值。替换本地化中的{Cards}。
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Lives", RemainingLives),
        new MaxHpVar(10)
        ];
    public override bool IsUsedUp =>  RemainingLives <= 0;

    public override bool ShowCounter => true;
	[SavedProperty]
	public int RemainingLives
	{
		get
		{
			return _remainingLives;
		}
		set
		{
			AssertMutable();
			_remainingLives = value;
			InvokeDisplayAmountChanged();
		}
	}

    // 界面上显示的数字
	public override int DisplayAmount => _remainingLives;
    // 小图标
    public override string PackedIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_min.png";
    // 大图标
    protected override string BigIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_big.png";

    public override bool ShouldDieLate(Creature creature)
	{
        if (creature != base.Owner.Creature)
		{
			return true;
		}
		if (_remainingLives <= 0)
		{
			return true;
		}
		return false;
	}

    public override async Task AfterPreventingDeath(Creature creature)
    {

        Flash();
		RemainingLives--;
        await CreatureCmd.Heal(creature, 1m);
        var targets = creature.CombatState?.HittableEnemies;
		if (creature.Player.RunState.CurrentRoom != null && creature.Player.RunState.CurrentRoom.RoomType == RoomType.Boss)
		{
			return;
		}
        else if (targets != null && targets.Count > 0)
        {
			Rng rng = Owner.Creature.Player.PlayerRng.Rewards;
        int Damagenum = rng.NextInt(999);
            await CreatureCmd.Damage(
                new ThrowingPlayerChoiceContext(),
                targets,
                Damagenum,
                ValueProp.Unblockable | ValueProp.Unpowered,
                null, null);
        }
        
    }

    public override async Task AfterObtained()
	{
        Flash();
		await CreatureCmd.SetMaxHp(base.Owner.Creature,base.DynamicVars.MaxHp.BaseValue);
	}

}