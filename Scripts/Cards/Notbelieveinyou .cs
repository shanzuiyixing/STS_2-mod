using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Test.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]

public class Notbelieveinyou : CustomCardModel
{
    // 额外悬浮提示：显示基础的能量消耗提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Energy)];

    // 标准动态变量：定义卡牌核心数值（能量）
    protected override IEnumerable<DynamicVar>CanonicalVars => [];
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";    

    // 核心约束：仅限多人模式使用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 构造函数：定义卡牌基础属性
    public Notbelieveinyou ()
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
        var targetPlayer = cardPlay.Target.Player;
        var selfPlayer = base.Owner;
        int targetEnergy = targetPlayer.PlayerCombatState.Energy;
        int selfEnergy = 2 * targetEnergy;

        // 核心效果：给目标队友 获得 卡牌定义的能量值
        await PlayerCmd.LoseEnergy(targetEnergy,targetPlayer);
        await PlayerCmd.GainEnergy(selfEnergy,selfPlayer);
    }

    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		base.EnergyCost.UpgradeBy(-1);
    }
}