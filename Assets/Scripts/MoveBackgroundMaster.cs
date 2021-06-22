using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackgroundMaster : MonoBehaviour
{
	MoveBackground[] moveBackgroundScripts;

	private void Awake()
	{
		moveBackgroundScripts = GetComponentsInChildren<MoveBackground>();
	}

	public void MoveAll(float posX)
	{
		foreach (MoveBackground moveBackground in moveBackgroundScripts)
		{
			moveBackground.Move(posX);
		}
	}
}
