using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public AppController appController;
    public float goalDelay;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // The player must remain on the platform for a brief time to win
        if (collision.CompareTag("Player"))
        {
            //Debug.log("the player has reached the goal");
            //this.appController.LevelEnded();
            this.appController.Invoke("LevelEnded", this.goalDelay);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.appController.CancelInvoke("LevelEnded");
        }
    }
}
