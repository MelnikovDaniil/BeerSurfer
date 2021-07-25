using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/ChillyPapperBuff")]
public class ChillyPapperBuff : ScriptableBuff
{
    public override string Name => "ChillyPepper";

    public float speedUpCoof = 2f;

    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedChillyPapperBuff(Duration, this, obj);
    }
}
