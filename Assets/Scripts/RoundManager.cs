using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public GameState state;

    public GameObject PlayerCamera;
    public GameObject roundBeginningCam;

    private bool isStarted;

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
            state = GameState.InProgress;
            isStarted = true;
            PlayerCamera.gameObject.SetActive(true);
        }
    }

    void startingRound()
    {
        state = GameState.Beginning;        

    }
}
