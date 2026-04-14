using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Test.Scripts.Powers;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;


namespace Test.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public sealed class Absorb : CustomCardModel
{

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AbsorbPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<AbsorbPower>(1m)];
	public Absorb()
		: base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
	{
	}
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";  
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<AbsorbPower>(base.Owner.Creature,base.DynamicVars["AbsorbPower"].BaseValue, base.Owner.Creature, this);
	}

	public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == base.Owner.Creature.Side)
		{
			await PowerCmd.Remove<AbsorbPower>(base.Owner.Creature);
		}
	}

	protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
