using Godot;
using System;
using System.Collections.Generic;

public partial class Sign : Area2D
{
	//[Export] string content;
	RichTextLabel text;
	Panel panel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += (node) => ShowText(node);
		BodyExited += (node) => HideText(node);

		panel = GetNode<Panel>("Panel");
		text = GetNode<RichTextLabel>("Panel/RichTextLabel");


		//text.Text = content;
	
		panel.Hide();
	}

	void ShowText(Node2D node)
	{
            panel.Show();

    }

void HideText(Node2D node)
	{
		//GD.Print("Hidden");
            panel.Hide();

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
