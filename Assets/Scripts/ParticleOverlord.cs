using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOverlord : MonoBehaviour {

	public static ParticleOverlord instance;

	public GameObject[] GameParticles;
	private IDictionary<string, GameObject> ParticleDictionary;
	// Use this for initialization
	void Start () {
		if (instance == null){
			instance = this;
		}
		ParticleDictionary = new Dictionary<string, GameObject>();
		foreach (GameObject g in GameParticles){ //the names of the prefabs are the strings used to retrieve them
			ParticleDictionary.Add(g.name, g);
		}
	}
	
	// Update is called once per frame
	public void SpawnParticle(Vector3 position, string particlename){
		
		GameObject newParticle = Instantiate(ParticleDictionary[particlename], position, Quaternion.identity);
		float particleLife = newParticle.GetComponent<ParticleSystem>().main.duration;
		Destroy(newParticle, particleLife);
	}
}
