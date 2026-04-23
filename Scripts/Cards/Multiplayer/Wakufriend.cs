using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Test.Scripts.Powers;


namespace Test.Scripts.Cards.Multiplayer;

[Pool(typeof(ColorlessCardPool))]

public class Wakufriend : CustomCardModel
{
    // 额外悬浮提示
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    // 标准动态变量：定义卡牌核心数值
    protected override IEnumerable<DynamicVar>CanonicalVars => [
      new PowerVar<IntangiblePower>(1m),
      new PowerVar<WakuPower>(1m),
    ];
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";    

    // 核心约束：仅限多人模式使用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WakuPower>()];
    // 构造函数：定义卡牌基础属性
    public Wakufriend ()
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
        await PowerCmd.Remove<WakuPower>(base.Owner.Creature);
        await PowerCmd.Apply<IntangiblePower>(cardPlay.Target, base.DynamicVars["IntangiblePower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<WakuPower>(cardPlay.Target, base.DynamicVars["WakuPower"].BaseValue, base.Owner.Creature, this);
    CardModel wakucard = cardPlay.Target.Player.Creature.CombatState.CreateCard<Wakufriend>(cardPlay.Target.Player);
	CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(wakucard, PileType.Draw,addedByPlayer: true), 1.5f);    
    }
    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		AddKeyword(CardKeyword.Retain);
    }
}