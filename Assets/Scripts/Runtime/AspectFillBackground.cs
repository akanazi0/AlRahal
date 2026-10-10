using UnityEngine;

namespace AlRahal
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class AspectFillBackground : MonoBehaviour
    {
        private SpriteRenderer sky;

        private void Awake() => sky = GetComponent<SpriteRenderer>();

        private void LateUpdate()
        {
            Camera camera = Camera.main;
            if (camera == null || sky.sprite == null) return;

            float visibleHeight = camera.orthographicSize * 2f;
            float visibleWidth = visibleHeight * camera.aspect;
            Vector2 spriteSize = sky.sprite.bounds.size;
            float scale = Mathf.Max(visibleWidth / spriteSize.x, visibleHeight / spriteSize.y);
            transform.localScale = new Vector3(scale, scale, 1f);
            transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, transform.position.z);
        }
    }
}
