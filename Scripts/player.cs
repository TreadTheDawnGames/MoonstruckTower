using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static Godot.TextServer;


[GlobalClass]
public partial class Player : CharacterBody2D
{
    [Export] public float Speed = 150.0f;
    [Export] public float climbSpeed = 75.0f;
    [Export] public float JumpVelocity = -270.0f;
    [Export] private float damageWaitTime = 0.5f;


    AnimatedSprite2D animator;
    Timer coyoteTimer;
    HitBox2D attackHitboxL;
    HitBox2D attackHitboxAirL;
    HitBox2D attackHitboxR;
    HitBox2D attackHitboxAirR;


    CollisionShape2D playerCharCollider;
    Area2D floorCheck;
    RayCast2D floorScan;
    Timer damageTimer;

    Node2D flippables;

    [Export] float fallTimeMax = 20;
    float fallTimeCounter = 0;

    int cameraPanDownCounter = 0;
    public Marker2D cameraTrolley { get; private set; }
    Vector2 cameraDefaultPosition;
    Vector2 cameraDownPosition;
    // Vector2 cameraUpPosition;

    AudioPlayer audioPlayer;

    [Export]
    Godot.Collections.Dictionary<string, AudioStream> sounds;

    CameraSmoother camera;
    bool cameraPan = false;

    ITool selectedTool;
    public ITool[] toolBagList { get; private set; }
    int toolBagItemCount;
    int selectedToolIndex = 0;
    public Node2D toolBag;
    public bool usingTool = false;
    TextureRect toolBoxDisplay;

    int coyoteFrames = 6;
    bool coyote = false;
    bool lastFloor = false;
    bool jumping = false;
    bool attacking = false;
    public bool touchingLadder = false;
    public int ladderCount = 0;
    public bool onLadder = false;
    bool takingDamage = false;
    public bool flippedSwitchThisAnimation = false;

    [Export] float camSmoothY = 0.07f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    bool spawning = false;

    bool walkStep1 = true;


    public override void _Ready()
    {
        flippables = GetNode<Node2D>("Flippables");
        toolBag = GetNode<Node2D>(new NodePath("Flippables/Toolbag"));
        floorCheck = (Area2D)GetNode(new NodePath("FloorCheck"));
        animator = (AnimatedSprite2D)GetNode(new NodePath("AnimatedSprite2D"));
        coyoteTimer = (Timer)GetNode(new NodePath("CoyoteTimer"));
        coyoteTimer.WaitTime = coyoteFrames / 60.0;

        attackHitboxL = (HitBox2D)GetNode(new NodePath("HitBoxL"));
        attackHitboxAirL = (HitBox2D)GetNode(new NodePath("HitBoxAirL"));
        attackHitboxR = (HitBox2D)GetNode(new NodePath("HitBoxR"));
        attackHitboxAirR = (HitBox2D)GetNode(new NodePath("HitBoxAirR"));


        playerCharCollider = (CollisionShape2D)GetNode(new NodePath("RectangleCollider"));
        damageTimer = GetNode<Timer>("DamageTimer");
        toolBoxDisplay = (TextureRect)GetTree().GetFirstNodeInGroup("ToolBoxDisplay");
        cameraTrolley = GetNode<Marker2D>("CameraTrolley");
        camera = Owner.GetNode<CameraSmoother>("Camera2D");

        audioPlayer = GetNode<AudioPlayer>("AudioStreamPlayer2D");


        cameraDefaultPosition = cameraTrolley.Position;
        cameraDownPosition = cameraTrolley.Position;
        cameraDownPosition.Y += 96;

        damageTimer.Timeout += () => takingDamage = false;
        coyoteTimer.Timeout += () => CoyoteDone();
        animator.AnimationFinished += () => AnimationDone();
        damageTimer.Stop();
        UpdateToolbag();

        lastY = (int)GlobalPosition.Y;

        ToolSaveSys();
    }


    void ToolSaveSys()
    {
        List<PackedScene> tools = new();

        if (PlayerPrefs.GetBool("Bow"))
        {
            tools.Add(GD.Load<PackedScene>("res://Scenes/Tools/Bow/bow.tscn"));
        }
        if (PlayerPrefs.GetBool("Ladder"))
        {
            tools.Add(GD.Load<PackedScene>("res://Scenes/Tools/Ladder/ladder_spawner.tscn"));

        }
        foreach (PackedScene tool in tools)
        {
            var addedTool = tool.Instantiate();
            toolBag.AddChild(addedTool);
            UpdateToolbag();
        }
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
            flippedSwitchThisAnimation = false;

            attackHitboxL.SetEnabled(false);
            attackHitboxAirL.SetEnabled(false);
            attackHitboxR.SetEnabled(false);
            attackHitboxAirR.SetEnabled(false);
        }
    }

    bool jumpReleased = false;
    private int lastY = 0;

    public void UpdateToolbag()
    {
        List<ITool> toolList = new List<ITool>();
        foreach (Node tool in toolBag.GetChildren())
        {
            if ((bool)tool.Call("Identify"))
            {
                toolList.Add((ITool)tool);
            }
        }
        foreach (ITool tool in toolList)
        {
            tool.SetupTool(animator, this);
        }
        toolBagList = toolList.ToArray();
        toolBagItemCount = toolList.Count;
        HandleToolSwap();
    }

    void HandleAttack(Vector2 direction)
    {
        bool flipped = animator.FlipH;

        if (Input.IsActionJustPressed("Attack") && !usingTool && !attacking)
        {
            audioPlayer.PlaySound(sounds["Attack"]);
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
        }

        if (attacking)
        {
            if (animator.Animation == new StringName("Attack") || animator.Animation == new StringName("AttackWalk"))
            {
                if (flipped)
                {
                    attackHitboxL.SetEnabled(false);
                    attackHitboxR.SetEnabled(true);
                }
                else
                {
                    attackHitboxR.SetEnabled(false);
                    attackHitboxL.SetEnabled(true);
                }
            }

            if (animator.Animation == "AttackAir" || animator.Animation == "AttackLadder")
            {
                if (flipped)
                {
                    attackHitboxAirL.SetEnabled(false);
                    attackHitboxAirR.SetEnabled(true);
                }
                else
                {
                    attackHitboxAirR.SetEnabled(false);
                    attackHitboxAirL.SetEnabled(true);
                }
            }
        }
    }

    void HandleTool(Vector2 direction)
    {
        if (direction.X > 0)
        {
            flippables.Scale = new Vector2(-1, 1);
        }
        else if (direction.X < 0)
        {
            flippables.Scale = new Vector2(1, 1);
        }

        if (Input.IsActionJustPressed("Tool") && !attacking && !usingTool)
        {

            if (selectedTool != null)
            {
                if (selectedTool.useRelease)
                {
                    // GD.Print("Used " + selectedTool.name);
                    selectedTool.PreUse(direction);
                }
                else
                {
                    selectedTool.Use(direction);
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
        if (usingTool)
            usingTool = false;

        if (toolBagList.Length != 0)
        {
            if (++selectedToolIndex > toolBagList.Length)
            {
                selectedToolIndex = 1;
            }

            selectedTool = toolBagList[selectedToolIndex - 1];
            if (toolBoxDisplay != null)
                toolBoxDisplay.Texture = selectedTool.displayTexture;
            selectedTool.BecomeActiveTool();
        }
    }

    void HandleDropthrough(float direction)
    {
        if (direction > 0 && Input.IsActionJustPressed("Jump") && IsOnFloor() && !floorCheck.HasOverlappingBodies())
        {
            audioPlayer.PlaySound(sounds["FallThru"]);
            jumping = true;
            GlobalPosition += new Vector2(0, 2);
        }
    }

    void HandleCamera(Vector2 direction)
    {
        //if down control is held count up
        if (direction.Y > 0)
        {
            cameraPanDownCounter++;
        }
        //otherwise don't
        else
        {
            cameraPan = false;
            cameraPanDownCounter = 0;
        }

        //if the counter is done move the camera down
        if (cameraPanDownCounter > 20)
        {
            cameraTrolley.Position = cameraTrolley.Position.Lerp(cameraDownPosition, camSmoothY);
        }

        else
        {
            cameraTrolley.Position = cameraTrolley.Position.Lerp(cameraDefaultPosition, camSmoothY);

            //this makes it so the camera keeps up if you fall infinitely
            if (Velocity.Y > 0)
            {
                fallTimeCounter++;

            }
            else
            {
                fallTimeCounter = 0;
            }

            if (camera.lerpSpeedY < 1f)
            {
                camera.lerpSpeedY = Mathf.Lerp(camSmoothY, camSmoothY * 3f, fallTimeCounter / fallTimeMax);
            }
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

        if (direction.X != 0 && !audioPlayer.Playing && IsOnFloor())
        {
            walkStep1 = !walkStep1;
            audioPlayer.PlaySound(walkStep1 ? sounds["Walk1"] : sounds["Walk2"]);
        }

        if (direction.X < 0)
        {
            animator.FlipH = false;
        }
        else if (direction.X > 0)
        {
            animator.FlipH = true;
        }

        if (IsOnFloor() && animator.Animation == "AttackAir")
        {
            attacking = false;
            attackHitboxAirL.SetEnabled(false);
            attackHitboxAirR.SetEnabled(false);
            flippedSwitchThisAnimation = false;

            //because the attack is canceled
        }

        if (!attacking && !usingTool)
        {

            if (IsOnFloor() && !onLadder)
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
            else if (onLadder)
            {
                if (direction.X != 0)
                {
                    animator.Play("LadderClimbSide");
                }
                else if (direction.Y == 0 || onLadder && direction.Y != 0 && IsOnFloor())
                {
                    animator.Play("LadderStay");
                }

                else if (direction.Y != 0)
                {
                    if (direction.Y > 0)
                    {
                        animator.Play("LadderClimbDown");
                    }
                    else
                    {
                        animator.Play("LadderClimb");
                    }
                }
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
    /*
        void SetClimb(bool setTo)
        {
            if (ladderCount > 0)
            {
                touchingLadder = true;

            }
            else
                touchingLadder = false;
        }*/

    public void TakeDamage(int amount, HitBox2D box)
    {

        if (audioPlayer.Stream == null || !audioPlayer.Stream.ResourceName.Contains("TakeDamage"))
        {
            int rand = (int)(GD.Randi() % 3);
            AudioStream stream = sounds["TakeDamage1"];

            switch (rand)
            {
                case 0:
                    stream = sounds["TakeDamage1"];
                    break;
                case 1:
                    stream = sounds["TakeDamage2"];
                    break;
                case 2:
                    stream = sounds["TakeDamage3"];
                    break;

            }

            audioPlayer.PlaySound(stream);
        }
        var direction = Mathf.Sign(GlobalPosition.X - box.GlobalPosition.X);
        GD.Print("PLAYER TOOK DAMAGE");

        var hitVelX = Velocity.X;
        var hitVelY = Velocity.Y;

        if (floorCheck.GetOverlappingBodies().Count == 0 && IsOnFloor())
        {
            hitVelX = direction * Speed * 2;
            GlobalPosition += new Vector2(0, 2);
        }
        else
        {
            hitVelX = direction * Speed * 2;
            hitVelY = GD.RandRange(6, 12);
            float jumpInPixels = -Mathf.Sqrt(2 * gravity * hitVelY);

            hitVelY = jumpInPixels;


        }
        takingDamage = true;

        Velocity = new Vector2(hitVelX * 1.25f, hitVelY) * amount;

        damageTimer.WaitTime = damageWaitTime;
        damageTimer.Start();

        if (box.Owner is Projectile)
        {
            //get arrow owner script /detect if box was on projectile and if it was, delete it
            // direction = -direction;
            Projectile arrow = box.Owner.GetNode<Projectile>(box.Owner.GetPath());

            arrow.HitHurtBox();

        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;
        }
        else
        {
            lastY = (int)GlobalPosition.Y;
            if (GlobalPosition.Y >= lastY + 75)
            {
                audioPlayer.PlaySound(sounds["FallThud"]);
            }
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = new Vector2(Mathf.Clamp(Input.GetAxis("MoveLeft", "MoveRight"), -1, 1), Input.GetAxis("ClimbUp", "ClimbDown"));//Input.GetVector("MoveLeft", "MoveRight", "ClimbUp", "ClimbDown"); Input.Get

        HandleAttack(direction);

        HandleTool(direction);

        if (Input.IsActionJustPressed("SwapTool"))
        {
            HandleToolSwap();
        }

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
            if (IsOnWall())
            {
                direction *= 1.01f;
            }
            GlobalPosition += direction;//new Vector2(direction.X, direction.Y * climbSpeed);

            if (!audioPlayer.Playing && !IsOnFloor())
            {
                walkStep1 = !walkStep1;
                audioPlayer.PlaySound(walkStep1 ? sounds["Climb1"] : sounds["Climb2"]);
            }
        }
        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && (IsOnFloor() /*|| onLadder*/ || coyote) && !jumping)
        {
            audioPlayer.PlaySound(sounds["Jump"]);
            jumping = true;
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
            if (IsOnFloor())
            {
                velocity.X = (float)Mathf.Lerp(Velocity.X, 0, 0.20);

            }
            else
            {
                velocity.X = (float)Mathf.Lerp(Velocity.X, 0, 0.15);
            }

        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        if ((usingTool || direction.Y > 0) && damageTimer.TimeLeft <= 0)
        {
            velocity.X *= 0.5f;
        }

        HandleAnimation(direction);

        HandleCamera(direction);

        velocity.X = velocity.X * (float)delta * 70;

        Velocity = velocity;

        MoveAndSlide();

        HandleCoyoteTime();
    }
}