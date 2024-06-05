using Godot;
using System;
using System.Collections.Generic;
using static Godot.TextServer;

public partial class EnemyState : State
{
	public EnemyStateMachine machine;
	public AnimatedSprite2D animator;
	public AnimatedSprite2D statusAnimator;
	public Enemy body;
    public int moveDirection = 0;
    public float speedMultiplier = 1;
    public bool jumping = false;
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        machine = (EnemyStateMachine)message["Machine"];
        animator = (AnimatedSprite2D)message["Animator"];
        statusAnimator = (AnimatedSprite2D)message["StatusAnimator"];
        body = (Enemy)message["Body"];
    }
    public override void UpdateState(float delta)
    {
      
        base.UpdateState(delta);
        if (body.hitPoints > 0)
        {
            Vector2 velocity = body.Velocity;
            if (body.wallDetection.GetCollider() != null )
            {
                bool detectingDropthrough = false;
                if((body.jumpDetection.GetCollider() != null))
                {

                TileMap hit = (TileMap)body.jumpDetection.GetCollider();
                    TileData hitData = hit.GetCellTileData(0, hit.GetCoordsForBodyRid(body.jumpDetection.GetColliderRid()));

                    detectingDropthrough = (bool)hitData.GetCustomData("DropThroughTiles");
                }
                if (body.wallDetection.GetCollider().IsClass("TileMap"))
                {
                    if (body.IsOnFloor() && !jumping && !(body.jumpDetection.GetCollider() == null && !detectingDropthrough))
                    {
                        
                        jumping = true;
                        velocity.Y = body.jumpPower;
                    }
                }
            }

            if (moveDirection > 0)
            {
                animator.FlipH = true;

            }
            else if (moveDirection < 0)
            {
                animator.FlipH = false;

            }
            body.DoMove(delta, new Vector2((body.speed * speedMultiplier) * moveDirection * (delta * 70), velocity.Y));
            if (body.IsOnFloor() && jumping)
            {
                jumping = false;
            }
        }

    }

    


}
