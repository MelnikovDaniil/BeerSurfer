using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/BitBuff")]
public class BitBuff : ScriptableBuff
{
    public override string Name => "Bat";
    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedBitBuff(Duration, this, obj);
    }
}
