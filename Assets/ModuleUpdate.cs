using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleUpdate : MonoBehaviour {


	public AudioClip moduleUpdate; 

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
			source.PlayOneShot (moduleUpdate, 1f); 
		}
	}
}