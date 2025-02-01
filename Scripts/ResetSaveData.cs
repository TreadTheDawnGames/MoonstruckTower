using Godot;
using System;

public partial class ResetSaveData : TexturePanel_YesNoDialog
{
	protected override void YesPressed()
	{
		PauseMenu.ResetSaveData();
		MyHide();
		OpenDialogButton.Disabled = false;
		MainMenu.instance.SetPlayButtonTextures();
	}

}
