using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorderView : MonoBehaviour
{
    private Animator _animator;
    private bool muted;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.SetMusicMuted(false);
        SoundManager.SetSoundMuted(false);
        var volume = SoundManager.GetMusicVolume();
        muted = volume == 0;
        _animator.SetBool("muted", muted);
    }

    private void OnMouseDown()
    {
        muted = !muted;
        var volume = muted ? 0 : 100f;
        _animator.SetBool("muted", muted);
        SoundManager.SetMusicVolume(volume);
        SoundManager.SetSoundVolume(volume);
    }
}
