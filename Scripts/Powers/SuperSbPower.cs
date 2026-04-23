using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models.Powers;


namespace Test.Scripts.Powers;

public class SuperSbPower : CustomPowerModel
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;
    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;
    // 自定义图标路径，自己指定，或者创建一个基类来统一指定图标路径
    public override string? CustomPackedIconPath => $"res://Test/images/powers/{Id.Entry.ToLowerInvariant()}_small.png";
    public override string? CustomBigIconPath => $"res://Test/images/powers/{Id.Entry.ToLowerInvariant()}_big.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        ];

     public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side)
		{
			Flash(); // BUFF闪光特效
			// 获得格挡 = 层数，固定数值（不受任何加成）        
            int PoisonNum = base.Owner.GetPower<PoisonPower>()?.Amount??0;
			await PowerCmd.Apply<StrengthPower>(base.Owner, PoisonNum, base.Owner, null);

		}
    }

}