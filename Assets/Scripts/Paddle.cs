using UnityEngine;

public class Paddle : MonoBehaviour
{
    #region Singleton
    private static Paddle _instance;
    public static Paddle Instance => _instance;
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public float velocity;
    public Camera cam;
    private float _limitOnX;
    public ParticleSystem leavesEffect;
    private AudioSource _audioSource;
    public TouchControls rightButton;
    public TouchControls leftButton;

    void Start()
    {
        if (Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            transform.localScale = new Vector3(transform.localScale.x * (float)1.7, transform.localScale.y * (float)1.7, 1);
        }
        _audioSource = this.GetComponent<AudioSource>();
        float platformWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        _limitOnX = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - platformWidth;
    }

    private void OnMouseDrag()
    {
        if (GameManager.IsPortrait)
        {
            velocity = 3;
        } else
        {
            velocity = 5;
        }
        if (!leftButton.isBeingPressed || !rightButton.isBeingPressed)
        {
            float FingerSpeed = Input.GetAxis("Mouse X") * velocity * Time.deltaTime;
            Vector3 position = transform.position;
            position.x += FingerSpeed;
            position.x = Mathf.Clamp(position.x, -_limitOnX, _limitOnX);
            transform.position = position;
        }  
    }

    void Update()
    {
        float MouseSpeed = Input.GetAxis("Mouse X") * velocity * Time.deltaTime;
        float KeysSpeed = Input.GetAxis("Horizontal") * (velocity * 2) * Time.deltaTime;
        if (leftButton.isBeingPressed)
        {
            velocity = 7;
            Vector3 position = transform.position;
            float posX = transform.position.x + (-1 * velocity * Time.deltaTime);
            position.x = Mathf.Clamp(posX, -_limitOnX, _limitOnX);
            transform.position = position;
        }
        else if (rightButton.isBeingPressed)
        {
            velocity = 7;
            Vector3 position = transform.position;
            float posX = transform.position.x + (1 * velocity * Time.deltaTime);
            position.x = Mathf.Clamp(posX, -_limitOnX, _limitOnX);
            transform.position = position;
        }
        else if (KeysSpeed != 0)
        {
            velocity = 4;
            Vector3 position = transform.position;
            position.x += KeysSpeed;
            position.x = Mathf.Clamp(position.x, -_limitOnX, _limitOnX);
            transform.position = position;
        }
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        else if (MouseSpeed != 0)
        {
            velocity = 4;
            Vector3 position = transform.position;
            position.x += MouseSpeed;
            //position.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            position.x = Mathf.Clamp(position.x, -_limitOnX, _limitOnX);
            transform.position = position;
        }
#endif
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            _audioSource.Play();
            Vector3 paddlePos = gameObject.transform.position;
            Vector3 spawnPos = new Vector3(paddlePos.x, paddlePos.y + 0.4f, paddlePos.z - 5.5f);
            GameObject effect = Instantiate(leavesEffect.gameObject, spawnPos, Quaternion.identity);
            effect.GetComponent<ParticleSystem>().Play();
            Destroy(effect, leavesEffect.main.startLifetime.constant);
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 paddleCenter = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
            ballRb.velocity = Vector2.zero;
            float difference = paddleCenter.x - hitPoint.x;
            if (hitPoint.x < paddleCenter.x)
            {
                ballRb.AddForce(new Vector2(-(Mathf.Abs(difference * 200)), BallsManager.Instance.initialBallSpeed));
            }
            else
            {
                ballRb.AddForce(new Vector2((Mathf.Abs(difference * 200)), BallsManager.Instance.initialBallSpeed));
            }
        }
    }
}