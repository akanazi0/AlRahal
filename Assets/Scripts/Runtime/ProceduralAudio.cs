using System;
using UnityEngine;

namespace AlRahal
{
    public static class ProceduralAudio
    {
        private const int SampleRate = 44100;

        public static AudioClip CreateFlap()
        {
            return Build("Cape / soft air", 0.19f, (t, rng) =>
            {
                float envelope = Mathf.Sin(Mathf.PI * t / 0.19f);
                float flutter = Mathf.Sin(2f * Mathf.PI * (190f * t + 420f * t * t));
                return envelope * (0.22f * flutter + 0.10f * Noise(rng));
            });
        }

        public static AudioClip CreateImpact()
        {
            return Build("Stone / glass impact", 0.58f, (t, rng) =>
            {
                float stone = Noise(rng) * Mathf.Exp(-20f * t) * 0.43f;
                float chime = Mathf.Sin(2f * Mathf.PI * 620f * t) * Mathf.Exp(-7.2f * t) * 0.19f;
                float overtone = Mathf.Sin(2f * Mathf.PI * 932f * t) * Mathf.Exp(-11f * t) * 0.10f;
                return stone + chime + overtone;
            });
        }

        public static AudioClip CreateScore()
        {
            return Build("Arch / passing chime", 0.38f, (t, rng) =>
            {
                float tone = t < 0.12f ? 523.25f : 783.99f;
                float local = t < 0.12f ? t : t - 0.12f;
                float envelope = Mathf.Exp(-10f * local) * Mathf.Min(1f, local * 90f);
                return Mathf.Sin(2f * Mathf.PI * tone * local) * envelope * 0.22f;
            });
        }

        public static AudioClip CreateSelect()
        {
            return Build("Glass / select", 0.16f, (t, rng) =>
                Mathf.Sin(2f * Mathf.PI * (540f + 950f * t) * t) * Mathf.Exp(-18f * t) * 0.17f);
        }

        private static AudioClip Build(string name, float seconds, Func<float, System.Random, float> signal)
        {
            int count = Mathf.CeilToInt(seconds * SampleRate);
            float[] samples = new float[count];
            System.Random rng = new System.Random(24817);
            for (int i = 0; i < count; i++) samples[i] = Mathf.Clamp(signal(i / (float)SampleRate, rng), -1f, 1f);
            AudioClip clip = AudioClip.Create(name, count, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static float Noise(System.Random rng) => (float)rng.NextDouble() * 2f - 1f;
    }
}
