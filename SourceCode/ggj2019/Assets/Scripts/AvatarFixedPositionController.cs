using UnityEngine;

public class AvatarFixedPositionController : AvatarController
{
    private new AvatarStates state = AvatarStates.Normal;
    public new AvatarStates State
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
    public AvatarStates FixedState;

    public float CurrentDirectionX = 1f;
    public float CurrentDirectionY = 1f;
    internal new Vector2 currentDirection
    {
        get
        {
            return new Vector2(this.CurrentDirectionX, this.CurrentDirectionY);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        this.tag = Tags.PLAYER_FIXED_POSITION;

        //this.rigidBody.gravityScale = 0.0f;

        //Duplicate Fixed State into State property.
        this.State = this.FixedState;
    }

    protected override void Update()
    {
        //this.rigidBody.gravityScale = 0.0f;

        //Duplicate Fixed State into State property.
        this.State = this.FixedState;

        base.Update();
    }
}