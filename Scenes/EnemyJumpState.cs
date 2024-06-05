using Godot;
using System;

public partial class EnemyJumpState : EnemyState
{

    public override void UpdateState(float delta)
    {
        Vector2 velocity = body.Velocity;
        if (body.wallDetection.GetCollider() != null)
        {
            bool detectingDropthrough = false;
            if ((body.jumpDetection.GetCollider() != null))
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
        base.UpdateState(delta);


    }
}
