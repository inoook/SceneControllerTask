using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class SceneA : SceneWithSubBase
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

    // test
    //[ContextMenu("Test")]
    //void Test() {
    //    var scene = this.gameObject.GetComponentInParent<SceneBase>();
    //    Debug.LogWarning(scene.gameObject.name);
    //}

}
