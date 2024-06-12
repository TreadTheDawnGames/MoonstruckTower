using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static Godot.TextServer;


[GlobalClass]
public partial class player : CharacterBody2D
{
    [Export] public float Speed = 150.0f;
    [Export] public float climbSpeed = 75.0f;
    [Export] public float JumpVelocity = -270.0f;
    [Export] private float damageWaitTime = 0.5f;

    AnimatedSprite2D animator;
    Timer coyoteTimer;
    HitBox2D attackHitboxR;
    HitBox2D attackHitboxL;
    HitBox2D attackHitboxAirL;
    HitBox2D attackHitboxAirR;


    CollisionShape2D linkCollider;
    Area2D floorCheck;
    RayCast2D floorScan;
    Timer damageTimer;

    ITool selectedTool;
    ITool[] toolBagList;
    int toolBagItemCount;
    int selectedToolIndex = 0;
    Node toolBag;
    public bool usingTool = false;

    int coyoteFrames = 6;
    bool coyote = false;
    bool lastFloor = false;
    bool jumping = false;
    bool attacking = false;
    public bool touchingLadder = false;
    public int ladderCount = 0;
    public bool onLadder = false;
    bool takingDamage = false;
    int damageDirection = 0;

    
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {

        toolBag = (Node)GetNode(new NodePath("Toolbag"));
        floorCheck = (Area2D)GetNode(new NodePath("FloorCheck"));
        animator = (AnimatedSprite2D)GetNode(new NodePath("AnimatedSprite2D"));
        coyoteTimer = (Timer)GetNode(new NodePath("CoyoteTimer"));
        coyoteTimer.WaitTime = coyoteFrames / 60.0;
        attackHitboxR = (HitBox2D)GetNode(new NodePath("AnimatedSprite2D/HitBoxRight"));
        attackHitboxL = (HitBox2D)GetNode(new NodePath("AnimatedSprite2D/HitBoxLeft"));
        attackHitboxAirR = (HitBox2D)GetNode(new NodePath("AnimatedSprite2D/HitBoxAirRight"));
        attackHitboxAirL = (HitBox2D)GetNode(new NodePath("AnimatedSprite2D/HitBoxAirLeft"));
        linkCollider = (CollisionShape2D)GetNode(new NodePath("LinkCollider"));
        damageTimer = GetNode<Timer>("DamageTimer");

        damageTimer.Timeout += () => takingDamage = false;
        coyoteTimer.Timeout += () => CoyoteDone();
        animator.AnimationFinished += () => AnimationDone();
        damageTimer.Stop();


    }

   


    private void CoyoteDone()
    {
        coyote = false;
    }

    private void AnimationDone()
    {
        if (animator.Animation == new StringName("Attack") || animator.Animation == new StringName("AttackAir") || animator.Animation == new StringName("AttackLadder") || animator.Animation == new StringName("AttackWalk"))
        {
            attacking = false;
            

                attackHitboxR.SetEnabled(false);
                attackHitboxL.SetEnabled(false);
                attackHitboxAirL.SetEnabled(false);
                attackHitboxAirR.SetEnabled(false);

            
        }
        


    }

    bool jumpReleased = false;


    void UpdateToolbag()
    {
        List<ITool> toolList = new List<ITool>();
        foreach (Node tool in toolBag.GetChildren())
        {
            if ((bool)tool.Call("Identify"))
            {
                toolList.Add((ITool)tool);
            }

        }//toolBagList;
        foreach (ITool tool in toolList)
        {
            tool.SetupTool(animator, this);
            //GD.Print("Added " + tool.name + " to Toolbag");
        }
        toolBagList = toolList.ToArray();
        toolBagItemCount = toolList.Count;
        //GD.Print($"{toolBagItemCount} items in toolbag");
    }

    void HandleAttack(Vector2 direction)
    {
        if (Input.IsActionJustPressed("Attack") && !usingTool && !attacking)
        {
            bool flipped = animator.FlipH;
            attacking = true;
            if (IsOnFloor() && direction.X != 0)
            {
                animator.Play("AttackWalk");
            }
            else if (onLadder)
            {
                animator.Play("AttackLadder");
            }
            else if (!IsOnFloor() && floorCheck.GetOverlappingBodies().Count == 0)
            {
                animator.Play("AttackAir");
            }
            else
            {
                animator.Play("Attack");
            }


            if (animator.Animation == new StringName("Attack") || animator.Animation == new StringName("AttackWalk"))
            {
                attackHitboxR.SetEnabled(flipped);
                attackHitboxL.SetEnabled(!flipped);

            }

            if (animator.Animation == "AttackAir" || animator.Animation == "AttackLadder")
            {
                attackHitboxAirR.SetEnabled(flipped);
                attackHitboxAirL.SetEnabled(!flipped);


            }




        }
    }

    void HandleTool(Vector2 direction)
    {

        if (Input.IsActionJustPressed("Tool") && !attacking && !usingTool)
        {

            if (selectedTool != null)
            {
                if (!selectedTool.useRelease)
                {
                    selectedTool.Use(direction);
                   // GD.Print("Used " + selectedTool.name);
                }
                else
                {
                    selectedTool.PreUse(direction);
                   // GD.Print("PreUse for " + selectedTool.name);
                }
                usingTool = true;

            }
        }

        if (Input.IsActionJustReleased("Tool") && !attacking)
        {
            if (selectedTool != null)
            {
                if (selectedTool.useRelease)
                {
                    if (selectedTool.charged)
                        selectedTool.Use(direction);
                    else
                        usingTool = false;
                }
            }
        }
    }

    
    void HandleToolSwap()
    {
        if (Input.IsActionJustPressed("SwapTool"))
        {
            if (usingTool)
                usingTool = false;

            UpdateToolbag();
            if (toolBagList.Length != 0)
            {

                if (++selectedToolIndex > toolBagList.Length)
                {
                    selectedToolIndex = 1;
                }

                selectedTool = toolBagList[selectedToolIndex - 1];
               // GD.Print("Selected " + selectedTool.name);
            }
        }
    }
    void HandleDropthrough(float direction)
    {
        if (direction > 0 && Input.IsActionJustPressed("Jump") && IsOnFloor() && floorCheck.GetOverlappingBodies().Count == 0)
        {
            jumping = true;
            GlobalPosition += new Vector2(0, 2);
        }
    }


    
    void HandleCoyoteTime()
    {
        if (IsOnFloor() && jumping)
        {
            jumping = false;
        }

        if (!IsOnFloor() && lastFloor && !jumping)
        {

            coyote = true;
            coyoteTimer.Start();
        }

        lastFloor = IsOnFloor();
    }

    void HandleAnimation(Vector2 direction)
    {
        if (direction.X < 0)
        {
            animator.FlipH = false;
        }
        else if (direction.X > 0)
        {
            animator.FlipH = true;
        }

        bool flipped = animator.FlipH;

        if (IsOnFloor() && animator.Animation == "AttackAir")
        {
            attacking = false;
            attackHitboxAirL.SetEnabled(false);
            attackHitboxAirR.SetEnabled(false);


            //because the attack is canceled
        }

        if (!attacking && !usingTool)
        {

            if (IsOnFloor()&&!onLadder)
            {

                if (direction.X != 0f)
                {
                    animator.Play(new StringName("Walk"));
                }
                else if (direction.Y > 0)
                {
                    animator.Play(new StringName("Duck"));
                }
                else 
                {
                    animator.Play(new StringName("Idle"));
                }
            }
            else if (onLadder && direction.Y != 0)
            {
                animator.Play("LadderClimb");
            }
            else if (onLadder && direction.Y == 0)
            {
                animator.Play("LadderStay");
            }
            else  
            {
                animator.Play(new StringName("Jump"));
            }
        }

        if (usingTool && !selectedTool.animating)
        {
            selectedTool.UpdateUseDirection(direction);
        }

        if (((IsOnFloor() || onLadder) && usingTool && direction.X != 0f) && !selectedTool.animating)
        {
            selectedTool.WalkWhileUseAnim(direction);
        }

        
    }

    void SetClimb(bool setTo)
    {
        if (ladderCount > 0)
        {
            touchingLadder = true;

        }
        else
            touchingLadder = false;
    }

    public void TakeDamage(int amount, HitBox2D caller)
    {
        var direction = Mathf.Sign(GlobalPosition.X-caller.GlobalPosition.X);
        GD.Print("PLAYER TOOK DAMAGE");


        var hitVelX = Velocity.X;
        var hitVelY = Velocity.Y;



        if (floorCheck.GetOverlappingBodies().Count == 0)
        {
            hitVelX = direction * Speed*2;
            GlobalPosition += new Vector2(0, 2);
        }
        else
        {
            hitVelX = direction * Speed*2;
            hitVelY = -GD.RandRange(200, 300);

        }
        takingDamage = true;

        Velocity = new Vector2(hitVelX, hitVelY);
        
        damageTimer.WaitTime = damageWaitTime;
        damageTimer.Start();
    }


    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y += gravity * (float)delta;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = new Vector2(Mathf.Clamp(Input.GetAxis("MoveLeft", "MoveRight"), -1, 1), Input.GetAxis("ClimbUp", "ClimbDown"));//Input.GetVector("MoveLeft", "MoveRight", "ClimbUp", "ClimbDown"); Input.Get

        HandleAttack(direction);

        HandleTool(direction);

        HandleToolSwap();

        HandleDropthrough(direction.Y);

        if (touchingLadder && direction.Y != 0)
        {
            onLadder = true;
        }



        if (!touchingLadder)
        {
            onLadder = false;
        }
        if (onLadder)
        {
            velocity.Y = 0;
        }


        //HandleLadder
        if (onLadder && direction.Y != 0f)
        {
            GlobalPosition += direction;//new Vector2(direction.X, direction.Y * climbSpeed);
        }
        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && (IsOnFloor() || coyote) && !jumping)
        {
            velocity.Y = JumpVelocity;
            onLadder = false;
        }

        if (Input.IsActionJustReleased("Jump"))
        {
            if (velocity.Y < -75)
            {
                velocity.Y *= 0.6f;

            }
        }

        /*if (!IsOnFloor() && Input.IsActionPressed("Jump") && velocity.Y > 0)
        {
            velocity.Y += -1000 * (float)delta;
        }*/




        if (direction != Vector2.Zero && damageTimer.TimeLeft <= 0)
        {
            velocity.X = direction.X * (!onLadder ? Speed : climbSpeed);
        }
        else if (damageTimer.TimeLeft > 0 || (takingDamage && !IsOnFloor()))
        {

            velocity.X = (float)Mathf.Lerp(Velocity.X, 0, 0.15);

        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        if ((usingTool || direction.Y > 0) && damageTimer.TimeLeft <= 0)
        {
            velocity.X /= 2.0f;
        }


        HandleAnimation(direction);


        velocity.X = velocity.X * (float)delta * 70;


        Velocity = velocity;

        MoveAndSlide();

        HandleCoyoteTime();
    }


}