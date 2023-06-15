using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{
    [SerializeField] private Rigidbody sphereRB;
    [SerializeField] private float forwardSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private LayerMask groundLayerMask;

    private float gravity = 9.81f;
    private float _forwardAmount;
    private float _currentSpeed;
    private float _turnAmount;
    private float rotate, currentRotate;
    private bool isGrounded;

    [Header("Drifting")]

    int driftDirection;
    float driftPower;
    int driftMode = 0;
    public bool drifting;

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

    public MovementState state;
    public enum MovementState
    {
        idle,
        moving,
        drifting,
        knockedout
    }

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

        if (Input.GetKey(KeyCode.LeftShift) && !drifting && Input.GetAxis("Horizontal") != 0)
        {
            drifting = true;
            driftDirection = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
        }
        else
        {
            drifting = false;
        }


        if (drifting)
        {
            float control;
            if (driftDirection == 1)
                control = 1;
            else
                control = -1;

            //float control = (driftDirection == 1) ? ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 0, 2) : ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 2, 0);
            float powerControl = (driftDirection == 1) ? ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, .2f, 1) : ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 1, .2f);
            Steer(driftDirection, control);
            driftPower += powerControl;
        }

        if (drifting)
        {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, .5f, 2) : ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 2, .5f);
            transform.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(transform.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
            /// THE LINE ABOVE IS THE ISSUE OF THE DRIFTING/// IF YOU REMOVE IT WON'T DRIFT BUT IF YOU ADD IT DRIFTS ON THE RIGHT
        }

        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;
        TurnHandler();
        GroundCheckAndNormalHandler();
        //Drifting();
    }

    private void GroundCheckAndNormalHandler()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1, groundLayerMask);

        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation, 0.1f);
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
        //sphereRB.AddForce(transform.forward * _currentSpeed, ForceMode.Acceleration);

        //Forward Acceleration
        if (drifting)
        {
            sphereRB.AddForce(-transform.transform.right * _currentSpeed, ForceMode.Acceleration);
        }
        else
        {
            sphereRB.AddForce(transform.forward * _currentSpeed, ForceMode.Acceleration);
        }

        //Steering
        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + transform.rotation.z, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, groundLayerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, groundLayerMask);

        //Normal Rotation
        //transform.up = Vector3.Lerp(transform.up, hitNear.normal, Time.deltaTime * 8.0f);
        //transform.Rotate(0, transform.eulerAngles.y, 0);
    }
    private void Drive()
    {
        _currentSpeed = _forwardAmount *= forwardSpeed;
        state = MovementState.moving;
    }
    private void DriveNowhere()
    {
        _currentSpeed = 0;
        state = MovementState.idle;
    }

    /*private void Drifting()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {

            if (_turnAmount > 0.1f)
            {
                transform.Rotate(transform.rotation.x, 50f, transform.rotation.z);
            }
            else if (_turnAmount < 0.1)
            {
                transform.Rotate(transform.rotation.x, -50f, transform.rotation.z);
            }
        }
    }*/

    public void Steer(int direction, float amount)
    {
        rotate = (_turnAmount * direction) * amount;
    }
}
