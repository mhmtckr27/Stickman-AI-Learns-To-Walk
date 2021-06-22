using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableManager : MonoBehaviour
{
	[SerializeField] private GameObject tablePrefab;
	[SerializeField] private int maxMeters;
	

	private void Awake()
	{
		int tableCount = (maxMeters - 10) / 10;
		tableCount += 2;

		GameObject currentTable = Instantiate(tablePrefab, new Vector3(5, -3.5f, 0), Quaternion.identity, transform);
		
		foreach(Text text in currentTable.GetComponentsInChildren<Text>())
		{
			text.text = "5 Meters";
		}

		currentTable = Instantiate(tablePrefab, new Vector3(10, -3.5f, 0), Quaternion.identity, transform);

		foreach (Text text in currentTable.GetComponentsInChildren<Text>())
		{
			text.text = "10 Meters";
		}

		for (int i = 2; i < tableCount; ++i)
		{
			currentTable = Instantiate(tablePrefab, new Vector3(i * 10, -3.5f, 0), Quaternion.identity, transform);

			foreach (Text text in currentTable.GetComponentsInChildren<Text>())
			{
				text.text = i * 10 + " Meters";
			}
		}
	}
}
