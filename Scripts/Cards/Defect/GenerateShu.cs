using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using Test.Scripts.Orbs;

namespace Test.Scripts.Cards.Defect;

[Pool(typeof(DefectCardPool))]

public sealed class GenerateShu : CustomCardModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
HoverTipFactory.Static(StaticHoverTip.Channeling),
		HoverTipFactory.FromOrb<ShuOrb>() 
    ];

    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";  

	public GenerateShu()
		: base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await OrbCmd.Channel<ShuOrb>(choiceContext, base.Owner);
	}

	protected override void OnUpgrade()
	{
		base.EnergyCost.UpgradeBy(-1);
	}
}
