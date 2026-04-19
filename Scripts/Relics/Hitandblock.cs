using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Commands;

namespace Test.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class Hitandblock : CustomRelicModel
{
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Common;

    // 遗物的数值。替换本地化中的{Cards}。
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<CoordinatePower>("sPower",3m),
		new PowerVar<AnticipatePower>("dPower",2m)
        ];
    // 小图标
    public override string PackedIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_min.png";
    // 大图标
    protected override string BigIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_big.png";

    public override bool HasUponPickupEffect => true;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
        CombatState combatState = player.Creature.CombatState;
        int turnnumber = combatState.RoundNumber;
        if (base.Owner.Creature.Player != player)
        {
            return;
        }
		if(turnnumber % 2 == 1)
        {
            await PowerCmd.Remove<WeakPower>(base.Owner.Creature);
            await PowerCmd.Apply<CoordinatePower>(base.Owner.Creature, base.DynamicVars["sPower"].BaseValue, base.Owner.Creature, null);
            await PowerCmd.Apply<FrailPower>(base.Owner.Creature,1m, base.Owner.Creature, null);
        }
        if(turnnumber % 2 == 0)
        {
            await PowerCmd.Remove<FrailPower>(base.Owner.Creature);
            await PowerCmd.Apply<AnticipatePower>(base.Owner.Creature, base.DynamicVars["dPower"].BaseValue, base.Owner.Creature, null);
            await PowerCmd.Apply<WeakPower>(base.Owner.Creature, 1m, base.Owner.Creature, null);
        }
	}
}