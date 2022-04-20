using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class Scene01_1 : SceneBase {

	public GUISpriteAnimation paneSpriteAnim;
	
	protected override void OnInitScene()
	{
		paneSpriteAnim.InitHide();
	}
	
	protected override async Task OnOpenScene()
	{
		await paneSpriteAnim.Appear();
	}
	
	protected override async Task OnCloseScene()
	{
		await Task.CompletedTask;
	}
}
