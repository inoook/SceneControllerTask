using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

public class GUISpriteAnimation : MonoBehaviour {
	public enum AnimState{
		Appear_COMPLETE, Disapper_COMPLETE
	}

	public delegate void HandlerAnim(AnimState state);
	public event HandlerAnim eventAnimState;

	public GUISprite sprite;
	
	// Use this for initialization
	void Start () {
		
	}

	public void InitAppear()
	{
		this.gameObject.SetActive(true);
		
		CancelSpriteAlphaLT ();
		sprite.alpha = 1.0f;
		sprite.ForceUpdate();
		
		if(this.GetComponent<Collider>() != null){
			this.GetComponent<Collider>().enabled = true;
		}
	}
	public void InitHide()
	{
		CancelSpriteAlphaLT ();
		sprite.Init();
		sprite.alpha = 0.0f;
		sprite.ForceUpdate();
		
		if(this.GetComponent<Collider>() != null){
			this.GetComponent<Collider>().enabled = false;
		}
		
		this.gameObject.SetActive(false);
	}
	
	// alphaAnimation Only
	public async Task Appear(System.Action completeAct)
	{
		await Appear(1.5f, 0.0f, completeAct);
	}

	CancellationTokenSource fadeCancellationTokenSource = null;
	void CancelSpriteAlphaLT()
	{
		fadeCancellationTokenSource?.Cancel();
		fadeCancellationTokenSource = null;

		fadeCancellationTokenSource = new CancellationTokenSource();
	}

	public async Task Appear(float time = 1.5f, float delay = 0.0f, System.Action completeAct = null )
	{
		this.gameObject.SetActive(true);

		if(this.GetComponent<Collider>() != null){
			this.GetComponent<Collider>().enabled = true;
		}

		CancelSpriteAlphaLT ();

		float from = sprite.alpha;
		float to = 1;
		await Ez.EzTween.TweenAct(Ez.EzEaseType.Linear, from, to, time, (float v) => {
			sprite.alpha = v;
		}, null, fadeCancellationTokenSource.Token);

		if (eventAnimState != null)
		{
			eventAnimState(AnimState.Appear_COMPLETE);
		}
		completeAct?.Invoke();
	}


	public async Task Disappear(System.Action completeAct)
	{
		await Disappear(1.5f, 0.0f, completeAct);
	}
	public async Task Disappear(float time = 1.5f, float delay = 0.0f, System.Action completeAct = null)
	{
		if(this.GetComponent<Collider>() != null){
			this.GetComponent<Collider>().enabled = false;
		}

		CancelSpriteAlphaLT ();

		//spriteAlphaLT = LeanTween.value (sprite.alpha, 0.0f, time).setDelay (delay).setOnUpdate ((v) => {
		//	sprite.alpha = v;
		//});

		//await spriteAlphaLT;

		float from = sprite.alpha;
		float to = 0;
		await Ez.EzTween.TweenAct(Ez.EzEaseType.Linear, from, to, time, (float v) => {
			sprite.alpha = v;
		}, null, fadeCancellationTokenSource.Token);

		this.gameObject.SetActive(false);

		if (eventAnimState != null)
		{
			eventAnimState(AnimState.Disapper_COMPLETE);
		}
		completeAct?.Invoke();
	}
	
	public void DisableBtn()
	{
		if(this.GetComponent<Collider>() != null){
			this.GetComponent<Collider>().enabled = false;
		}
	}

	//public void AppearAndDisappear(float time = 0.5f, float appearTime = 0.0f)
	//{
	//	this.gameObject.SetActive(true);
		
	//	if(this.GetComponent<Collider>() != null){
	//		this.GetComponent<Collider>().enabled = true;
	//	}

	//	CancelSpriteAlphaLT ();

	//	spriteAlphaLT = LeanTween.value (sprite.alpha, 1.0f, time).setOnUpdate ((v) => {
	//		sprite.alpha = v;
	//	}).setOnComplete(() => {
	//		spriteAlphaLT = LeanTween.value (sprite.alpha, 0.0f, time).setOnUpdate ((v) => {
	//			sprite.alpha = v;
	//		}).setOnComplete(() => {
	//			if(eventAnimState != null){
	//				eventAnimState(AnimState.Disapper_COMPLETE);
	//			}
	//		});
	//	});

	//	float from = sprite.alpha;
	//	float to = 1;
	//	_ = Ez.EzTween.TweenAct(Ez.EzEaseType.Linear, from, to, time, (float v) => {
	//		sprite.alpha = v;
	//	}, null, fadeCancellationTokenSource.Token);
	//}

}
