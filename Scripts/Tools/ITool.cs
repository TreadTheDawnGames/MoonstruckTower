using Godot;
using System;

public interface ITool 
{
	public bool useRelease { get; }
	public bool charged { get; }
	public bool animating { get;  }
	public string name { get;  }
	public bool Identify();
	public void Use(Vector2 direction = new()) { }

	public void PreUse(Vector2 direction = new()) { }

	public void WalkWhileUseAnim(Vector2 direction = new()) { }

	public void UpdateUseDirection(Vector2 direction = new()) { }

	public void SetupTool(AnimatedSprite2D character, player playerLink) { }
}
