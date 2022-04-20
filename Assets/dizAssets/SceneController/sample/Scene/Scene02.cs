using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class Scene02 : SceneBase {

	public GUISpriteAnimation paneSpriteAnim;

	protected override void OnInitScene()
	{
		paneSpriteAnim.InitHide();
	}
	
	protected override async Task OnOpenScene()
	{
//		paneSpriteAnim.InitAppear();
//		OpenComplete();
		await paneSpriteAnim.Appear();
	}
	
	protected override async Task OnCloseScene()
	{
		//		paneSpriteAnim.InitHide();
		//		CloseComplete();
		await paneSpriteAnim.Disappear();
	}
}
