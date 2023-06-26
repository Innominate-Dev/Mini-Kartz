using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Checkpoints : MonoBehaviour
{
    public int checkpointsCompleted;
    public TextMeshProUGUI checkpoints;
    public List <GameObject> Checkpoint;
    // Start is called before the first frame update
    void Start()
    {
        //checkpointsCompleted = Checkpoint
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            checkpointsCompleted = checkpointsCompleted - 1;
            checkpoints.text = (checkpointsCompleted + " Left");
            Destroy(gameObject);
        }
    }
}
