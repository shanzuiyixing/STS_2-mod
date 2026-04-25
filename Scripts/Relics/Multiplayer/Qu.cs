using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Multiplayer.Game;

namespace Test.Scripts.Relics.Multiplayer;

[Pool(typeof(SharedRelicPool))]

public class Qu : CustomRelicModel
{
    public override bool IsAllowed(IRunState runState)
    {
        // 只有多人模式允许生成，单机返回false（不生成）
        return IsMultiplayerGame();;
    }
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Common;

    // 遗物的数值。替换本地化中的{Cards}。
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(1m),
        ];
    
    // 小图标
    public override string PackedIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_min.png";
    // 大图标
    protected override string BigIconPath => $"res://Test/images/relics/{Id.Entry.ToLowerInvariant()}_big.png";

    private bool _wasUsed = false;

     public override async Task AfterPreventingDeath(Creature creature){
        {
            Flash();
        if(_wasUsed)
        await CreatureCmd.Heal(base.Owner.Creature,base.DynamicVars.Heal.BaseValue);
        }
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
	{
		if (player != base.Owner)
		{
			return count;
		}
        if (!_wasUsed)
        {
            return count;
        }
		return 1;
	}

    public override bool ShouldDieLate(Creature creature)
	{
		if (creature != base.Owner.Creature)
		{
			return true;
		}
		else if (!IsAllyAlive())
		{
			return true;
		}
        _wasUsed = true;
		return false;
	}

    private bool IsAllyAlive()
    {
        IEnumerable<Creature> enumerable = base.Owner.Creature.CombatState.GetTeammatesOf(base.Owner.Creature);
        if (enumerable == null) return false;
        foreach (Creature item in enumerable)
        {
            if (item.IsAlive && item.IsPlayer)
            {
                return true;
            }
        }     
        return false;
    }

    // 静态工具方法：判断当前是否为【多人游戏】
    public static bool IsMultiplayerGame()
    {
        var netService = RunManager.Instance?.NetService;
        // 满足：网络服务存在 + 已连接 + 不是单人/回放模式
        return netService != null && netService.IsConnected && 
               netService.Type != NetGameType.Singleplayer &&
               netService.Type != NetGameType.Replay;
    }
}
       