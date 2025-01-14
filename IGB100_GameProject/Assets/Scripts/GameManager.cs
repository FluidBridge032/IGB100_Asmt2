﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public float startTime;
    private GameObject enemySpawner;
    //public float maxTime = 15.0f;
    //Singleton Setup
    public static GameManager instance = null;

    public GameObject player;
    public GameObject uiController;

    public bool win = false;
    public bool gameOver = false;
    public bool menu = false;
    public bool pause = false;
    public bool hasPlayed = false;
    public bool bossFight = false;
    private bool isRoomClear = false;

    public int roomMax = 3; // Start counting from 1
    public int currentRoom = 0; // Start counting from 0
    private int difficulty = 0;
    public int[] totalEnemyInEachRoom;
    public int[] totalEnemyInEachRoomHard;
    private int currentEnemyCount = 0;
    public Transform[] teleportPointInRooms;
    public Transform teleportPointInLounge;
    public GameObject door;
    public GameObject portalController;

    public GameObject[] enemySpawnerInEachRoom;
    public GameObject[] enemySpawnerInEachRoomHard;
    public GameObject roof;

    public int score = 0;
    
    //private float pauseDelay = 0.5f;
    //private float pauseTimer = 0.0f;



    // Awake Checks - Singleton setup
    void Awake()
    {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
        Debug.Log("game start");
        TeleportToRoom();
        player.GetComponent<Player>().health = 100;
        player.GetComponent<Player>().SetBGM("roomFight");
        gameOver = false;
        win = false;
        roof.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //time += Time.deltaTime;
        if (!bossFight)
        {
            RoomClear();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            TeleportToLounge();
        }
        AutoTeleportToRoom();
    }


    public void GameOver()
    {
        gameOver = true;
        Time.timeScale = 0;
        GameObject.FindWithTag("Player").GetComponent<PlayerLook>().enabled = false;
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().enabled = false;
        GameObject.FindWithTag("Player").GetComponent<Interaction>().enabled = false;
        GameObject.FindWithTag("Player").GetComponentInChildren<PlayerGun>().enabled = false;
        if (win)
        {
            uiController.GetComponent<UIController>().ShowWinUI();
        }
        else
        {
            uiController.GetComponent<UIController>().ShowLoseUI();

        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Destroy(enemySpawner);

    }

    public void BossFight(bool b)
    {
        if (b)
        {
            uiController.GetComponent<UIController>().ShowBossUI();
        }
        else
        {
            uiController.GetComponent<UIController>().HideBossUI();
        }
        
    }

    public void UpdateBossHpBar(float hp, float hpMax)
    {
        uiController.GetComponent<UIController>().UpdateBossHpBar(hp, hpMax);
    }

    public void ReduceEnemyCount(int c)
    {
        currentEnemyCount -= c;
        if (currentEnemyCount < 0)
        {
            Debug.Log("Error: currentEnemyCount < 0! currentEnemyCount = " + currentEnemyCount);
            currentEnemyCount = 0;
        }
    }

    private void RoomClear()
    {
        if (!isRoomClear && currentEnemyCount <= 0)
        {
            portalController.GetComponent<PortalController>().ShowPortal(currentRoom);
            currentRoom++;
            isRoomClear = true;
            if (currentRoom >= roomMax)
            {
                //Debug.Log("Error: currentRoom >= roomMax! currentRoom = " + currentRoom);
                currentRoom = roomMax - 1;
            }
            uiController.GetComponent<UIController>().ShowRoomClearUI();
         
            //GameObject.FindWithTag("Player").GetComponent<PlayerLook>().enabled = false;
            //GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().enabled = false;
            //GameObject.FindWithTag("Player").GetComponent<Interaction>().enabled = false;
            //GameObject.FindWithTag("Player").GetComponentInChildren<PlayerGun>().enabled = false;
        } 
    }

    public void TeleportToLounge()
    {
        player.GetComponent<Player>().Teleport(teleportPointInLounge.transform);
        //player.transform.position = teleportPointInLounge.position;
        //Cursor.lockState = CursorLockMode.Locked;
        uiController.GetComponent<UIController>().HideRoomClearUI();

    }

    private void AutoTeleportToRoom()
    {
        if (Vector3.Distance(player.transform.position, door.transform.position) < 1.0f)
        {
            TeleportToRoom();
        }
    }

    public void TeleportToRoom() // and start fighting
    {
        startTime = Time.time;
        isRoomClear = false;
        Cursor.lockState = CursorLockMode.Locked;
        switch (difficulty)
        {
            case 0:
                currentEnemyCount = totalEnemyInEachRoom[currentRoom];
                enemySpawner = Instantiate(enemySpawnerInEachRoom[currentRoom], transform.position, transform.rotation);
                break;
            case 1:
                currentEnemyCount = totalEnemyInEachRoomHard[currentRoom];
                enemySpawner = Instantiate(enemySpawnerInEachRoomHard[currentRoom], transform.position, transform.rotation);
                break;
            default:
                Debug.Log("Invalid difficulty.");
                break;
        }
        
        player.GetComponent<Player>().Teleport(teleportPointInRooms[currentRoom]);
        if (currentRoom == roomMax -1) {
            player.GetComponent<Player>().SetBGM("bossFight");
        }
        else
        {
            player.GetComponent<Player>().SetBGM("roomFight");
        }
    }
}