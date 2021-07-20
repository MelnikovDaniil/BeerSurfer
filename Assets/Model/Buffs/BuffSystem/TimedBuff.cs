using System;
using UnityEngine;

public abstract class TimedBuff
{
    public ScriptableBuff Buff;
    public bool IsFinished;

    protected float Duration;
    protected GameObject obj;

    public TimedBuff(float duration, ScriptableBuff buff, GameObject obj)
    {
        this.Duration = duration;
        this.Buff = buff;
        this.obj = obj;
        SoundManager.PlaySoundUI(buff.buffSound);
    }

    protected TimedBuff(ScriptableBuff buff, GameObject obj)
    {
        Buff = buff;
        this.obj = obj;
    }

    public void Tick(float delta)
    {
        Duration -= delta;
        if (Duration <= 0)
        {
            End();
            IsFinished = true;
        }
    }

    public void Update()
    {
        Duration = Buff.Duration;
    }

    public abstract void Activate();
    public abstract void End();
}
