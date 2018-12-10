using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparksController : MonoBehaviour {

    private ParticleSystem sparks;
    private float timer = 0;
    private float sparkRate = 5f;

	// Use this for initialization
	void Start () {
        sparks = gameObject.GetComponent<ParticleSystem>();
        sparks.Stop();
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer >= sparkRate) {
            sparks.Emit(Random.Range(5, 10));
            PlayForTime(0.3f);
            timer = 0f;
        }
	}

    public void PlayForTime(float time) {
        GetComponent<AudioSource>().Play();
        Invoke("StopAudio", time);
    }

    private void StopAudio() {
        GetComponent<AudioSource>().Stop();
    }
}
