﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    public float debugvar = 10f;

    public AppController AppController;
    public float AxisSensitive = 0.7f;
    public bool IsJumping = false;
    public float JumpHeight = 250.0f;
    public int PlayerNumber;
    public float Speed = 6.0F;
    //public GameObject groundCheck;

    [Header("Glide variables")]
    public float glideGravity;
    private float initialGravity;
    private bool isGliding = false;

    [Header("Sound Effects")]
    public AudioSource playerSoundSource;
    public AudioClip jumpSound;
    public AudioClip fallSound;
    public AudioClip glideSound;

    protected AvatarStates state = AvatarStates.Normal;
    public AvatarStates State
    {
        get
        {
            return this.state;
        }
        set
        {
            if (this.state != value)
            {
                this.state = value;

                //Set variables depending new state.
                switch (this.state)
                {
                    #region Normal
                    case AvatarStates.Normal:
                        {
                            //this.spriteRendered.color = Color.white;

                            this.rigidBody.gravityScale = 1f;
                            break;
                        }
                        #endregion
                }
            }
        }
    }

    public event EventHandler Died;

    protected Animator animator;
    internal BoxCollider2D boxCollider;
    internal Vector2 currentDirection
    {
        get
        {
            return new Vector2(
                        (this.rigidBody.position.x > this.previousPosition.x)
                            ? 1
                            : (this.rigidBody.position.x < this.previousPosition.x)
                                ? -1
                                : 0,
                        (this.rigidBody.position.y > this.previousPosition.y)
                            ? 1
                            : (this.rigidBody.position.y < this.previousPosition.y)
                                ? -1
                                : 0
            ); ;
        }
    }
    protected string playerName
    {
        //get { return "Player" + this.PlayerNumber + "-"; }
        get { return "Player-"; }
    }
    protected internal Vector2 previousDirection = Vector2.zero;
    protected internal Vector2 previousPosition = Vector2.zero;
    protected internal AvatarStates previousState;
    protected SpriteRenderer spriteRendered;
    protected internal Rigidbody2D rigidBody;

    protected virtual void Awake()
    {
        this.animator = GetComponent<Animator>();
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.rigidBody = GetComponent<Rigidbody2D>();
        this.spriteRendered = GetComponent<SpriteRenderer>();
        this.tag = Tags.PLAYER;

        this.initialGravity = this.rigidBody.gravityScale;
    }

    protected virtual void Start()
    {
        // Getting the reference to the camera
        CameraFollow myCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();

        if (myCamera != null)
        {
            //Debug.Log("We have a Camera");
            myCamera.player = gameObject.transform;
        }
    }

    protected virtual void Update()
    {
        switch (this.State)
        {
            #region Normal
            case AvatarStates.Normal:
                {
                    this.CheckAvatarMove();

                    break;
                }
            #endregion
        }

        this.previousDirection = this.currentDirection;
        this.previousPosition = new Vector2(this.rigidBody.position.x, this.rigidBody.position.y);
        this.previousState = this.State;

        //Debug.Log("Velocity: " + this.rigidBody.velocity.ToString());
        //if (this.rigidBody.velocity.x != 0) 
        //    Debug.Log("Velocity: " + this.rigidBody.velocity.ToString());
        //Debug.Log(string.Format("Axis H: {0}, V: {1}.", (float)Input.GetAxis(this.playerName + "Horizontal"), (float)Input.GetAxis(this.playerName + "Vertical")));
    }

    private void CheckAvatarMove()
    {
        Vector2 moveVector = new Vector2();
        //float axisHor = Input.GetAxis(this.playerName + "Horizontal");
        //float axisVer = Input.GetAxis(this.playerName + "Vertical");

        if (Input.GetButton(this.playerName + "Left"))
            //if (Input.GetButton(this.playerName + "Left") || axisHor <= -this.AxisSensitive)
        {
            moveVector += Vector2.left * this.Speed * Time.deltaTime;

            this.animator.SetBool("Moving", true);
            this.animator.SetFloat("MoveX", -1.5f);
        }
        else if (Input.GetButton(this.playerName + "Right"))
        //else if (Input.GetButton(this.playerName + "Right") || axisHor >= this.AxisSensitive)
        {
            moveVector += Vector2.right * this.Speed * Time.deltaTime;

            this.animator.SetBool("Moving", true);
            this.animator.SetFloat("MoveX", 1.5f);
        }
        else
        {
            this.animator.SetBool("Moving", false);
            //this.animator.SetFloat("MoveX", 0.5f);
        }

        if (Input.GetButton(this.playerName + "Jump") && this.IsJumping == false)
        {
            //this.rigidBody.AddForce(Vector2.up * this.JumpHeight);
            this.rigidBody.velocity = new Vector2(this.rigidBody.velocity.x, Vector2.up.y * this.JumpHeight);

            this.IsJumping = true;

            playerSoundSource.clip = jumpSound;
            this.playerSoundSource.Play();
        }

        // Glide function
        if (Input.GetKey(KeyCode.RightShift) && this.IsJumping == true)
        {
            if(!this.isGliding)
            {
                this.rigidBody.velocity = new Vector2(this.rigidBody.velocity.x, 0f);
            }
            
            //Debug.Log("Let's glide");
            this.rigidBody.gravityScale = this.glideGravity;
            this.isGliding = true;

            this.animator.SetBool("Planing", true);

            // Sound effect
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                this.playerSoundSource.clip = glideSound;
                this.playerSoundSource.Play();
            }
        }
        if (this.IsJumping == false || Input.GetKeyUp(KeyCode.RightShift))
        {
            this.rigidBody.gravityScale = this.initialGravity;
            this.isGliding = false;

            this.animator.SetBool("Planing", false);
            playerSoundSource.Stop();
        }

        this.transform.Translate(moveVector);
    }
    public void Kill()
    {
        this.gameObject.SetActive(false);

        this.OnDied();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        CollisionsManager.ResolveCollision(this.gameObject, col.gameObject, col);

        // For moving the player with the platform
        if (col.gameObject.CompareTag("Ground"))
        {
            transform.SetParent(col.gameObject.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // To stop moving the player with the platform
        if (collision.gameObject.CompareTag("Ground"))
        {
            transform.SetParent(null);
        }
    }

    private void OnDied()
    {
        if (this.Died != null)
            this.Died(this, new EventArgs());
    }
}
