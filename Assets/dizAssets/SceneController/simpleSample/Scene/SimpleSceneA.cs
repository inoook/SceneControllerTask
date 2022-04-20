using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SimpleSceneA : SceneBase
{
	[SerializeField] CanvasGroup canvasGroup = null;
	FadeTransition fade;

	protected override void OnSetup()
	{
		fade = new FadeTransition(canvasGroup);
	}

	protected override void OnInitScene()
	{

	}

	protected override async Task OnOpenScene()
	{
		await fade.StartFade(1, 1);
	}

	protected override async Task OnCloseScene()
	{
		await fade.StartFade(1, 0);
	}
}
