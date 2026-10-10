using UnityEngine;

namespace AlRahal
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private AudioSource source;
        private AudioClip flap;
        private AudioClip impact;
        private AudioClip score;
        private AudioClip select;

        private void Awake()
        {
            Instance = this;
            source = GetComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            flap = ProceduralAudio.CreateFlap();
            impact = ProceduralAudio.CreateImpact();
            score = ProceduralAudio.CreateScore();
            select = ProceduralAudio.CreateSelect();
        }

        public void PlayFlap() => source.PlayOneShot(flap, 0.58f);
        public void PlayImpact() => source.PlayOneShot(impact, 0.75f);
        public void PlayScore() => source.PlayOneShot(score, 0.75f);
        public void PlaySelect() => source.PlayOneShot(select, 0.55f);
        public void PlayStart() => source.PlayOneShot(score, 0.38f);

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (flap != null) Destroy(flap);
            if (impact != null) Destroy(impact);
            if (score != null) Destroy(score);
            if (select != null) Destroy(select);
        }
    }
}
