using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization;

namespace Test.Scripts.Cards.Multiplayer;

[Pool(typeof(ColorlessCardPool))]

public class Statustofriend : CustomCardModel
{
    
    public override string PortraitPath => $"res://Test/images/cards/{Id.Entry.ToLowerInvariant()}.png";    

    // 核心约束：仅限多人模式使用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 构造函数：定义卡牌基础属性
    public Statustofriend ()
        : base(
            0,              // 卡牌费用：0费
            CardType.Skill, // 卡牌类型：技能牌
            CardRarity.Uncommon, // 卡牌稀有度：稀有
            TargetType.AnyAlly)  // 目标类型：任意一名队友
    {
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 校验：必须选择目标队友（为空则报错）
ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
    // 播放角色施法动画
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        List<CardModel> list = GetStatuses(base.Owner).ToList();
		foreach (CardModel item in list)
		{
            CardModel canonicalCard = ModelDb.GetById<CardModel>(item.Id);
            CardModel clonedstatus = 
cardPlay.Target.Player.Creature.CombatState
.CreateCard(
        canonicalCard,
        cardPlay.Target.Player
    );
            await CardPileCmd.AddGeneratedCardToCombat(clonedstatus,newPileType: PileType.Discard,addedByPlayer:true);
			await CardCmd.Exhaust(choiceContext, item);
		}
    }

    // 卡牌升级时执行
    protected override void OnUpgrade()
	{
		AddKeyword(CardKeyword.Retain);
    }

    private static IEnumerable<CardModel> GetStatuses(Player owner)
	{
		return owner.PlayerCombatState.AllCards.Where((CardModel c) => c.Type == CardType.Status && c.Pile.Type != PileType.Exhaust);
	}
}