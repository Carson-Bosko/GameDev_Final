﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthController : MonoBehaviour {


    public Text healthText;
    public Entity player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        healthText.text = player.Health + " / " + player.maxHealth;

        if(player.Health <= 0) {
            SceneManager.LoadScene("Scenes/MainMenu");
        }
	}
}
