using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Test.Scripts.Cards;

namespace Test.Scripts.Relics.Multiplayer;

[Pool(typeof(SharedRelicPool))]
public class StealmoneyRelic : CustomRelicModel
{
    public override bool IsAllowed(IRunState runState)
    {
        // 只有多人模式允许生成，单机返回false（不生成）
        return !RunManager.Instance.IsSinglePlayerOrFakeMultiplayer;
    }
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Common;

    // 遗物的数值。替换本地化中的{Cards}。
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new GoldVar(1)
        ];
    // 小图标
    public override string PackedIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_min.png";
    // 大图标
    protected override string BigIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_big.png";

    public override bool HasUponPickupEffect => true;
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		if (side != base.Owner.Creature.Side)
		{
			return;
		}
        Flash();
        CardModel stealcard  = base.Owner.Creature.CombatState.CreateCard<Stealmoney>(base.Owner);
		await CardPileCmd.AddGeneratedCardToCombat(stealcard, PileType.Hand, addedByPlayer: true);
	}
}