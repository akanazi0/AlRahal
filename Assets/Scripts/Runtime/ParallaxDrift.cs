using UnityEngine;

namespace AlRahal
{
    public sealed class ParallaxDrift : MonoBehaviour
    {
        [SerializeField] private float speed = 0.16f;
        [SerializeField] private float wrapDistance = 22f;
        private float startX;

        private void Awake() => startX = transform.position.x;

        private void Update()
        {
            float multiplier = GameManager.Instance != null && GameManager.Instance.State == GameState.Playing ? 1f : 0.35f;
            transform.position += Vector3.left * (speed * multiplier * Time.unscaledDeltaTime);
            if (transform.position.x < startX - wrapDistance)
                transform.position += Vector3.right * wrapDistance;
        }
    }
}
