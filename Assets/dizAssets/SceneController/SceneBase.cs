using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class SceneBase : MonoBehaviour {
	
	public delegate void SceneEventHandler(SceneBase scene);
	public event SceneEventHandler eventOpenTransitionInterruptComplete;
	public event SceneEventHandler eventCloseTransitionInterruptComplete;

	[SerializeField] protected SceneController sceneController; // このSceneControllerがこのSceneBaseの遷移を管理する。

	/**
	 * OnSetup (起動時に１回のみ)
	 * シーン遷移時に呼ばれる順序
	 * 
	 * OnInitScene
	 * OnOpenScene
	 * OpenComplete (Dispatch)
	 * OnOpenSceneComplete
	 * 
	 * OnCloseScene
	 * CloseComplete (Dispatch)
	 * OnCloseSceneComplete
	 * OnInitScene
	 * 
	 **/

	public void SetParentScene(int id)
	{
		sceneController.SetScene (id);
	}

	// 起動時にSceneControllerから一度だけ呼ばれる。
	[SerializeField] private bool isSetup = false;
	public void Setup(SceneController sceneController_)
	{
		if (!isSetup) {
			sceneController = sceneController_;

            OnSetup ();
			isSetup = true;
		}else{
			Debug.LogWarning ("Setupは起動時のみよばれます。");
		}
	}

	// セットアップ時
	protected virtual void OnSetup()
	{
		
	}

	// 初期状態へ
	// シーンの初期化時にSceneControllerから呼ばれる。
	// シーンの開始時／シーン終了時
	public void Initialization()
	{
		OnInitScene();
	}

	// SceneController から呼ばれる
	public async Task OpenSceneAsync()
	{
		await OnOpenScene ();
		OnOpenSceneComplete();
	}
	// SceneController から呼ばれる
	public async Task CloseScene()
	{
		await OnCloseScene();
		OnCloseSceneComplete();
	}
	//public void OpenTransitionInterrupt()
	//{
	//	OnOpenTransitionInterrupt ();
	//}
	//public void CloseTransitionInterrupt()
	//{
	//	OnCloseTransitionInterrupt ();
	//}



	// ----------
	// override
	// 初期化(シーンの開始時／シーン終了時)の処理はこの関数をoverrideして書く
	// override する関数は OnXXX としています。
	protected virtual void OnInitScene()
	{
		SceneController.Log ("[" + this.gameObject.name + "] no Implementation 'OnInitScene'");
	}
	// シーントランジション、オープンの開始
	protected virtual async Task OnOpenScene()
	{
		SceneController.Log ("[" + this.gameObject.name + "] no Implementation 'OnOpenScene'");
		await Task.CompletedTask; // "正常終了"を表すTaskオブジェクト
	}
	// シーントランジション、クローズの開始
	protected virtual async Task OnCloseScene()
	{
		SceneController.Log ("[" + this.gameObject.name + "] no Implementation 'OnCloseScene'");
		await Task.CompletedTask; // "正常終了"を表すTaskオブジェクト
	}
	
	// シーン表示完了時何か実行する時
	protected virtual void OnOpenSceneComplete()
	{
		SceneController.Log ("[" + this.gameObject.name + "] no Implementation 'OnOpenSceneComplete'");
	}
	
	// シーン終了完了時何か行う時
	protected virtual void OnCloseSceneComplete()
	{
		SceneController.Log ("[" + this.gameObject.name + "] no Implementation 'OnCloseSceneComplete'");
	}

	public SceneController GetSceneController()
	{
		return sceneController;
	}

	#region complete
	//protected void OpenTransitionInterruptComplete()
	//{
	//	OnOpenTransitionInterruptComplete();

	//	if(eventOpenTransitionInterruptComplete != null){
	//		eventOpenTransitionInterruptComplete(this);
	//	}
	//}

	//protected void CloseTransitionInterruptComplete()
	//{
	//	OnCloseTransitionInterruptComplete();

	//	if(eventCloseTransitionInterruptComplete != null){
	//		eventCloseTransitionInterruptComplete(this);
	//	}
	//}
	#endregion
	// 表示シーンの割り込み時
	//public virtual void OnOpenTransitionInterrupt()
	//{
	//	SceneController.Log ("[" + this.gameObject.name + "] no Implementation 'OnOpenTransitionInterrupt'");
	//	Initialization ();
	//	OpenTransitionInterruptComplete ();
	//}
	//public virtual void OnCloseTransitionInterrupt()
	//{
	//	SceneController.Log ("[" + this.gameObject.name + "] no Implementation 'OnCloseTransitionInterrupt'");
	//	Initialization ();
	//	CloseTransitionInterruptComplete ();
	//}

	//protected virtual void OnOpenTransitionInterruptComplete()
	//{

	//}
	//protected virtual void OnCloseTransitionInterruptComplete()
	//{

	//}
}
