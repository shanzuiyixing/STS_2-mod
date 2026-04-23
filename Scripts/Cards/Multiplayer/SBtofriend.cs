using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.HoverTips;
using Test.Scripts.Powers;

namespace Test.Scripts.Cards.Multiplayer;

[Pool(typeof(ColorlessCardPool))]

public class SBtofriend : CustomCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

   

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SuperSbPower>(),
HoverTipFactory.FromCard<SuperSnakeBite>()
    ];

    // 标准动态变量：定义卡牌核心数值（能量）
    protected override IEnumerable<DynamicVar>CanonicalVars => [
        new PowerVar<SuperSbPower>(1m),
        new PowerVar<AbsorbPower>(1m)
    ];
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";    

    // 核心约束：仅限多人模式使用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 构造函数：定义卡牌基础属性
    public SBtofriend ()
        : base(
            0,              // 卡牌费用：0费
            CardType.Skill, // 卡牌类型：技能牌
            CardRarity.Uncommon, // 卡牌稀有度：稀有
            TargetType.AnyAlly)  // 目标类型：任意一名队友
    {
    }
    // 卡牌被使用时执行
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 校验：必须选择目标队友（为空则报错）
    ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        CardModel cardModel = cardPlay.Target.CombatState.CreateCard<SuperSnakeBite>(cardPlay.Target.Player);
		await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
        await PowerCmd.Apply<SuperSbPower>(cardPlay.Target,base.DynamicVars["SuperSBPower"].BaseValue,base.Owner.Creature,this);
        await PowerCmd.Apply<SuperSbPower>(cardPlay.Target,base.DynamicVars["AbsorbPower"].BaseValue,base.Owner.Creature,this);
    }

    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		AddKeyword(CardKeyword.Retain);
    }
}