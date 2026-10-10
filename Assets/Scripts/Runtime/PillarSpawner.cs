using System.Collections.Generic;
using UnityEngine;

namespace AlRahal
{
    public sealed class PillarSpawner : MonoBehaviour
    {
        [SerializeField] private PillarMovement pillarPrefab;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private int initialPoolSize = 7;
        [SerializeField] private float firstSpawnDelay = 1.45f;

        private readonly Stack<PillarMovement> available = new Stack<PillarMovement>();
        private readonly List<PillarMovement> active = new List<PillarMovement>();
        private float timer;
        private float previousGapCenter;
        private bool running;

        private void Awake()
        {
            for (int i = 0; i < initialPoolSize; i++) available.Push(CreatePillar());
        }

        private PillarMovement CreatePillar()
        {
            PillarMovement pillar = Instantiate(pillarPrefab, transform);
            pillar.Initialize(this);
            pillar.gameObject.SetActive(false);
            return pillar;
        }

        public void BeginRun()
        {
            StopAndClear();
            previousGapCenter = 0f;
            timer = firstSpawnDelay;
            running = true;
        }

        private void Update()
        {
            if (!running || GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;
            timer -= Time.deltaTime;
            if (timer > 0f) return;

            DifficultySettings settings = GameManager.Instance.GetDifficulty(GameManager.Instance.Score);
            Spawn(settings);
            timer += settings.SpawnInterval;
            // Frame stalls cannot create stacked columns or bursts of spawns.
            if (timer < 0.15f) timer = settings.SpawnInterval;
        }

        private void Spawn(DifficultySettings settings)
        {
            float halfHeight = gameCamera.orthographicSize;
            float halfWidth = halfHeight * gameCamera.aspect;
            float safeLimit = Mathf.Min(1.75f,
                GameManager.Instance.VerticalBoundary - settings.Gap * 0.5f - 0.65f);
            float center = Mathf.Clamp(Random.Range(-safeLimit, safeLimit),
                previousGapCenter - settings.MaximumGapShift,
                previousGapCenter + settings.MaximumGapShift);
            previousGapCenter = center;

            PillarMovement pillar = available.Count > 0 ? available.Pop() : CreatePillar();
            pillar.Activate(halfWidth + 1.4f, center, settings.Gap, settings.Speed, -halfWidth);
            active.Add(pillar);
        }

        public void Release(PillarMovement pillar)
        {
            if (!active.Remove(pillar)) return;
            pillar.gameObject.SetActive(false);
            available.Push(pillar);
        }

        public void StopAndClear()
        {
            running = false;
            for (int i = active.Count - 1; i >= 0; i--) Release(active[i]);
        }
    }
}
