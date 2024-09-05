using Godot;
using System;

public partial class SettingsWindow : Control
{
	public bool windowOpen = false;
    AnimationPlayer animator;

    HSlider master, music;
    TextureButton xButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        master = GetNode<HSlider>("TextureRect/Master");
        music = GetNode<HSlider>("TextureRect/Music");
        xButton = GetNode<TextureButton>("TextureRect/Close");

        
        master.Value = Mathf.DbToLinear(PlayerPrefs.GetValue("MasterVolume", master.MaxValue));
        music.Value = Mathf.DbToLinear(PlayerPrefs.GetValue("MusicVolume", music.MaxValue/2));

        master.ValueChanged += UpdateMasterVolume;
        master.DragEnded += SaveAudioSettings;

        music.ValueChanged += UpdateMusicVolume;
        music.DragEnded += SaveAudioSettings;

        xButton.Pressed += Animate;
    }

    public void UpdateMasterVolume(double value)
    {
        //value / maxValue = wantedValue/maxWantedValue


        var wantedVolume = Mathf.LinearToDb(value);


        AudioServer.SetBusVolumeDb(0, (float)wantedVolume);
    }
    public void UpdateMusicVolume(double value)
    {
        var wantedVolume = Mathf.LinearToDb(value);

        AudioServer.SetBusVolumeDb(1, (float)wantedVolume);
    }

    void SaveAudioSettings(bool isNewValue)
    {
        if(isNewValue)
        {
            PlayerPrefs.SetFloat("MasterVolume", AudioServer.GetBusVolumeDb(0));
            PlayerPrefs.SetFloat("MusicVolume", AudioServer.GetBusVolumeDb(1));

            var masterLinearVolume = Mathf.LinearToDb(AudioServer.GetBusVolumeDb(0));
            var musicLinearVolume = Mathf.LinearToDb(AudioServer.GetBusVolumeDb(1));
        }
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}


	public void Animate()
	{

            if (animator.IsPlaying())
            {

                animator.SpeedScale *= -1;
            }
            else
            {
                animator.SpeedScale = 1;


                if (windowOpen)
                {
                    animator.PlayBackwards("In");

                }
                else
                {
                    animator.Play("In");

                }
            }
        windowOpen = !windowOpen;

    }

}
