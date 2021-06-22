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
		/*movableMuscles.Add(LEFTLEG);
		movableMuscles.Add(RIGHTLEG);*/
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

	private void Start() {
		//muscles = GetComponentsInChildren<Rigidbody2D>();
	}
    // Update is called once per frame
    private void Update()
    {
		foreach (_Muscle muscle in muscles)
		{
			muscle.ActiveMuscle();
		}
		
		if (CanControllerNow)
		{
			if (Input.GetKeyDown(RightInput))
			{
				right = true;
			}
			if (Input.GetKeyDown(LeftInput))
			{
				left = true;
			}
			if (Input.GetKeyUp(LeftInput))
			{
				left = false;
				right = false;

			}
			if (Input.GetKeyUp(RightInput))
			{
				left = false;
				right = false;
			}
		}
		

		while(right == true && left == false && Time.time > MoveDelayPointer)
		{
			RIGHTLEG.bone.MoveRotation(Mathf.LerpAngle(RIGHTLEG.bone.rotation, -60, 1000 * Time.deltaTime));
			RIGHTLEG.bone.AddForce(new Vector2(10, 0), ForceMode2D.Impulse);
			//LEFTLEG.bone.AddForce(new Vector2(60, 0) * -0.5f, ForceMode2D.Impulse);
			//Invoke("Step1RightArm",0f);
			//Invoke("Step2Right",0.085f);

			MoveDelayPointer = Time.time  + MoveDelay;
		}
		while(left == true && right == false && Time.time > MoveDelayPointer)
		{
			LEFTLEG.bone.MoveRotation(Mathf.LerpAngle(LEFTLEG.bone.rotation, 60, 1000 * Time.deltaTime));
			LEFTLEG.bone.AddForce(new Vector2(-10, 0), ForceMode2D.Impulse);
			//RIGHTLEG.bone.AddForce(new Vector2(-60, 0) * -0.5f, ForceMode2D.Impulse);
			//Invoke("Step2left",0.085f);
			MoveDelayPointer = Time.time  + MoveDelay;
		}
    }

	public void Balance()
	{
		foreach (_Muscle muscle in muscles)
		{
			muscle.ActiveMuscle();
		}
	}

	public void Step(int muscleIndex, Vector2 rotation, float multiplierForce)
	{
		movableMuscles[muscleIndex].ApplyForce(rotation, multiplierForce);
		/*if(muscleIndex == 0)
		{
			movableMuscles[1].ApplyForce(rotation * -0.5f, multiplierForce);
		}
		else
		{
			movableMuscles[0].ApplyForce(rotation * -0.5f, multiplierForce);
		}*/
	}

	public void Step2(int muscleIndex, Vector2 addForceVector, float moveRotationRotation, float moveRotationLerpMultiplier)
	{
		//movableMuscles[muscleIndex].bone.MoveRotation(Mathf.LerpAngle(movableMuscles[muscleIndex].RootRotation, moveRotationRotation, moveRotationLerpMultiplier * Time.deltaTime));
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
