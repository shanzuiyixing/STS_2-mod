using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Test.Scripts.Cards.Multiplayer;

[Pool(typeof(ColorlessCardPool))]

public class Guard : CustomCardModel
{

    // 标准动态变量：定义卡牌核心数值（能量）
    protected override IEnumerable<DynamicVar>CanonicalVars => [
        new GoldVar(20)
    ];
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";    

    // 核心约束：仅限多人模式使用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override bool IsPlayable => base.Owner.Gold > 20 ;
    // 构造函数：定义卡牌基础属性
    public Guard ()
        : base(
            1,              // 卡牌费用：0费
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
        await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue, cardPlay.Target.Player);
        await PlayerCmd.LoseGold(base.DynamicVars.Gold.BaseValue, base.Owner);
        await PowerCmd.Apply<CoveredPower>(base.Owner.Creature, 1m, cardPlay.Target, this);
    }

    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		AddKeyword(CardKeyword.Retain);
    }
}