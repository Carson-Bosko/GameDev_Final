using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedWeaponCointroller : MonoBehaviour {

    public Image pistolSelected;
    public Image SMGSelected;

    public Text ammoText;

    public Entity player;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (player.weaponIndex == 0) {
            pistolSelected.enabled = true;
            SMGSelected.enabled = false;
        }
        else if (player.weaponIndex == 1) {
            pistolSelected.enabled = false;
            SMGSelected.enabled = true;
        }
        ammoText.text = player.currentWeapon.Ammo + " / " + player.currentWeapon.maxAmmo;
    }
}
