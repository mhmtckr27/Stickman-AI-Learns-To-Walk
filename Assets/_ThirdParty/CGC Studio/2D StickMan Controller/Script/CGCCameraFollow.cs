using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))]
public class CGCCameraFollow : MonoBehaviour
{
    [SerializeField] private List<GameObject> backgrounds;
    [SerializeField] private float backgroundXOffset;

    public GameObject Player;
    public float smoothSpeed = 0.05f;

    public Vector3 Offset = new Vector3(0, 0, -10);

	void LateUpdate()
    {
        if (!Player) { return; }
        Vector3 desiredPosition = Player.transform.position + Offset;
        desiredPosition.y = transform.position.y;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;

        if (transform.position.x > backgrounds[2].transform.position.x - backgroundXOffset / 2)
        {
            GameObject background = backgrounds[0];
            Vector3 newPos = background.transform.position;
            newPos.x = backgrounds[2].transform.position.x + backgroundXOffset;
            background.transform.position = newPos;
            backgrounds.RemoveAt(0);
            backgrounds.Insert(2, background);
        }
        else if (transform.position.x < backgrounds[0].transform.position.x + backgroundXOffset / 2)
		{
            GameObject background = backgrounds[2];
            Vector3 newPos = background.transform.position;
            newPos.x = backgrounds[0].transform.position.x - backgroundXOffset;
            background.transform.position = newPos;
            backgrounds.RemoveAt(2);
            backgrounds.Insert(0, background);
        }
    }
}
