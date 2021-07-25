using System;
using UnityEngine;

public class TimedBitBuff : TimedBuff
{
    private readonly BitBuff bitBuff;
    private readonly Character character;

    public TimedBitBuff(float duration, ScriptableBuff buff, GameObject obj) : base(duration, buff, obj)
    {
        bitBuff = (BitBuff)buff;
        character = obj.GetComponent<Character>();
    }

    public override void Activate()
    {
        character.ActivateBit();
    }

    public override void End()
    {
        character.DeactivateBit();
    }
}

