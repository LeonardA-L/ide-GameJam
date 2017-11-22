using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour {


	public AudioClip events; 

	private AudioSource source;




	// Use this for initialization
	void Start () {

		source = GetComponent<AudioSource>();

	}

	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown("Fire1"))
		{
			source.isPlaying (events, 1f); 
		}
	}
}
