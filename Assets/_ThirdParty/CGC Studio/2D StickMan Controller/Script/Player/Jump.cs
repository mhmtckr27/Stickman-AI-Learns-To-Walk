
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{ public float JumpSpeed = 10f;
	public bool Grounded;
	public KeyCode jump = KeyCode.Space;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {



		//if(Input.GetButton("Jump")&& Grounded == true){
		//	JumpDown();
		//}
		if(Input.GetKey(jump)&& Grounded == true){
			JumpDown();
		}
	}	
	public void JumpDown()
	{
		
			gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f,JumpSpeed),ForceMode2D.Impulse);

		
	}  

}
