using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
namespace Test.Scripts.Events;

public sealed class ColorlessEvent : CustomEventModel
{
    public override bool IsShared => true;
    // 背景图位置
    public override string? CustomInitialPortraitPath => "res://Test/images/events/Demon.png";

    // 设置一些数值
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Unblockable | ValueProp.Unpowered),
        new GoldVar(10)
    ];

    // 什么时候会遇到。这里的条件是所有玩家的金币都大于等于60
    public override bool IsAllowed(IRunState runState) 
    {
        // 核心：只有联机模式才允许生成这个事件
        if(RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
        return false;
        // 原有条件：所有玩家金币≥10
        return runState.Players.All(p => p.Gold >= DynamicVars.Gold.BaseValue);
    }

    // 事件开始前的逻辑。这里是禁止玩家移除药水
    protected override Task BeforeEventStarted(bool isPreFinished)
    {
        Owner!.CanRemovePotions = false;
        return Task.CompletedTask;
    }

    // 事件结束后的逻辑。这里是允许玩家移除药水
    protected override void OnEventFinished()
    {
        Owner!.CanRemovePotions = true;
    }

    // 生成事件初始选项。这里是两个选项：失去生命值或者失去金币，然后进入选择奖励阶段
    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(TakeDamage),
        Option(LoseGold),
    ];

    // 失去生命
    private async Task TakeDamage()
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner!.Creature, DynamicVars.Damage, null, null);
        ChooseRewardTypePage();
    }

    // 失去金币
    private async Task LoseGold()
    {
        await PlayerCmd.LoseGold(DynamicVars.Gold.BaseValue, Owner!, GoldLossType.Stolen);
        ChooseRewardTypePage();
    }

    // 进入事件第二阶段，两个选项：选择药水或者选择卡牌
    private void ChooseRewardTypePage()
    {
        SetEventState(PageDescription("CHOOSE_TYPE"), [
            Option(ChoosePotions, "CHOOSE_TYPE"), // 第二个参数代表该选项所在页面
            Option(ChooseCards, "CHOOSE_TYPE"),
        ]);
    }

    // 选择药水奖励，然后结束事件
    private async Task ChoosePotions()
    {
        await RewardsCmd.OfferCustom(Owner!, [new PotionReward(Owner!)]);
        SetEventFinished(PageDescription("POTIONS_CHOSEN")); // 结束事件时调用这个
    }

    // 选择卡牌奖励，然后结束事件
    private async Task ChooseCards()
    {
        CardCreationOptions onlineCardOptions = CardCreationOptions.ForNonCombatWithDefaultOdds(
            new List<CardPoolModel> { ModelDb.CardPool<ColorlessCardPool>() },
            DeckFilter // 关键：传入联机卡牌过滤条件
        );

        await RewardsCmd.OfferCustom(
            Owner!, 
        [new CardReward
        (onlineCardOptions ,3, Owner)]);
SetEventFinished(PageDescription("CARDS_CHOSEN"));
    }

    private bool DeckFilter(CardModel card)
    {
        return card.MultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly;
    }
}