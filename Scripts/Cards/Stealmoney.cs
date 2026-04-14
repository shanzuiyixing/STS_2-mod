using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Test.Scripts.Cards;

[Pool(typeof(CurseCardPool))]

public class Stealmoney : CustomCardModel
{
    // 标准动态变量：定义卡牌核心数值（能量）
    protected override IEnumerable<DynamicVar>CanonicalVars => [
        new GoldVar(1)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";    

    // 核心约束：仅限多人模式使用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 构造函数：定义卡牌基础属性
    public Stealmoney ()
        : base(
            0,              // 卡牌费用：0费
            CardType.Skill, // 卡牌类型：技能牌
            CardRarity.Uncommon, // 卡牌稀有度：稀有
            TargetType.AllAllies)  
    {
    }
    // 卡牌被使用时执行
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
        where c != null && c.IsAlive && c.IsPlayer
        select c;

    // 2. 遍历每个队友，执行抽牌
    foreach (Creature item in enumerable)
    {
        await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue, base.Owner);
        if(item == base.Owner.Creature)
            {
                continue;
            }
        await PlayerCmd.LoseGold(base.DynamicVars.Gold.BaseValue, item.Player);
    }
    }

    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		base.DynamicVars.Gold.UpgradeValueBy(1m);
    }
    
}