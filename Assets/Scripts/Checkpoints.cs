using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Checkpoints : MonoBehaviour
{
    public int checkpointsCompleted;
    public TextMeshProUGUI checkpoints;
    public GameObject checkpoint;
    public AudioSource checkpointSFX;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkpointsCompleted = checkpoint.transform.childCount;
        checkpoints.text = (checkpointsCompleted + " Left");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            checkpointsCompleted = checkpointsCompleted - 1;
            checkpoints.text = (checkpointsCompleted + " Left");
            checkpointSFX.PlayOneShot(checkpointSFX.clip);
            Destroy(gameObject);
            
        }
    }
}
