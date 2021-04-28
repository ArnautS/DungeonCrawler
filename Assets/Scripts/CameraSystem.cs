using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
	private GameObject player;
	[SerializeField]
	private float xMin;
	[SerializeField]
	private float xMax;
	[SerializeField]
	private float yMin;
	[SerializeField]
	private float yMax;


	// Start is called before the first frame update
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void LateUpdate()
	{
		// Camera controls, change to metroidvania tutorial if necessary. 
		float x = Mathf.Clamp(player.transform.position.x, xMin, xMax);
		float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
		gameObject.transform.position = new Vector3(x, y, gameObject.transform.position.z);
	}
}
