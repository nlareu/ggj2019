using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppController : MonoBehaviour {

    public AvatarController PlayerSource;
    public int PlayersCount = 4;
    public List<GameObject> RespawnPositions = new List<GameObject>();

    private List<AvatarController> players = new List<AvatarController>();
    private List<AvatarController> playersDead = new List<AvatarController>();

    [Header("Time variables")]
    public Text timerText;
    public GameObject completionMenu;
    [HideInInspector]
    public float elapsedTime = 0f;
    private bool updatingTimer = true;

    private bool levelFinished = false;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < this.PlayersCount; i++)
        {
            AvatarController player = Instantiate(this.PlayerSource, this.RespawnPositions[i].transform.position, Quaternion.identity);

            player.AppController = this;
            player.PlayerNumber = this.AddPlayer(player);
            player.Died += this.Player_Died;
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Reset static.
            this.players.Clear();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Updating timer UI.
        if (updatingTimer)
        {
            elapsedTime = Mathf.Max(0, elapsedTime + Time.deltaTime);
            var timeSpan = System.TimeSpan.FromSeconds(elapsedTime);
            timerText.text = timeSpan.Hours.ToString("00") + ":" +
                timeSpan.Minutes.ToString("00") + ":" +
                timeSpan.Seconds.ToString("00") + "." +
                timeSpan.Milliseconds / 100;
        }

        if (this.levelFinished && Input.anyKeyDown)
        {
            //Debug.Log("Restart the level");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /// <summary>
    /// Indicates that the level has ended and ends it.
    /// </summary>
    public void LevelEnded()
    {
        //Debug.Log("The level has ended");
        this.updatingTimer = false;
        this.levelFinished = true;
        this.completionMenu.SetActive(true);
    }

    public int AddPlayer(AvatarController avatar)
    {
        this.players.Add(avatar);

        return this.players.Count;
    }
    private bool CheckRoundEnded()
    {
        return (this.players.Count - this.playersDead.Count <= 1);
    }
    public List<AvatarController> GetPlayers()
    {
        //Return a copy to prevent reference and not desired changes on the list.
        return new List<AvatarController>(this.players);
    }
    private void RestartRound()
    {
        this.playersDead.Clear();

        for (int i = 0; i < this.players.Count; i++)
        {
            var player = this.players[i];
            var respawnPos = this.RespawnPositions[i].transform.position;

            player.transform.position = new Vector2(respawnPos.x, respawnPos.y);

            player.gameObject.SetActive(true);
        }
    }


    private void Player_Died(object sender, EventArgs e)
    {
        AvatarController avatar = (AvatarController)sender;

        playersDead.Add(avatar);



        if (this.CheckRoundEnded() == true)
            this.RestartRound();
    }
}
