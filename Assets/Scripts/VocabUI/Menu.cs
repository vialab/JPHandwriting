using UnityEngine;

public class Menu : VocabUI {
    private AudioSource _audioSource;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetClip(AudioClip clip) {
        _audioSource.clip = clip;
    }

    public void PlayPronunciationClip() {
        _audioSource.Play();
        LogEvent("Pronunciation clip played");
    }
}
