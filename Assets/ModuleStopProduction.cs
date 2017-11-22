using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleStopProduction : MonoBehaviour {

	public AudioClip moduleStopProduction; 

	private AudioSource source;
	private float volLowRange = .5f;
	private float volHighRange = 1.0f;



	// Use this for initialization
	void Start () {

		source = GetComponent<AudioSource>();

	}

	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown("Fire1"))
		{
			source.PlayOneShot (moduleStopProduction, 1f); 
		}
	}
}