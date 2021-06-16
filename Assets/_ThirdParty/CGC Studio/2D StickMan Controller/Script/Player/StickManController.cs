using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class StickManController : MonoBehaviour
{
	public static int musclesCount;
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

	private Individual individual;

	public Individual Individual
	{
		get => individual;
		set
		{
			individual = value;
			individual.stickman = this;
		}
	}

	private void Awake()
	{
		movableMuscles.Add(LEFTLEG);
		movableMuscles.Add(RIGHTLEG);
		musclesCount = muscles.Length;
	}

	private void Start() {
		//muscles = GetComponentsInChildren<Rigidbody2D>();
	}
    // Update is called once per frame
    private void Update()
    {	
		/*foreach(_Muscle muscle in muscles)
		{
			muscle.ActiveMuscle(muscle.RootRotation, muscle.force);
		}*/
		
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
		

		/*while(right == true && left == false && Time.time > MoveDelayPointer)
		{
			Invoke("Step1Right",0f);
			//Invoke("Step1RightArm",0f);
			//Invoke("Step2Right",0.085f);
			MoveDelayPointer = Time.time  + MoveDelay;
		}*/
		/*while(left == true && right == false && Time.time > MoveDelayPointer)
		{
			Invoke("Step1Left",0f);
			Invoke("Step1LeftArm",0f);
			//Invoke("Step2left",0.085f);
			MoveDelayPointer = Time.time  + MoveDelay;
		}*/
    }

	public void Step(int muscleIndex, float rotation, float multiplierForce)
	{
		muscles[muscleIndex].ActiveMuscle(rotation, multiplierForce);
	}


}
[System.Serializable]
public class _Muscle
{
	public Rigidbody2D bone;
	public float RootRotation;
	public float force = 1000f;

	public void ActiveMuscle(float rot, float forc)
	{
		bone.MoveRotation(Mathf.LerpAngle(bone.rotation, rot, forc * Time.deltaTime));
	}
	public _Muscle (Transform t)
	{
		bone = t.GetComponent<Rigidbody2D>();
	}
	
}
