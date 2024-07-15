using Godot;
using System;

public partial class BossDeathPuppet : CharacterBody2D
{
	[Export] Curve positionCurveX;
	[Export] Curve positionCurveY;
	float xPosition, yPosition;
	[Export] float counterMax = 75;
	float counter = 1;

	Vector2 startPos;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
//		positionCurveX.Bake();
		//positionCurveY.Bake();
		counter = -counterMax;
		startPos = GlobalPosition;
		startPos.Y += 128;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float dasCounter = counter++ / counterMax;
		
		if(dasCounter > 0)
		{
	        yPosition = (Mathf.Pow(dasCounter,2f));
		}
		else
		{

	        yPosition = -(Mathf.Pow(dasCounter,2f));
		}
		GD.Print(counter / counterMax);
		GD.Print(yPosition);
		/*yPosition = positionCurveY.SampleBaked(counterMax/counter++);
		xPosition = positionCurveX.SampleBaked(xPosition);*/
		Vector2 position = new Vector2(0, yPosition);
		Rotate(Mathf.DegToRad(5));
		GlobalPosition += position*10;

		if (GlobalPosition > startPos)
		{
			QueueFree();
		}
	}
}
