using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OnGrounded : MonoBehaviour
{
	public GameObject PlayerHip;
	public GameObject Dust;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.tag == "Ground"){
			PlayerHip.GetComponent<Jump>().Grounded = true;
			if (Dust)
			{
				Instantiate(Dust,transform.position,transform.rotation);
			}
		}
	}	
	void OnCollisionStay2D(Collision2D col){
		if(col.gameObject.tag == "Ground"){
			PlayerHip.GetComponent<Jump>().Grounded = true;
			
		}
	}
	void OnCollisionExit2D(Collision2D col){
		if(col.gameObject.tag == "Ground"){
			PlayerHip.GetComponent<Jump>().Grounded = false;
			if (Dust)
			{
				Instantiate(Dust,transform.position,transform.rotation);
			}
		}
	}
}
