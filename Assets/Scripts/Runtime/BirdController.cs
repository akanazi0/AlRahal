using UnityEngine;
using UnityEngine.EventSystems;

namespace AlRahal
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public sealed class BirdController : MonoBehaviour
    {
        [Header("Glide tuning")]
        [SerializeField] private float easyFlapVelocity = 3.45f;
        [SerializeField] private float hardFlapVelocity = 4.1f;
        [SerializeField] private float easyGravityScale = 1.16f;
        [SerializeField] private float hardGravityScale = 1.42f;
        [SerializeField] private float maxFallVelocity = -4.4f;
        [SerializeField] private float maxRiseVelocity = 3.9f;
        [SerializeField] private float tiltSpeed = 9f;
        [SerializeField] private Vector2 startPosition = new Vector2(-3.25f, 0f);

        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private float flapVelocity;
        private float flapAnimationTime = -1f;
        private bool terminal;

        [SerializeField] private Sprite[] flightSprites;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            flapVelocity = easyFlapVelocity;
            body.gravityScale = easyGravityScale;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.simulated = false;
            SetFlightSprite(0);
        }

        private void Update()
        {
            GameManager game = GameManager.Instance;
            if (game == null || game.State != GameState.Playing || terminal) return;

            UpdateFlightAnimation();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                game.TogglePause();
                return;
            }

            if (Time.frameCount > game.InputBlockedThroughFrame && WantsFlap()) Flap();

            if (Mathf.Abs(body.position.y) > game.VerticalBoundary) game.EndRun();
        }

        private bool WantsFlap()
        {
            if (Input.GetKeyDown(KeyCode.Space)) return true;

            if (Input.GetMouseButtonDown(0) &&
                (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())) return true;

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began &&
                    (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject(touch.fingerId)))
                    return true;
            }
            return false;
        }

        private void Flap()
        {
            body.linearVelocity = new Vector2(0f, flapVelocity);
            flapAnimationTime = 0f;
            SetFlightSprite(1);
            SoundManager.Instance?.PlayFlap();
        }

        private void UpdateFlightAnimation()
        {
            if (flapAnimationTime < 0f) return;
            flapAnimationTime += Time.deltaTime;
            if (flapAnimationTime < 0.07f) return;
            if (flapAnimationTime < 0.14f) SetFlightSprite(2);
            else if (flapAnimationTime < 0.21f) SetFlightSprite(3);
            else if (flapAnimationTime < 0.28f) SetFlightSprite(5);
            else
            {
                flapAnimationTime = -1f;
                SetFlightSprite(0);
            }
        }

        private void SetFlightSprite(int frame)
        {
            if (spriteRenderer == null || flightSprites == null || flightSprites.Length == 0) return;
            spriteRenderer.sprite = flightSprites[frame < flightSprites.Length ? frame : 0];
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing || terminal) return;
            Vector2 velocity = body.linearVelocity;
            velocity.y = Mathf.Clamp(velocity.y, maxFallVelocity, maxRiseVelocity);
            body.linearVelocity = velocity;
            float targetTilt = Mathf.Lerp(-43f, 23f,
                Mathf.InverseLerp(maxFallVelocity, maxRiseVelocity, velocity.y));
            body.MoveRotation(Mathf.LerpAngle(body.rotation, targetTilt, tiltSpeed * Time.fixedDeltaTime));
        }

        public void BeginRun(DifficultyLevel level)
        {
            bool hard = level == DifficultyLevel.Hard;
            flapVelocity = hard ? hardFlapVelocity : easyFlapVelocity;
            body.gravityScale = hard ? hardGravityScale : easyGravityScale;
            terminal = false;
            spriteRenderer.enabled = true;
            flapAnimationTime = -1f;
            SetFlightSprite(0);
            body.simulated = false;
            transform.SetPositionAndRotation(startPosition, Quaternion.identity);
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.simulated = true;
        }

        public void StopMotion()
        {
            terminal = true;
            flapAnimationTime = -1f;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.simulated = false;
        }

        public void ResetToMenu()
        {
            StopMotion();
            transform.SetPositionAndRotation(startPosition, Quaternion.identity);
            SetFlightSprite(0);
            spriteRenderer.enabled = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (GameManager.Instance != null && GameManager.Instance.State == GameState.Playing)
                GameManager.Instance.EndRun();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;
            PillarMovement pair = other.GetComponentInParent<PillarMovement>();
            if (pair != null && other.CompareTag("ScoreGate")) pair.TryScore();
        }
    }
}
