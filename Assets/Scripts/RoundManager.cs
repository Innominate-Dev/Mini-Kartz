using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public GameState state;

    // GameObjects //

    public GameObject PlayerCamera;
    public GameObject roundBeginningCam;

    // Logic //

    private bool isStarted;

    [Header("Script References")]

    public KartAI AI;
    public KartController player;

    // Components //

    Animator anim;

    public enum GameState
    {
        Beginning,
        InProgress,
        Ending
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = roundBeginningCam.GetComponentInChildren<Animator>();
        startingRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f && isStarted == false)
        {
            isStarted = true;
            AI.speed = 20f;
            player.forwardSpeed = 50f;
            state = GameState.InProgress;
            PlayerCamera.gameObject.SetActive(true);
        }
    }

    void startingRound()
    {
        state = GameState.Beginning;        

    }
}
