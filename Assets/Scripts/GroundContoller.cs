using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundContoller : MonoBehaviour
{

	private void OnCollisionEnter2D(Collision2D collision)
	{
		/*if(collision.collider.name == "Head")
		{
			Debug.LogError(collision.collider.name);
			collision.collider.transform.root.GetComponent<StickManController>().Individual.fitness -= 1000f;
			collision.collider.transform.root.GetComponent<StickManController>().Individual.isDead = true;
		}*/
	}
}
