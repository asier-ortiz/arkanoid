using UnityEngine;

public class BallsManager : MonoBehaviour
{
    #region Singleton
    private static BallsManager _instance;
    public static BallsManager Instance => _instance;
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

    [SerializeField]
    private Ball ballPrefab;
    private Ball initialBall;
    private Rigidbody2D initialBallRb;
    public float initialBallSpeed { get; set; } = 250;
    public Ball Ball { get; set; }

    private void Start()
    {
        InitBall();
    }

    private void Update()
    {
        if (!GameManager.Instance.isGameStarted)
        {
            Vector3 paddlePossition = Paddle.Instance.transform.position;
            Vector3 ballPosition = new Vector3(paddlePossition.x, paddlePossition.y + .27f, 0);
            initialBall.transform.position = ballPosition;
            if (GameManager.IsPortrait)
            {
                if (Input.GetMouseButtonUp(0) && !Paddle.Instance.rightButton.isBeingPressed && !Paddle.Instance.leftButton.isBeingPressed || Input.GetKeyDown("space"))
                {
                    initialBallRb.isKinematic = false;
                    initialBallRb.AddForce(new Vector2(0, initialBallSpeed));
                    GameManager.Instance.isGameStarted = true;
                }
            } else
            {
                if (Input.GetMouseButtonDown(0) && !Paddle.Instance.rightButton.isBeingPressed && !Paddle.Instance.leftButton.isBeingPressed || Input.GetKeyDown("space"))
                {
                    initialBallRb.isKinematic = false;
                    initialBallRb.AddForce(new Vector2(0, initialBallSpeed));
                    GameManager.Instance.isGameStarted = true;
                }
            }
        }
    }

    private void InitBall()
    {
        Vector3 paddlePossition = Paddle.Instance.transform.position;
        Vector3 startingPosition = new Vector3(paddlePossition.x, paddlePossition.y + .27f, 0);
        initialBall = Instantiate(ballPrefab, startingPosition, Quaternion.identity);
        initialBallRb = initialBall.GetComponent<Rigidbody2D>();
        this.Ball = initialBall;
    }

    public void ResetBalls()
    {
        Destroy(this.Ball.gameObject);
        InitBall();
    }
}