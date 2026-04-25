using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;


namespace Test.Scripts.Relics.KamiAncient;

[Pool(typeof(EventRelicPool))]

public class TurnAncient : CustomRelicModel
{
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Ancient;

    // 小图标
    public override string PackedIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_min.png";
    // 大图标
    protected override string BigIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_big.png";
    
    public  override async Task AfterCardDrawn(PlayerChoiceContext PlayerChoiceContext, CardModel card, bool fromHandDraw)
	{
        if (base.Owner.Creature.Player != card.Owner)
        {
            return;
        }
        Rng rewards = base.Owner.Creature.Player.PlayerRng.Rewards;
        int random = rewards.NextInt(3);
		if (random == 0)
        {
            Flash();
            await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 1m, base.Owner.Creature, null);
        }
        else if(random == 1)
        {
            Flash();
            await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, 1m, base.Owner.Creature, null);
        }
        else
        {
            Flash();
            await PlayerCmd.GainEnergy(1m,base.Owner);
        }
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext PlayerChoiceContext, CardPlay cardPlay)
    {
        // 只响应：减益拥有者自己打出的牌
        if (cardPlay.Card.Owner == base.Owner.Creature.Player)
        {
            CardType type = cardPlay.Card.Type;
            await PlayerCmd.LoseEnergy(1m, base.Owner);
            if (type == CardType.Attack)
            {
                // 攻击牌：力量 -1（静默不提示）
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, -1m, base.Owner.Creature, null, silent: true);
            }
            else if (type == CardType.Skill)
            {
                // 技能牌：敏捷 -1（静默不提示）
                await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, -1m, base.Owner.Creature, null, silent: true);
            }
        }
    }
}