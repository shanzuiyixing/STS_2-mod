using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Assets;

namespace Test.Scripts.Orbs;

public class Grassorb : CustomOrbModel
{
    // 被动持续音效（宝珠在场时循环播放）
    protected override string PassiveSfx => "event:/sfx/characters/defect/defect_lightning_passive";
    // 触发宝珠时的音效
    protected override string EvokeSfx => "event:/sfx/characters/defect/defect_lightning_evoke";
    // 装备/召唤宝珠时的音效
    protected override string ChannelSfx => "event:/sfx/characters/defect/defect_lightning_channel";
    // 被动效果数值，ModifyOrbValue表示是否吃集中等
    public override decimal PassiveVal => ModifyOrbValue(1m);

    // 激发效果数值
    public override decimal EvokeVal => ModifyOrbValue(2m);

    // 暗色，使用球的主体色的暗色调
    public override Color DarkenedColor => new(0.1f, 0.2f, 0.5f);

    // 不出现在随机球池中
     public override bool IncludeInRandomPool => true;

    // 提示图标路径
    public override string? CustomIconPath => "res://icon.svg";
    // 球的场景的路径。如果你使用这个，你必须要有一个名称为SpineSkeleton并且是SpineSprite类型的节点
    // public override string? CustomSpritePath => "res://test/scenes/test_orb.tscn";

    // 可以继承这个并自行搭建场景，这样就没有上述限制。代码上优先使用这个
    public override Node2D? CreateCustomSprite()
    {
        return PreloadManager.Cache.GetScene("res://Test/scenes/test-grass_orb.tscn").Instantiate<Node2D>();
    }

    // 回合开始时触发被动
    public override async Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)
    {
        Trigger(); // 触发宝珠动画/状态
        await Passive(choiceContext, null);
    }

    // 触发被动
    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        Trigger();
        await CreatureCmd.Heal(base.Owner.Creature,PassiveVal);
    }

    // 触发激发，返回受影响的角色
    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext)
    {
        PlayEvokeSfx();
        await CreatureCmd.Heal(base.Owner.Creature,EvokeVal);
        return [Owner.Creature];
    }
}