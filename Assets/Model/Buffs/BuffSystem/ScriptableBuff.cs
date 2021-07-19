using UnityEngine;

public abstract class ScriptableBuff : ScriptableObject
{
    public virtual string Name { get; }

    public float Duration;

    public AudioClip buffSound;

    public abstract TimedBuff InitializeBuff(GameObject obj);
}
