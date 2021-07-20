using System;
using UnityEngine;

public class TimedChillyPapperBuff : TimedBuff
{
    private readonly ChillyPapperBuff chillyPapperBuff;
    private readonly Character character;

    public TimedChillyPapperBuff(float duration, ScriptableBuff buff, GameObject obj) : base(duration, buff, obj)
    {
        chillyPapperBuff = (ChillyPapperBuff)buff;
        character = obj.GetComponent<Character>();
    }
    public override void Activate()
    {
        LocationGenerator.enableObstacleSpawn = false;
        character.enableMovementActions = false;
        character.animator.SetBool("burn", true);
        Time.timeScale *= chillyPapperBuff.speedUpCoof;
    }

    public override void End()
    {
        LocationGenerator.enableObstacleSpawn = true;
        character.enableMovementActions = true;
        character.animator.SetBool("burn", false);
        Time.timeScale /= chillyPapperBuff.speedUpCoof;
    }
}
    
