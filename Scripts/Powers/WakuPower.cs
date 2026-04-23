using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Test.Scripts.Powers;

public class WakuPower : CustomPowerModel
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Debuff;
    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Single;
    // 自定义图标路径，自己指定，或者创建一个基类来统一指定图标路径
    public override string? CustomPackedIconPath => $"res://Test/images/powers/{Id.Entry.ToLowerInvariant()}_small.png";
    public override string? CustomBigIconPath => $"res://Test/images/powers/{Id.Entry.ToLowerInvariant()}_big.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2),
        new PowerVar<CoordinatePower>("sPower",5m),
		new PowerVar<AnticipatePower>("dPower",5m),
        new CardsVar(1)
        ];
    private const int _maxCardsToPlay = 13;

    public override decimal ModifyHandDraw(Player player, decimal count)
	{
		if (player != base.Owner.Player)
		{
			return count;
		}
		return count + base.DynamicVars.Cards.BaseValue;
	}
    public override async Task BeforePlayPhaseStartLate(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != base.Owner.Player)
		{
			return;
		}
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue,base.Owner.Player);
        await PowerCmd.Apply<CoordinatePower>(base.Owner,base.DynamicVars["sPower"].BaseValue,base.Owner,null);
        await PowerCmd.Apply<AnticipatePower>(base.Owner,base.DynamicVars["dPower"].BaseValue,base.Owner,null);
		CombatState combatState = player.Creature.CombatState;
		bool flag;
		using (CardSelectCmd.PushSelector(new VakuuCardSelector()))
		{
			int cardsPlayed;
			for (cardsPlayed = 0; cardsPlayed < 13; cardsPlayed++)
			{
				if (CombatManager.Instance.IsOverOrEnding)
				{
					break;
				}
				if (CombatManager.Instance.IsPlayerReadyToEndTurn(player))
				{
					break;
				}
				CardPile pile = PileType.Hand.GetPile(base.Owner.Player);
				CardModel card = pile.Cards.FirstOrDefault((CardModel c) => c.CanPlay());
				if (card == null)
				{
					break;
				}
				Creature target = GetTarget(card, combatState);
				await card.SpendResources();
				await CardCmd.AutoPlay(choiceContext, card, target, AutoPlayType.Default, skipXCapture: true);
			}
			flag = cardsPlayed >= 13;
			if (cardsPlayed == 0)
			{
				return;
			}
		}
		LocString line = (flag ? new LocString("relics", "WHISPERING_EARRING.warning") : new LocString("relics", "WHISPERING_EARRING.approval"));
		TalkCmd.Play(line, base.Owner.Player.Creature, VfxColor.Purple);
	}

    private Creature? GetTarget(CardModel card, CombatState combatState)
	{
		Rng combatTargets = base.Owner.Player.
        RunState.Rng.CombatTargets;
		return card.TargetType switch
		{
			TargetType.AnyEnemy => combatState.HittableEnemies.FirstOrDefault(), 
			TargetType.AnyAlly => combatTargets.NextItem(combatState.Allies.Where((Creature c) => c != null && c.IsAlive && c.IsPlayer && c != base.Owner.Player.Creature)), 
			TargetType.AnyPlayer => base.Owner.Player.Creature, 
			_ => null, 
		};
	}
}