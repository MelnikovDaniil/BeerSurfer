using UnityEngine;

public abstract class ScriptableBuff : ScriptableObject
{
    public virtual string Name { get; }

    public Sprite sprite;

    public float duration;

    public AudioClip buffSound;

    public abstract TimedBuff InitializeBuff(GameObject obj);
}
