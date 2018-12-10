using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAI : MonoBehaviour {

    Entity entity;

    AudioSource audio;

    public GameObject target;

    void Start () {
        audio = GetComponent<AudioSource>();
        entity = GetComponent<Entity>();
        entity.regularMoveSpeed = .6f;
        entity.moveSpeed = entity.regularMoveSpeed;
        target = GameObject.Find("Player");
    }
	
	void Update () {
        Vector2 dir = target.transform.position - gameObject.transform.position;
        entity.SetDirectionalInput(dir);
        entity.UpdateFacing(dir);

        if (entity.Health <= 0) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<Entity>().modHealth(-10);
            audio.Play();
        }
    }
}
