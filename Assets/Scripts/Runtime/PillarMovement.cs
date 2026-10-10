using UnityEngine;

namespace AlRahal
{
    public sealed class PillarMovement : MonoBehaviour
    {
        private const float HalfPillarHeight = 3f;

        [SerializeField] private Transform topPillar;
        [SerializeField] private Transform bottomPillar;
        [SerializeField] private BoxCollider2D scoreGate;

        private PillarSpawner owner;
        private float speed;
        private float recycleX;
        private bool scored;

        public void Initialize(PillarSpawner spawner) => owner = spawner;

        public void Activate(float x, float gapCenter, float gapHeight, float movementSpeed, float leftEdge)
        {
            scored = false;
            speed = movementSpeed;
            recycleX = leftEdge - 2f;
            transform.position = new Vector3(x, gapCenter, 0f);
            topPillar.localPosition = new Vector3(0f, gapHeight * 0.5f + HalfPillarHeight, 0f);
            bottomPillar.localPosition = new Vector3(0f, -gapHeight * 0.5f - HalfPillarHeight, 0f);
            scoreGate.size = new Vector2(0.12f, gapHeight);
            scoreGate.offset = Vector2.zero;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;
            transform.position += Vector3.left * (speed * Time.deltaTime);
            if (transform.position.x < recycleX) owner.Release(this);
        }

        public void TryScore()
        {
            if (scored || GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;
            scored = true;
            GameManager.Instance.AddScore();
        }
    }
}
