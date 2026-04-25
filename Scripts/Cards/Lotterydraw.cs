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

public class Lotterydraw : CustomCardModel
{
    // 标准动态变量：定义卡牌核心数值（能量）
    protected override IEnumerable<DynamicVar>CanonicalVars => [
        new BlockVar(10m,ValueProp.Unpowered),
        new DamageVar(3,ValueProp.Unblockable),
        new GoldVar(100),
        new HealVar(5),
        new MaxHpVar(1),
        new PowerVar<IntangiblePower>(1m)
    ];
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";     

    // 核心约束：仅限多人模式使用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 构造函数：定义卡牌基础属性
    public Lotterydraw ()
        : base(
            1,              // 卡牌费用：1费
            CardType.Skill, // 卡牌类型：技能牌
            CardRarity.Uncommon, // 卡牌稀有度：稀有
            TargetType.AllAllies)  // 目标类型：任意一名队友
    {
    }
    // 卡牌被使用时执行
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 筛选所有存活的队友玩家
#pragma warning disable CS8602 // 解引用可能出现空引用。
        IEnumerable<Creature> enumerable = from c in CombatState.GetTeammatesOf(base.Owner.Creature)
            where c != null && c.IsAlive && c.IsPlayer
            select c;
#pragma warning restore CS8602 // 解引用可能出现空引用。
        Rng rewards = Owner.Creature.Player.PlayerRng.Rewards;
        int random = rewards.NextInt(100);
        // 2. 遍历每个队友
        foreach (Creature item in enumerable)
        {
            if (random < 50)
            {
                await CreatureCmd.GainBlock(item,base.DynamicVars.Block.BaseValue, ValueProp.Unpowered, null);
            }
            else if (random < 80)
            {
                await CreatureCmd.Damage(choiceContext,item,base.DynamicVars.Damage.BaseValue,ValueProp.Unblockable,this);
            }
            else if (random < 90)
            {
                await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue, item.Player);
            }
            else if (random < 96)
            {
                await CreatureCmd.Heal(item,base.DynamicVars.Heal.BaseValue);
            }
            else if (random < 99)
            {
                await CreatureCmd.GainMaxHp(item,base.DynamicVars.MaxHp.BaseValue);
            }
            else
                {
                    await PowerCmd.Apply<IntangiblePower>(item, base.DynamicVars["IntangiblePower"].BaseValue, base.Owner.Creature, this);
                }
        }
    }
    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		base.EnergyCost.UpgradeBy(-1);
    }
}