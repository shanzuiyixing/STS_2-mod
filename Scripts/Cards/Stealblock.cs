using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;


namespace Test.Scripts.Cards;

[Pool(typeof(CurseCardPool))]

public class Stealblock : CustomCardModel
{
    // 标准动态变量：定义卡牌核心数值（能量）
    protected override IEnumerable<DynamicVar>CanonicalVars => [
        new BlockVar(10m,ValueProp.Unpowered),
        new DynamicVar("Blockindex", 2m),
        new CalculationBaseVar(0m),
		new CalculationExtraVar(1m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? target) => target?.Block ?? 0)
    ];
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";    

    // 核心约束：仅限多人模式使用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 构造函数：定义卡牌基础属性
    public Stealblock ()
        : base(
            1,              // 卡牌费用：0费
            CardType.Skill, // 卡牌类型：技能牌
            CardRarity.Uncommon, // 卡牌稀有度：稀有
            TargetType.AnyAlly)  
    {
    }
    // 卡牌被使用时执行
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.CalculatedBlock.Calculate(cardPlay.Target) * base.DynamicVars["Blockindex"].BaseValue, base.DynamicVars.CalculatedBlock.Props, cardPlay);
        await CreatureCmd.LoseBlock(cardPlay.Target, base.DynamicVars.CalculatedBlock.Calculate(cardPlay.Target));

    }

    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		base.DynamicVars["Blockindex"].UpgradeValueBy(3m);
    }
    
}