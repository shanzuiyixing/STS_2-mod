using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Commands;

namespace Test.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]

public class Gambling : CustomCardModel
{
    // 标准动态变量：定义卡牌核心数值（能量）
    protected override IEnumerable<DynamicVar>CanonicalVars => [
        new GoldVar(10)
    ];
    public override string PortraitPath => $"res://Test/images/cards/test-test_card.png";    

    // 构造函数：定义卡牌基础属性
    public Gambling ()
        : base(
            0,              // 卡牌费用：1费
            CardType.Skill, // 卡牌类型：技能牌
            CardRarity.Uncommon, // 卡牌稀有度：稀有
            TargetType.AllAllies)  // 目标类型：任意一名队友
    {
    }
	protected override PileType GetResultPileType()
	{
		PileType resultPileType = base.GetResultPileType();
		if (resultPileType != PileType.Discard)
		{
			return resultPileType;
		}
		return PileType.Hand;
	}
    // 卡牌被使用时执行
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 筛选所有存活的队友玩家
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
            where c != null && c.IsAlive && c.IsPlayer
            select c;
        Rng rewards = base.Owner.Creature.Player.PlayerRng.Rewards;
        int random = rewards.NextInt(100);
        // 2. 遍历每个队友，执行抽牌
        foreach (Creature item in enumerable)
        {
            if (item.Player.Gold < 10)
            {
                continue;
            }
            await PlayerCmd.LoseGold(base.DynamicVars.Gold.BaseValue,item.Player);
            if (random < 60)
            {

                await PlayerCmd.LoseGold(base.DynamicVars.Gold.BaseValue/2,item.Player);
            }
            else if (random < 80)
            {
                await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue + 5,item.Player);
            }
            else if (random < 90)
            {
                await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue *2,item.Player);
            }
            else if (random < 96)
            {
                await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue *3,item.Player);
            }
            else if (random < 99)
            {
                await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue *4,item.Player);
            }
            else
                {
                    await PlayerCmd.GainGold(999m,item.Player);
                }
        }
    }
    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		AddKeyword(CardKeyword.Innate);
    }
}