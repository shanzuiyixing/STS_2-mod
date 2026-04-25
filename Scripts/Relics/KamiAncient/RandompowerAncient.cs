using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Test.Scripts.Cards;
using MegaCrit.Sts2.Core.HoverTips;

namespace Test.Scripts.Relics.KamiAncient;

[Pool(typeof(EventRelicPool))]

public class RandompowerAncient : CustomRelicModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<Randompower>();
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Ancient;

    // 小图标
    public override string PackedIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_min.png";
    // 大图标
    protected override string BigIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_big.png";

    public override async Task AfterObtained()
	{
        CardModel card = base.Owner.RunState.CreateCard<Randompower>(base.Owner);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck),2f);
	}
}