using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;


using Test.Scripts.Powers;

namespace Test.Scripts.Cards;

[Pool(typeof(CurseCardPool))]

public sealed class SuperSnakeBite : CustomCardModel
{
	public override int MaxUpgradeLevel => 0;

	public override bool CanBeGeneratedByModifiers => false;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<PoisonPower>(),	
		];
	public override IEnumerable<CardKeyword> CanonicalKeywords => [
		CardKeyword.Eternal,
		CardKeyword.Innate,
		CardKeyword.Retain,
		];
    protected override IEnumerable<DynamicVar> CanonicalVars => [ 
	new PowerVar<PoisonPower>(5m)
	];
	public SuperSnakeBite()
		: base(2, CardType.Curse, CardRarity.Curse, TargetType.None)
	{
	}
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";  
		public override bool HasTurnEndInHandEffect => true;


	public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
	{
		await PowerCmd.Apply<PoisonPower>(base.Owner.Creature, base.DynamicVars.Poison.BaseValue, base.Owner.Creature, this);
	}
	
}
