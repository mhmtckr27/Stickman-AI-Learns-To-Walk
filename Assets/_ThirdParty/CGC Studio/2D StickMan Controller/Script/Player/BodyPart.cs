using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
	public PlayerDead Player;
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == Player.DeadTag)
			Player.isDead = true;

	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		/*if(name == "Head")
		{
			if(transform.position.y < -3.5f)
			{
				Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.25f);
				foreach(Collider2D collider in colliders)
				{
					if(collider.tag == "Ground")
					{
						Player.GetComponent<StickManController>().Individual.fitness -= 1000f;
						Player.GetComponent<StickManController>().Individual.isDead = true;
					}
				}
			}
		}*/
	}
}
