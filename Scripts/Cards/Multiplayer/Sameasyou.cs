using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Test.Scripts.Cards.Multiplayer;
using MegaCrit.Sts2.Core.Random;

[Pool(typeof(ColorlessCardPool))]

public class Sameasyou : CustomCardModel
{

    // 标准动态变量：定义卡牌核心数值（能量）
    protected override IEnumerable<DynamicVar>CanonicalVars => [

    ];
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";    

    // 核心约束：仅限多人模式使用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 构造函数：定义卡牌基础属性
    public Sameasyou ()
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
        IReadOnlyList<PowerModel> allPowers  =
            cardPlay.Target.Powers;
        if (allPowers == null || allPowers.Count == 0)
        {

            return;
        }
        Rng rng = Owner.Creature.Player.PlayerRng.Rewards;
        int randomPowernum = rng.NextInt(allPowers.Count);
        PowerModel randomPower = allPowers[randomPowernum];
        PowerModel canonicalPowerid = ModelDb.GetById<PowerModel>(randomPower.Id);
        PowerModel newPowerInstance = canonicalPowerid.ToMutable(canonicalPowerid.Amount);
        await PowerCmd.Apply(newPowerInstance,base.Owner.Creature, randomPower.Amount, cardPlay.Target.Player.Creature, this);
    }

    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		AddKeyword(CardKeyword.Retain);
    }
}