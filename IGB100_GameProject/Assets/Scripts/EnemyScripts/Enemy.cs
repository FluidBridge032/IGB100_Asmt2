﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    
    public float healthMax = 100;
    private float health;
    public float hitDamage = 25;
    private float damageTimer;
    private float damageRate = 1.0f;

    private bool firing = false;

    public float killIntention = 10.0f;
    public int token = 1;
    public GameObject tokenObject;
    public int score = 100;

    public bool isBoss = false;
    private float supportTimer = 0.0f;
    private float supportTimerMax = 2.1f;

    private float hurtFlashingTimer = 0.0f;
    private float hurtFlashingRate = 0.05f;
    private float flashDuration = 0.0f;
    private float flashDurationMax = 0.2f;
    private bool bright = false;
    public SkinnedMeshRenderer[] thisRenderers;
    private Color currentColour;
    private Color emissionColour;

    //Effects
    public GameObject deathEffect;

	// Use this for initialization
	void Start () {
        health = healthMax;
        foreach (SkinnedMeshRenderer eachRenerer in thisRenderers)
        {
            eachRenerer.material.EnableKeyword("_EMISSION");
        }
        currentColour = thisRenderers[0].material.GetColor("_EmissionColor");
        emissionColour = new Color(1.0f, 0.0f, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {
        Flashing();
        SupprortCountdown();
    }

    //Public method for taking damage and dying
    public void TakeDamage(float dmg) {
        health -= dmg;
        //Debug.Log("hit");
        AddFlashDuration();
        if (isBoss)
        {
            GameManager.instance.UpdateBossHpBar(health, healthMax);
            if (GetComponent<BossManagementJump>().GetBattleStage() == 1 && health < healthMax / 2)
            {
                GetComponent<BossManagementJump>().SetBattleStage(2);
            }
        }
        if (health <= 0) {
            if (isBoss)
            {
                GameManager.instance.win = true;
                GameManager.instance.GameOver();
            }
            
            Destroy(this.gameObject);
        }
    }
    public void AddHp(float add)
    {
        health += add;
        if(health > healthMax)
        {
            health = healthMax;
        }
        if (isBoss)
        {
            GameManager.instance.UpdateBossHpBar(health, healthMax);
        }
    }
    private void OnCollisionStay(Collision collision) {

        if (collision.transform.tag == "Player" && Time.time > damageTimer) {
            collision.gameObject.GetComponent<Player>().TakeDamage(hitDamage);
            damageTimer = Time.time + damageRate;
        }
    }

    public void SetFiring(bool f)
    {
        firing = f;
    }
    public bool GetFiring()
    {
        return firing;
    }

    private void Flashing()
    {
        if (flashDuration > 0)
        {
            if (Time.time - hurtFlashingTimer > hurtFlashingRate)
            {
                //Debug.Log("bright = " + bright);
                if (bright)
                {
                    foreach (SkinnedMeshRenderer eachRenerer in thisRenderers)
                    {
                        eachRenerer.material.SetColor("_EmissionColor", emissionColour);
                    }                  
                }
                else
                {
                    foreach (SkinnedMeshRenderer eachRenerer in thisRenderers)
                    {
                        eachRenerer.material.SetColor("_EmissionColor", currentColour);
                    }
                }
                bright = !bright;
                hurtFlashingTimer = Time.time;
            }
            flashDuration -= Time.deltaTime;
            if (flashDuration < 0)
            {
                foreach (SkinnedMeshRenderer eachRenerer in thisRenderers)
                {
                    eachRenerer.material.SetColor("_EmissionColor", currentColour);
                }
                bright = false;
                flashDuration = 0;
            }
        }
    }

    private void AddFlashDuration()
    {
        flashDuration = flashDurationMax;
        bright = true;
    }

    public void SetSupport(float t)
    {
        supportTimer += t;
        if (supportTimer > supportTimerMax)
        {
            supportTimer = supportTimerMax;
        }
    }
    private void SupprortCountdown()
    {
        supportTimer -= Time.deltaTime;
        if (supportTimer < 0)
        {
            supportTimer = 0;
        }
    }

    public bool CheckIsSupported()
    {
        if (supportTimer > 0) {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDestroy()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        GameObject tempToken = Instantiate(tokenObject, transform.position, transform.rotation);
        tempToken.GetComponent<Token>().SetTokenValue(token);
        //GameManager.instance.player.GetComponent<Player>().tokens += token;
        GameManager.instance.score += score;
        GameManager.instance.player.GetComponent<Player>().AddKillIntention(killIntention);
        GameManager.instance.ReduceEnemyCount(1);
    }
}
