﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float health = 100;
    public float maxHealth;
    public float killIntention;
    public float maxKillIntention;
    public float ShotGunFireRate;
    public float GrimbrandFireRate;
    public int ultimateSkillID = 1;
    public string ultimateSkillName;
    public GameObject mainCamera;
    public GameObject EnemyBullet;
    public GameObject[] weapons;
    public int tokens;

    private float teleportTimer = 0.0f; // do not delete this
    private string currentScene;
    //UI Elements
    public GameObject uiController;
    [SerializeField] private GameObject playerUI;

    public GameObject effectHeal;
    public GameObject roomFightBGM;
    public GameObject bossFightBGM;

    // Use this for initialization
    void Start () {
        if (ultimateSkillID == 1) { ultimateSkillName = "Heal"; }
        else if (ultimateSkillID == 2) { ultimateSkillName = "Adrenaline Surge"; }
        uiController.GetComponent<UIController>().UpdateHpBar(health, maxHealth);
        uiController.GetComponent<UIController>().UpdateKillIntentionBar(killIntention, maxKillIntention, ultimateSkillName);
        if (SceneManager.GetActiveScene().name == "Lounge")
        {

        }
    }
	
	// Update is called once per frame
	void Update () {
        TeleportCountDown();
        if (playerUI.activeInHierarchy)
        {
            if (Input.GetKeyDown("f"))
            {
                UseUltimateSkill();
            }
        }
    }
    void OnUpdate(string sceneName)
    {

    }

    public void TakeDamage(float dmg) {
        // increase kill intention when hurt
        killIntention += dmg * 100 / maxHealth;
        if (killIntention > maxKillIntention)
        {
            killIntention = maxKillIntention;
        }
        uiController.GetComponent<UIController>().UpdateKillIntentionBar(killIntention, maxKillIntention, ultimateSkillName);

        // lose hp when hurt , 0 to die
        health -= dmg;
        uiController.GetComponent<UIController>().UpdateHpBar(health, maxHealth);
        
        if (health <= 0) {
            GameManager.instance.win = false;
            GameManager.instance.GameOver();
        }
    }

    public void AddKillIntention(float k)
    {
        killIntention += k;
        if (killIntention > maxKillIntention) 
        { 
            killIntention = maxKillIntention;
        }
        uiController.GetComponent<UIController>().UpdateKillIntentionBar(killIntention, maxKillIntention, ultimateSkillName);
        
    }


    public void UseUltimateSkill()
    {
        if(killIntention == maxKillIntention)
        {
            switch (ultimateSkillID)
            {
                case 1:
                    killIntention = 0;
                    USkill_Heal(maxHealth / 2);
                    break;
                case 2:
                    break;
                default:
                    Debug.Log("Wrong ultimateSkillID: " + ultimateSkillID);
                    break;

            }
        }
        else
        {
            // show some prompt / ui
        }
    }

    // this function can also be called by enemy
    public void USkill_Heal(float h)
    {
        health += h;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        GameObject tempHeal = Instantiate(effectHeal, transform.position, transform.rotation);
        tempHeal.GetComponent<EffectOnObject>().SetTarget(this.gameObject);
        uiController.GetComponent<UIController>().UpdateHpBar(health, maxHealth);
        uiController.GetComponent<UIController>().UpdateKillIntentionBar(killIntention, maxKillIntention, ultimateSkillName);
    }

    public void USkill_AdrenalineSurge()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<NavMeshAgent>().speed /= 2;
        }
        foreach (GameObject projectile in GameObject.FindGameObjectsWithTag("EnemyBullet"))
        {
            projectile.GetComponent<EnemyBulletRed>().moveSpeed /= 2;
        }
        EnemyBullet.GetComponent<EnemyBulletRed>().moveSpeed /= 2;
    }

    public void ChangeSkill(int ID, string name)
    {
        ultimateSkillID = ID;
        ultimateSkillName = name;
    }

    public void Teleport(Transform t)
    {
        teleportTimer = 0.2f;
        Debug.Log("teleported");
        transform.position = t.position;
        GetComponent<PlayerLook>().SetRotation(t);
        GameObject.FindWithTag("Player").GetComponent<PlayerLook>().enabled = true;
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().enabled = true;
        GameObject.FindWithTag("Player").GetComponent<Interaction>().enabled = true;
        GameObject.FindWithTag("Player").GetComponentInChildren<PlayerGun>().enabled = true;
    }

    public void TeleportCountDown()
    {
        if (teleportTimer > 0.0f)
        {
            teleportTimer -= Time.deltaTime;
        }
        else
        {
            teleportTimer = 0.0f;
        }
    }

    public float GetTeleportTimer()
    {
        return teleportTimer;
    }

    public void SetBGM(string s)
    {
        switch (s)
        {
            case "roomFight":
                roomFightBGM.SetActive(true);
                bossFightBGM.SetActive(false);
                break;
            case "bossFight":
                roomFightBGM.SetActive(false);
                bossFightBGM.SetActive(true);
                break;
            default:
                break;
        }
    }
}