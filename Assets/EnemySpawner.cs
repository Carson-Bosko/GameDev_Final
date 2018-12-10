using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public GameObject[] spawners;

    public GameObject[] enemies;

    public float spawnRate = 5f;
    private float timer;

	// Use this for initialization
	void Start () {
        timer = 0;

    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer >= spawnRate) {
            timer = 0;
            spawnEnemy();
        }
	}

    private void spawnEnemy() {
        int x = Random.Range(0, 4);
        GameObject enemy = Instantiate(enemies[0]);
        enemy.transform.position = spawners[x].transform.position;

    }
}
