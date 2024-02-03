using UnityEngine;

public class ScreenCollider : MonoBehaviour
{
    #region Singleton
    private static ScreenCollider _instance;
    public static ScreenCollider Instance => _instance;
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

    public Camera cam;
    public new EdgeCollider2D collider;

    void Start()
    {
        LoadScreenCollider();
    }

    public void LoadScreenCollider()
    {
        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperLeft = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        Vector3 upperRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector3 bottomRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        collider.points = new Vector2[5] { bottomLeft, upperLeft, upperRight, bottomRight, bottomLeft };
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        if (normal == Vector2.down && collision.collider.CompareTag("Ball")) 
        {
            Ball ball = collision.collider.GetComponent<Ball>();
            ball.Die();
        }
    }
}
