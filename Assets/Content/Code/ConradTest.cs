using UnityEngine;
using System.Collections;

public class ConradTest : MonoBehaviour 
{
	private Animator mAnimator;

	// Use this for initialization
	private void Start () 
	{
		mAnimator = GetComponent<Animator> ();	
	}
	
	// Update is called once per frame
	private void Update () 
	{
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			mAnimator.SetTrigger("GoToWalk");
		}

		if (Input.GetKeyUp (KeyCode.A)) 
		{
			mAnimator.SetTrigger("GoToIdle");
		}
	}
}
