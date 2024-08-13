using Godot;
using System;
using System.Linq;

public partial class PauseMenu : Control
{
    [Export]
    Fader fader;
    bool quitting;
        TextureButton quitButton;
        TextureButton resumeButton;
    AnimationPlayer animator;

    ColorRect backgroundFader;

    bool paused = false;

    ColorRect rect;

    AudioStreamPlayer audioPlayer;
    [Export] AudioStream pausedSound, playedSound;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        quitButton = GetNode<TextureButton>("TextureRect/VBoxContainer/QuitButton");
        resumeButton = GetNode<TextureButton>("TextureRect/VBoxContainer/ResumeButton");
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        backgroundFader = GetNode<ColorRect>("ColorRect");
        audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

        quitButton.Pressed += QuitPressed;
        resumeButton.Pressed += HandlePause;
        fader.FadedOut += QuitToMenu;
        //rect = GetNode<ColorRect>("ColorRect");
    }

    void ToggleBackground()
    {
        if(paused)
        {
            AudioServer.SetBusEffectEnabled(1, 0, true);

            backgroundFader.Show();
            audioPlayer.Stream = pausedSound;
            audioPlayer.Play();
        }
        else
        {
            AudioServer.SetBusEffectEnabled(1, 0, false);

            backgroundFader.Hide();
            audioPlayer.Stream = playedSound;
            audioPlayer.Play();
        }
    }
    
    public void PopOut()
    {

        if (animator.IsPlaying())
        {
            
                animator.SpeedScale *= -1;
        }
        else
        {
            animator.SpeedScale = 1;


            if (paused)
            {
                animator.Play("Out");

            }
            else
            {
                animator.Play("In");

            }
        }
    }

    void HandlePause()
    {
        
        paused = !paused;
        ToggleBackground();
        PopOut();
            
        GetTree().Paused = paused;
        if (paused)
        {
    
            GD.Print("Pausing");
            SaveGame();
        }
        else
        {
            GD.Print("Unpausing");
        }

    }

    private void SaveGame()
    {
        GD.Print("--SAVING--");
        SavePlayerPosition();
        SaveLadderPosition();
        SaveToolsAndChests();
        GD.Print("--END--");
    }

    private void SaveToolsAndChests()
    {
        foreach (Chest chest in GetTree().GetNodesInGroup("Chests").OfType<Chest>())
        {
            GD.Print("Saving " + chest.Name);
            if (chest.opened)
            {
                PlayerPrefs.SetBool(chest.Name + "Opened", true);

                switch (chest.treasure)
                {
                    case Chest.Treasure.Bow:
                        PlayerPrefs.SetBool("Bow", true);

                        break;


                    case Chest.Treasure.Ladder:
                        PlayerPrefs.SetBool("Ladder", true);
                        break;
                }
                GD.Print("-Saved");

            }
            else { GD.Print("-Not Opened"); }
        }
    }

    private void SaveLadderPosition()
    {
        GD.Print("Saving Ladder");

        Ladder ladder = (Ladder)GetTree().GetFirstNodeInGroup("Ladders");
        if (ladder != null)
        {
            PlayerPrefs.SetBool("LadderPlaced", true);
            PlayerPrefs.SetInt("LadderPositionX", (int)ladder.GlobalPosition.X);
            PlayerPrefs.SetInt("LadderPositionY", (int)ladder.GlobalPosition.Y);
            PlayerPrefs.SetFloat("LadderRotation", ladder.Rotation);
            GD.Print("-Saved");
        }
        else
        {
            PlayerPrefs.SetBool("LadderPlaced", false);
            GD.Print("-Not Placed");

        }
    }

    private void SavePlayerPosition()
    {
        Player player = (Player)GetTree().GetFirstNodeInGroup("Player");
        PlayerPrefs.SetInt("PositionX", (int)player.GlobalPosition.X);
        PlayerPrefs.SetInt("PositionY", (int)player.GlobalPosition.Y);
        PlayerPrefs.SetInt("CamPositionX", (int)player.cameraTrolley.GlobalPosition.X);
        PlayerPrefs.SetInt("CamPositionY", (int)player.cameraTrolley.GlobalPosition.Y);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("Pause"))
        {
            HandlePause();
        }

    }
    public void StartPressed()
    {
        fader.FadeOut();
    }

    public void QuitPressed()
    {
        fader.FadeOut();
        quitting = true;
        fader.MouseFilter = MouseFilterEnum.Stop;
    }

    void QuitToMenu()
    {
        if (quitting)
        {
            GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
        }


    }
}
