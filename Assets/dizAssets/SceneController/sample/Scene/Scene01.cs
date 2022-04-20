using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class Scene01 : SceneWithSubBase {
	
	public GUISpriteAnimation paneSpriteAnim;

	void Update()
	{
		
	}

	protected override void OnInitScene()
	{
		subSceneController.ClearScene ();

		paneSpriteAnim.InitHide();
	}
	
	protected override async Task OnOpenScene()
	{
		 await paneSpriteAnim.Appear(() => {
			subSceneController.SetScene(0);
		});
	}
	
	protected override async Task OnCloseScene()
	{
		subSceneController.ClearScene ();

		await paneSpriteAnim.Disappear();
	}

}
