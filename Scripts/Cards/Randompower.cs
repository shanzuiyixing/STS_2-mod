using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Commands;

namespace Test.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]

public class Randompower : CustomCardModel
{
    // 标准动态变量：定义卡牌核心数值（能量）
    protected override IEnumerable<DynamicVar>CanonicalVars => [
        new PowerVar<FreePowerPower>(1m),
        new PowerVar<GigantificationPower>(1m),
        new PowerVar<ConfusedPower>(1m),
        new PowerVar<BufferPower>(1m),
        new PowerVar<ConstrictPower>(3m),
        new PowerVar<BlurPower>(1m),
        new PowerVar<SubroutinePower>(1m),
        new PowerVar<RitualPower>(1m),
        new PowerVar<EchoFormPower>(1m),
        new PowerVar<MindRotPower>(1m)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override string PortraitPath => $"res://Test/images/cards/test-test_card.png";    

    // 核心约束：仅限多人模式使用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 构造函数：定义卡牌基础属性
    public Randompower ()
        : base(
            3,              // 卡牌费用：1费
            CardType.Skill, // 卡牌类型：技能牌
            CardRarity.Uncommon, // 卡牌稀有度：稀有
            TargetType.AllAllies)  // 目标类型：任意一名队友
    {
    }
    // 卡牌被使用时执行
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 筛选所有存活的队友玩家
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
            where c != null && c.IsAlive && c.IsPlayer
            select c;
        Rng rewards = base.Owner.Creature.Player.PlayerRng.Rewards;
        // 2. 遍历每个队友，执行
        foreach (Creature item in enumerable)
        {
            Rng rng = Owner.Creature.Player.PlayerRng.Rewards;
            int random = rng.NextInt(10); // 9种能力，随机0-8
        switch (random)
        {
            case 0: await PowerCmd.Apply<FreePowerPower>(item, 1m, Owner.Creature, this); break;
            case 1: await PowerCmd.Apply<GigantificationPower>(item, 1m, Owner.Creature, this); break;
            case 2: await PowerCmd.Apply<ConfusedPower>(item, 3m, Owner.Creature, this); break;
            case 3: await PowerCmd.Apply<BufferPower>(item, 1m, Owner.Creature, this); break;
            case 4: await PowerCmd.Apply<ConstrictPower>(item, 1m, Owner.Creature, this); break;
            case 5: await PowerCmd.Apply<BlurPower>(item, 1m, Owner.Creature, this); break;
            case 6: await PowerCmd.Apply<SubroutinePower>(item, 1m, Owner.Creature, this); break;
            case 7: await PowerCmd.Apply<RitualPower>(item, 1m, Owner.Creature, this); break;
            case 8: await PowerCmd.Apply<EchoFormPower>(item, 1m, Owner.Creature, this); break;
            case 9: await PowerCmd.Apply<MindRotPower>(item, 1m, Owner.Creature, this); break;
        }
        }
    }
    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		base.EnergyCost.UpgradeBy(-1);
    }
}