using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))]
public class CGCCameraFollow : MonoBehaviour
{   public GameObject Player;
    public float smoothSpeed = 0.05f;

    public Vector3 Offset = new Vector3(0,0,-10);
    

    void FixedUpdate()
    {
		if (!Player) { return; }
       Vector3 desiredPosition = Player.transform.position + Offset;
       Vector3 smoothedPosition = Vector3.Lerp(transform.position,desiredPosition,smoothSpeed);

        transform.position = smoothedPosition;
    }
}
