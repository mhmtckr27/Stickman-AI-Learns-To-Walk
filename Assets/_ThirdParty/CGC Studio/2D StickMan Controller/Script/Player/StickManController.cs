using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class StickManController : MonoBehaviour
{
	public Jump playerHipJumpScript;
	public static int musclesCount;
	public static int movableMusclesCount;
	public _Muscle[] muscles = new _Muscle [7];
    [HideInInspector]
	public bool right;
	[HideInInspector]
	public bool left;
	 [HideInInspector]
	public bool CanControllerNow = true;

    [Header("MOVEMENT")]
	[Space()]

	public Rigidbody2D rbRIGHTLEG;
	public Rigidbody2D rbLEFTLEG;

	public _Muscle RIGHTLEG;
	public _Muscle LEFTLEG;

	public List<_Muscle> movableMuscles = new List<_Muscle>();

	public Vector2 WalkRightVector = new Vector2(1,0);

	float MoveDelayPointer;
	[Range(0.1f,1)]
	public float MoveDelay = 0.2f;

    [Header("INPUTS")]
	[Space()]

	public KeyCode LeftInput = KeyCode.LeftArrow; 
	public KeyCode RightInput = KeyCode.RightArrow;

	public List<Vector3> childLocations;
	public List<Quaternion> childRotations;

	private Individual individual;

	public Individual Individual
	{
		get => individual;
		set
		{
			if(value == null)
			{
				if (individual != null)
				{
					individual.stickman = null;
				}
				individual = null;
			}
			else
			{
				individual = value;
				individual.stickman = this;
			}
		}
	}

	private void Awake()
	{
		musclesCount = muscles.Length;
		movableMusclesCount = movableMuscles.Count;

		childLocations = new List<Vector3>();
		childRotations = new List<Quaternion>();

		Transform[] childTransforms = GetComponentsInChildren<Transform>();

		for (int i = 0; i < childTransforms.Length; ++i)
		{
			childLocations.Add(childTransforms[i].position);
			childRotations.Add(childTransforms[i].rotation);
		}
	}

    // Update is called once per frame
    private void Update()
    {
		foreach (_Muscle muscle in muscles)
		{
			muscle.ActiveMuscle();
		}
    }

	public void Balance()
	{
		foreach (_Muscle muscle in muscles)
		{
			muscle.ActiveMuscle();
		}
	}

	public void Step(int muscleIndex, Vector2 addForceVector)
	{
		if (playerHipJumpScript.Grounded)
		{
			movableMuscles[muscleIndex].bone.AddForce(addForceVector, ForceMode2D.Impulse);
		}
	}
}
[System.Serializable]
public class _Muscle
{
	public Rigidbody2D bone;
	public float RootRotation;
	public float force = 1000f;

	public void ActiveMuscle()
	{
		bone.MoveRotation(Mathf.LerpAngle(bone.rotation, RootRotation, force * Time.deltaTime));
	}

	public void ApplyForce(Vector2 rot, float forc)
	{
		bone.AddForce(rot * 0.25f * forc, ForceMode2D.Impulse);
	}

	public _Muscle (Transform t)
	{
		bone = t.GetComponent<Rigidbody2D>();
	}
	
}
