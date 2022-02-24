using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private enum GameState { START, PLAY, PAUSE, WIN, LOSE};
    private GameState gameState;

    private float energy;
    private float respawnTime = 50.0f;
    private float energyLosingTime = 5.0f;
    private float timeLeft;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private GameObject energyBall;

    public GameObject player;
    public GameObject fpCamera;
    public GameObject externalCamera;
    public GameObject messages;
    public GameObject energyPrefab;
    public bool renderAnaglyphic;
    public Material anaglyphicMaterial;

    public GameObject startView;
    public GameObject playView;

    static bool startCalled = false;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.START;
        startPosition = player.transform.position;
        startRotation = player.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameState)
        {
            case GameState.START:
                // game reset
                if(startCalled == false)
                {
                    resetGame(); // ONE TIME
                }
                DisablePlayerMovement();
                // start view enabled, other disabled
                // player camera disabled
                SetExternalCamera();
                // energy spawn
                EnergyRespawn();

                // Start Canvas Text
                // check input
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    gameState = GameState.PLAY;
                }
                ; break;
            case GameState.PLAY:
                EnablePlayerMovement();
                // play view enabled, other disabled
                SetPlayView();
                // player camera enabled
                SetPlayerCamera();

                // no messages needed
                messages.gameObject.SetActive(true);
                messages.transform.Find("GameName").GetComponent<Text>().text = "";
                messages.transform.Find("Status").GetComponent<Text>().text = "";
                messages.transform.Find("EventText").GetComponent<Text>().text = "TIME: " + timeLeft;
                messages.transform.Find("EventText2").GetComponent<Text>().text = "ENERGY: " + energy + " %";
                // pickup event and energy recharge

                // energy state
                // energy lose over time
                energyLosingTime -= Time.deltaTime;
                if (energyLosingTime <= 0.0f)
                {
                    // enery lose
                    energy -= 4.0f;
                    energyLosingTime = 5.0f;
                }
                // respawn of energy
                respawnTime -= Time.deltaTime;
                if(respawnTime <= 0.0f)
                {
                    EnergyRespawn();
                    respawnTime = 50.0f;
                }
                // time runout
                timeLeft -= Time.deltaTime;

                if (Input.GetKeyDown(KeyCode.P))
                {
                    gameState = GameState.PAUSE;
                }

                // when time is over or energy empty
                if (timeLeft <= 0.0f)
                {
                    messages.transform.Find("GameName").GetComponent<Text>().text = "TIME OUT";
                    gameState = GameState.LOSE;
                } else if (energy <= 0.0f)
                {
                    messages.transform.Find("GameName").GetComponent<Text>().text = "NO ENERGY";
                    gameState = GameState.LOSE;
                }

                ; break;
            case GameState.PAUSE:
                // stop movement
                DisablePlayerMovement();
                // canvas pause
                messages.gameObject.SetActive(true);
                messages.transform.Find("GameName").GetComponent<Text>().text = "";
                messages.transform.Find("Status").GetComponent<Text>().text = "PAUSED.";
                messages.transform.Find("EventText").GetComponent<Text>().text = "PRESS [SPACE] TO CONTINUE"; 
                messages.transform.Find("EventText2").GetComponent<Text>().text = "PRESS [E] TO END";
                // check input play / end
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    gameState = GameState.PLAY;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    startCalled = false;
                    gameState = GameState.START;
                }
                ; break;
            case GameState.WIN:
                // when goal is reached
                // canvas win
                messages.gameObject.SetActive(true);
                messages.transform.Find("GameName").GetComponent<Text>().text = "";
                messages.transform.Find("Status").GetComponent<Text>().text = "YOU WIN!";
                messages.transform.Find("EventText").GetComponent<Text>().text = "PRESS [SPACE] TO RESTART";
                //messages.transform.Find("EventText2").GetComponent<Text>().text = "PRESS [E] TO END";
                messages.transform.Find("EventText2").GetComponent<Text>().text = "";

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    startCalled = false;
                    gameState = GameState.START;
                }
                ; break;
            case GameState.LOSE:

                DisablePlayerMovement();
                player.GetComponent<AudioSource>().Play();
                messages.gameObject.SetActive(true);
                messages.transform.Find("Status").GetComponent<Text>().text = "GAME OVER";
                messages.transform.Find("EventText").GetComponent<Text>().text = "PRESS [SPACE] TO RETRY";
                messages.transform.Find("EventText2").GetComponent<Text>().text = "";
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    startCalled = false;
                    gameState = GameState.START;
                }

                ; break;
        }
    }

    private void resetGame()
    {
        // setting data back
        energy = 100.0f;
        timeLeft = 200.0f;
        // setting place back
        startView.SetActive(true);
        playView.SetActive(false);
        // setting position back
        player.GetComponent<Transform>().SetPositionAndRotation(startPosition, startRotation);
        // setting pick ups back
        // external camera enabled
        // all cameras disabled
        externalCamera.SetActive(false);
        if (externalCamera.GetComponent<AnaglyphizerC>() != null)
        {
            Destroy(externalCamera.GetComponent<AnaglyphizerC>());
            foreach (Transform child in externalCamera.transform)
            {
                Destroy(child.gameObject);
            }
        }
        fpCamera.GetComponent<Camera>().enabled = false;
        if (fpCamera.GetComponent<AnaglyphizerC>() != null)
        {
            Destroy(fpCamera.GetComponent<AnaglyphizerC>());
            foreach (Transform child in fpCamera.transform)
            {
                Destroy(child.gameObject);
            }
        }
        startCalled = true;
    }

    private void SetExternalCamera()
    {
        fpCamera.GetComponent<Camera>().enabled = false;
        // off anaglyphic for FPS 
        // if to render anaglyphic and there is a alaglyphizeer
        if(renderAnaglyphic && fpCamera.GetComponent<AnaglyphizerC>() != null)
        {
            // remove it
            Destroy(fpCamera.GetComponent<AnaglyphizerC>());
            foreach (Transform child in fpCamera.transform)
            {
                Destroy(child.gameObject);
            }

        }

        externalCamera.SetActive(true);
        // ON anaglyphic for externalCamera 
        // if to render anaglyphic and there is no alaglyphizeer
        if (renderAnaglyphic && externalCamera.GetComponent<AnaglyphizerC>() == null)
        {
            externalCamera.AddComponent<AnaglyphizerC>();
            externalCamera.GetComponent<AnaglyphizerC>().anaglyphMat = anaglyphicMaterial;
            externalCamera.GetComponent<AnaglyphizerC>().enableKeys = false;
        }
    }

    private void SetPlayerCamera()
    {
        fpCamera.GetComponent<Camera>().enabled = true;
        // anaglyphic on for FPS
        if (renderAnaglyphic && fpCamera.GetComponent<AnaglyphizerC>() == null)
        {
            fpCamera.AddComponent<AnaglyphizerC>();
            fpCamera.GetComponent<AnaglyphizerC>().anaglyphMat = anaglyphicMaterial;
            fpCamera.GetComponent<AnaglyphizerC>().enableKeys = false;
        }

        externalCamera.SetActive(false);
        if (renderAnaglyphic && externalCamera.GetComponent<AnaglyphizerC>() != null)
        {
            Destroy(externalCamera.GetComponent<AnaglyphizerC>());
            foreach (Transform child in externalCamera.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void SetPlayView()
    {
        // setting place to maze
        startView.SetActive(false);
        playView.SetActive(true);
    }

    void DisablePlayerMovement()
    {
        player.GetComponent<ThirdPersonCharacter>().enabled = false;
        player.GetComponent<ThirdPersonUserControl>().enabled = false;
    }

    void EnablePlayerMovement()
    {
        player.GetComponent<ThirdPersonCharacter>().enabled = true;
        player.GetComponent<ThirdPersonUserControl>().enabled = true;
    }

    void EnergyRespawn()
    {
        Transform spawnPoint;
        for (int i = 1; i <= 7; i++)
        {
            spawnPoint = playView.transform.Find("SpawnPoint" + i);
            if (spawnPoint.childCount == 0)
            {
                energyBall = Instantiate(energyPrefab);
				energyBall.transform.SetParent(spawnPoint.transform,false); 
            }
        }
    }

	public void EnergyPickedUp(Collider other) {
		Destroy(other.gameObject);
		energy += 10.0f;
        if (energy > 100.0f)
        {
            energy = 100.0f;
        }
    }

    public void GameFinished(Collider other)
    {
        gameState = GameState.WIN;
    }
}
