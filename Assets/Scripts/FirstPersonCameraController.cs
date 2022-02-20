using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offsetGround;
	public float groundDistance = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
		//offsetGround = transform.up * groundDistance;
    }

    // Update is called once per frame
    void LateUpdate()
    {
		//transform.position = player.transform.position + (transform.up * groundDistance);
		transform.forward = player.transform.forward;
    }
}
