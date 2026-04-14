using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Test.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]

public class RevivePactRelic : CustomRelicModel
{

    private bool _wasUsed;
    public override bool IsUsedUp => _wasUsed;
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Common;

    // 遗物的数值。替换本地化中的{Cards}。
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(1m),
        new MaxHpVar(23)
        ];
    
    // 小图标
    public override string PackedIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_min.png";
    // 大图标
    protected override string BigIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_big.png";
    [SavedProperty]
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

    public override bool ShouldDieLate(Creature creature)
	{
		if (creature == base.Owner.Creature || !base.Owner.Creature.IsPlayer)
		{
			return true;
		}
		if (WasUsed)
		{
			return true;
		}
		return false;
	}
     public override async Task AfterPreventingDeath(Creature creature){
        {
            Flash();
            _wasUsed = true;
            decimal amount = Math.Max(1m, (decimal)creature.MaxHp * (base.DynamicVars.Heal.BaseValue / 100m));
            await CreatureCmd.Heal(creature, amount);
        }
    }
    public override async Task AfterCombatVictory(CombatRoom _)
	{
		if (!base.Owner.Creature.IsDead)
		{
			Flash();
            _wasUsed = false;
            AssertMutable();
		}
	}
    public override async Task AfterObtained()
    {
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars.MaxHp.BaseValue, isFromCard: false);
    }
}