using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Test.Scripts.Relics.KamiAncient;
namespace Test.Scripts.Ancients;

public class KamiAncient : CustomAncientModel
{
    // 选项按钮颜色
    public override Color ButtonColor => new(0.12f, 0.2f, 0.8f, 0.5f);
    // 对话框颜色
    public override Color DialogueColor => new(0.12f, 0.2f, 0.8f);

    // 出现条件。这里是只能在第二幕出现
    public override bool IsValidForAct(ActModel act) => act.ActNumber() == 2 || act.ActNumber() == 3;
    // 自定义场景的路径
    public override string? CustomScenePath => "res://Test/scenes/test-kami_ancient.tscn";
    // 自定义地图图标和轮廓的路径
    public override string? CustomMapIconPath => "res://icon.svg";
    public override string? CustomMapIconOutlinePath => "res://icon.svg";
    // 历史记录图标路径
    public override string? CustomRunHistoryIconPath => "res://icon.svg";
    public override string? CustomRunHistoryIconOutlinePath => "res://icon.svg";

    // 生成选项。每个选项有自己的池子。
    protected override OptionPools MakeOptionPools { get; } = new OptionPools(
        MakePool(
            AncientOption<AbsorbAncient>(),
            AncientOption<LotterydrawAncient>(),
            AncientOption<Turnplayback>()
        ),
        MakePool(
            AncientOption<TurnAncient>(),
            AncientOption<RandompowerAncient>(),
            AncientOption<NinelivesRelic>()
        ),
        MakePool(
            AncientOption<PlaytodrawRelic>(),
            AncientOption<LifeswapRelic>()
        )
    );
}