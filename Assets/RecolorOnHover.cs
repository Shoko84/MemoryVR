using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolorOnHover : MonoBehaviour {

	public Material notLooking;
	public Material whenLooking;

	// Use this for initialization
	void Start () {

	}

	public void WhenLookingAtObject()
	{
		GetComponent<Renderer>().material = whenLooking;
	}

	public void WhenNotLookingAtObject()
	{
		GetComponent<Renderer>().material = notLooking;
	}
}
