using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
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
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";     

    // 构造函数：定义卡牌基础属性
    public Gambling ()
        : base(
            0,              // 卡牌费用：1费
            CardType.Skill, // 卡牌类型：技能牌
            CardRarity.Uncommon, // 卡牌稀有度：稀有
            TargetType.Self)  // 目标类型：任意一名队友
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
        Rng rewards = Owner.Creature.Player.PlayerRng.Rewards;
        int random = rewards.NextInt(100);
        
            if (base.Owner.Creature.Player.Gold < 10)
            {
                return;
            }
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
            await PlayerCmd.LoseGold(base.DynamicVars.Gold.BaseValue,base.Owner.Creature.Player);
            if (random < 50)
            {

                await PlayerCmd.LoseGold(base.DynamicVars.Gold.BaseValue/2,base.Owner.Creature.Player);
            }
            else if (random < 80)
            {
                await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue + 5,base.Owner.Creature.Player);
            }
            else if (random < 90)
            {
                await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue *2,base.Owner.Creature.Player);
            }
            else if (random < 96)
            {
                await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue *3,base.Owner.Creature.Player);
            }
            else if (random < 99)
            {
                await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue *4,base.Owner.Creature.Player);
            }
            else
                {
                    await PlayerCmd.GainGold(999m,base.Owner.Creature.Player);
                }
    }
    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		AddKeyword(CardKeyword.Innate);
    }
}