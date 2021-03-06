﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityController))]
public class Entity : MonoBehaviour {

    private Collider2D hitbox;

    private SpriteRenderer sprtieRenderer;

    private float health;
    public float maxHealth;
    public Action modHealthEvent;

    public Weapon currentWeapon;
    public Weapon[] weapons;
    public int weaponIndex = 0;

    //public float jumpHeight = 4;
    //public float timeToJumpApex = 0.4f;
    //float accelerationTimeAirborne = 0.2f;
    //float accelerationTimeGrounded = 0.1f;
    //float jumpVelocity;

    public float moveSpeed;
    public float regularMoveSpeed = 6;
    public float aimedMoveSpeed = 2;

    protected Vector2 facing;
    protected float recoilOffset;
    protected Vector2 recoilVector;

    float gravity = -50;
    Vector3 velocity;
    float velocityXSmoothing;

    EntityController controller;

    public Vector2 directionalInput;

    private bool aiming;
    private bool attackable;
    private bool inCover;
    private bool coverAvailable;

    private int coverLevel;

    public Vector2 Facing {
        get { return facing; }
        set { facing = value; }
    }

    public Weapon Weapon {
        get { return currentWeapon; }
        set { currentWeapon = value; }
    }

    public bool Attackable {
        get { return attackable; }
        set { attackable = value; }
    }

    public float Health {
        get { return health; }
        set { health = value; }
    }

    void ModHealthEvent() {
    }

    void Awake() {
        for (int i = 0; i < weapons.Length; i++) {
            weapons[i] = Instantiate(weapons[i], gameObject.transform);
            weapons[i].transform.localScale = new Vector3(0, 0, 0);
        }
        if (weapons.Length != 0) {
            currentWeapon = weapons[weaponIndex];
            switchWeapon(1);
        }
    }

    void Start() {
        sprtieRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<EntityController>();

        //jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        moveSpeed = regularMoveSpeed;
        aiming = false;
        attackable = true;
        inCover = false;
        coverAvailable = false;

        coverLevel = 0;

        //weapon = gameObject.AddComponent<Weapon>();
        recoilOffset = 0.0f;

        maxHealth = health = 100;
    }

    public void SetDirectionalInput(Vector2 input) {
        directionalInput = input;
    }

    void Update() {
        if (controller.collisions.above || controller.collisions.below) {
            if (controller.collisions.slidingDownMaxSlope) {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else {
                velocity.y = 0;
            }
        }

        //jumping
        //if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below) {
        //    if (controller.collisions.slidingDownMaxSlope) {
        //        if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) { //not jumping against max slope
        //            velocity.y = jumpVelocity * controller.collisions.slopeNormal.y;
        //            velocity.x = jumpVelocity * controller.collisions.slopeNormal.x;
        //        }
        //    }
        //    else {
        //        velocity.y = jumpVelocity;
        //    }
        //}

        if (inCover) {
            velocity.x = 0;
            return;
        }

        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, 0.1f);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void switchWeapon(int index) {
        currentWeapon.transform.localScale = new Vector3(0, 0, 0);
        currentWeapon = weapons[index-1];
        weaponIndex = index-1;
        currentWeapon.transform.localScale = new Vector3(1, 1, 1);
    }

    public void modHealth(float mod) {
        health += mod;
    }

    public void EnterCover() {
        if (coverAvailable && coverLevel >= 1 && !inCover) {
            sprtieRenderer.color = new Color(0.5f, 0.5f, 0.5f);
            attackable = false;
            inCover = true;
        }
    }

    public void ExitCover() {
        if (inCover) {
            sprtieRenderer.color = new Color(1, 1, 1);
            sprtieRenderer.flipY = false;
            attackable = true;
            inCover = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        Debug.Log(other.gameObject.layer);
        if (other.gameObject.layer == LayerMask.NameToLayer("CoverZone")) {
            coverAvailable = true;
            coverLevel += 1;
        }

        if (other.gameObject.CompareTag("Bullet")) {
            Debug.Log("ouch");
            ProjectileData proj = other.gameObject.GetComponent<ProjectileData>();
            modHealth(proj.GetComponent<ProjectileData>().Damage);
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("CoverZone")) {
            if (coverLevel > 0) {
                coverLevel -= 1;
                if (coverLevel == 0) {
                    coverAvailable = false;
                }
            }
        }
    }

    public void Shoot() {
        if (!inCover) {
            if (currentWeapon != null) {
                currentWeapon.Shoot(this, facing);
                recoilOffset += currentWeapon.RecoilValue;
            }
        }
    }

    public void SetAiming(bool state) {
        if (state) {
            aiming = true;
            moveSpeed = aimedMoveSpeed;
        }
        else {
            aiming = false;
            moveSpeed = regularMoveSpeed;
        }
    }

    public void CheckVerticalInput(bool shiftDown) {
        String s = "";
        if (shiftDown) {
            s += "shift on ";
            if (controller.collisions.underStairs) {
                s += "under stairs ";
                controller.collisionMask &= ~(1 << LayerMask.NameToLayer("Stairs"));
                controller.collisionMask |= 1 << LayerMask.NameToLayer("StairsTop");
            } else {
                s += "not under stairs ";
                controller.collisionMask |= 1 << LayerMask.NameToLayer("Stairs");
                controller.collisionMask &= ~(1 << LayerMask.NameToLayer("StairsTop"));
            }
        }
        else {
            s += "shift off ";
            if (controller.collisions.onStairs) {
                s += "on stairs ";
                controller.collisionMask |= 1 << LayerMask.NameToLayer("Stairs");
                controller.collisionMask &= ~(1 << LayerMask.NameToLayer("StairsTop"));
            } else {
                s += "not on stairs ";
                controller.collisionMask &= ~(1 << LayerMask.NameToLayer("Stairs"));
                controller.collisionMask |= 1 << LayerMask.NameToLayer("StairsTop");
            }
        }
        //Debug.Log(s);
    }

    public void UpdateFacing(Vector2 newFacing) {
        sprtieRenderer.flipX = Math.Sign(newFacing.x) == 1 ? false : true;

        float angle = Mathf.Atan2(newFacing.y, newFacing.x);
        var offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 0.8f;

        var weaponOffset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 0.5f;
        float weaponDirectionMod = Mathf.Sign(newFacing.x);

        if (currentWeapon != null) {
            currentWeapon.transform.position = transform.position;
            currentWeapon.transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector2(weaponDirectionMod * -1 * newFacing.y, weaponDirectionMod * newFacing.x));
            currentWeapon.GetComponent<SpriteRenderer>().flipX = Math.Sign(newFacing.x) == 1 ? false : true;
            currentWeapon.transform.position += weaponOffset;
        }

        float dirX = Mathf.Sign(newFacing.x);
        recoilVector = Vector3.Cross(newFacing, (dirX == -1) ? Vector3.forward : Vector3.back).normalized;
        Debug.DrawRay(transform.position, recoilVector, Color.green);

        Facing = newFacing;
    }
}
