using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
	public float bgSpeed;
	public Renderer bgRend;
	private GameObject platform;

	void Update()
	{
		Camera cam = Camera.main;
		platform = GameObject.FindWithTag("Player");
		if (cam.WorldToScreenPoint(platform.transform.position).x > Screen.width / 2)
		{
			bgRend.material.mainTextureOffset += new Vector2(bgSpeed * Time.deltaTime, 0f);
		}
		else if (cam.WorldToScreenPoint(platform.transform.position).x < Screen.width / 2)
		{
			bgRend.material.mainTextureOffset += new Vector2(-bgSpeed * Time.deltaTime, 0f);
		}
	}
}