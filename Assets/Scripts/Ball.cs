using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static event Action<Ball> OnBallDeath;

    private void Start()
    {   
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            transform.localScale = new Vector3(transform.localScale.x * (float)1.7, transform.localScale.y * (float)1.7,1);
        }     
    }

    public void Die()
    {
        OnBallDeath?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.GetComponent<AudioSource>().Play();
    }
}