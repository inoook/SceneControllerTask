using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class SceneAA : SceneWithSubBase
{

    [SerializeField] CanvasGroup canvasGroup = null;
    FadeTransition fade;

    protected override void OnSetup() {
        fade = new FadeTransition(canvasGroup);
    }

    protected override void OnInitScene()
	{
		
	}
	
	protected override async Task OnOpenScene()
	{
        await fade.StartFade(3, 1);
	}
	
	protected override async Task OnCloseScene()
	{
        await fade.StartFade(3, 0);
    }

}
