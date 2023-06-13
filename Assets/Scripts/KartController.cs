using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{
    [SerializeField] private Rigidbody sphereRB;
    [SerializeField] private float forwardSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private LayerMask groundLayerMask;

    private float _forwardAmount;
    private float _currentSpeed;
    private float _turnAmount;
    private bool isGrounded;

    [Header("Sound Manager")]

    [Tooltip("What audio clip should play when the kart starts?")]
    public AudioSource StartSound;
    [Tooltip("What audio clip should play when the kart does nothing?")]
    public AudioSource IdleSound;
    [Tooltip("What audio clip should play when the kart moves around?")]
    public AudioSource RunningSound;
    [Tooltip("What audio clip should play when the kart is drifting")]
    public AudioSource Drift;
    [Tooltip("Maximum Volume the running sound will be at full speed")]
    [Range(0.1f, 1.0f)] public float RunningSoundMaxVolume = 1.0f;
    [Tooltip("Maximum Pitch the running sound will be at full speed")]
    [Range(0.1f, 2.0f)] public float RunningSoundMaxPitch = 1.0f;
    [Tooltip("What audio clip should play when the kart moves in Reverse?")]
    public AudioSource ReverseSound;
    [Tooltip("Maximum Volume the Reverse sound will be at full Reverse speed")]
    [Range(0.1f, 1.0f)] public float ReverseSoundMaxVolume = 0.5f;
    [Tooltip("Maximum Pitch the Reverse sound will be at full Reverse speed")]
    [Range(0.1f, 2.0f)] public float ReverseSoundMaxPitch = 0.6f;



    private void Start()
    {
        sphereRB.transform.parent = null;
    }

    private void Update()
    {
        transform.position = sphereRB.transform.position;

        _forwardAmount = Input.GetAxis("Vertical");
        _turnAmount = Input.GetAxis("Horizontal");

        if (_forwardAmount != 0)
        {
            Drive();
        }
        else
        {
            DriveNowhere();
        }

        IdleSound.volume = Mathf.Lerp(0.6f, 0.0f, _currentSpeed * 4);

        if (_currentSpeed < 0)
        {
            // In reverse
            RunningSound.volume = 0.0f;
            ReverseSound.volume = Mathf.Lerp(0.1f, ReverseSoundMaxVolume, -_currentSpeed * 1.2f);
            ReverseSound.pitch = Mathf.Lerp(0.1f, ReverseSoundMaxPitch, -_currentSpeed + (Mathf.Sin(Time.time) * .1f));
        }
        else
        {
            // Moving forward
            ReverseSound.volume = 0.0f;
            RunningSound.volume = Mathf.Lerp(0.1f, RunningSoundMaxVolume, _currentSpeed * Time.deltaTime);
            RunningSound.pitch = Mathf.Lerp(0.3f, RunningSoundMaxPitch, _currentSpeed + (Mathf.Sin(Time.time) * .1f));
        }

        TurnHandler();
        GroundCheckAndNormalHandler();
    }

    private void GroundCheckAndNormalHandler()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1, groundLayerMask);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation, 0.1f);
    }

    private void TurnHandler()
    {
        float newRotation = _turnAmount * turnSpeed * Time.deltaTime;
        if(_currentSpeed > 0.1f)
        {
            transform.Rotate(0, newRotation, 0, Space.World);
        }
    }

    private void FixedUpdate()
    {
        sphereRB.AddForce(transform.forward * _currentSpeed, ForceMode.Acceleration);
    }

    private void Drive()
    {
        _currentSpeed = _forwardAmount *= forwardSpeed;
    }
    private void DriveNowhere()
    {
        _currentSpeed = 0;
    }
}
