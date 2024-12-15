using UnityEngine;

public class VocabInfo : VocabUI {
    [SerializeField] private GameObject tracingEnabledMarker;
    private AudioSource _audioSource;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetClip(AudioClip clip) {
        _audioSource.clip = clip;
    }

    public void SetTracingIndicator(bool tracingEnabled) {
        tracingEnabledMarker.SetActive(tracingEnabled);
    }

    public void PlayPronunciationClip() {
        _audioSource.Play();
        LogEvent("Pronunciation clip played");
    }
}
