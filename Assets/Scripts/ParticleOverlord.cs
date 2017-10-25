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
		
		SpawnParticle(position, particlename, ParticleDictionary[particlename].GetComponent<ParticleSystem>().main.startColor.color);
	}

	public void SpawnParticle(Vector3 position, string particlename, Color particleColor){

		GameObject newParticle = Instantiate(ParticleDictionary[particlename], position, Quaternion.identity);
		var psMain = newParticle.GetComponent<ParticleSystem>().main;
		float particleLife = psMain.duration;
		psMain.startColor = particleColor;
		Destroy(newParticle, particleLife);
	}

}
