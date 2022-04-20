using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Threading;

public class SceneController : MonoBehaviour
{
	public static void Log(string str){
		Debug.Log (str);
	}

	public enum TransitionType
	{
		OPEN_START,
		OPEN_COMP,
		CLOSE_START,
		CLOSE_COMP,

		SET_SCENE
	}

	public delegate void TransitionHandler (SceneBase scene,TransitionType transitionType);
	public event TransitionHandler eventTransition;// 画面遷移通知用
	
	public SceneBase currentScene = null; // 現在表示中のScene
	private SceneBase nextScene; // 次に表示しようとしているScene

	[SerializeField]
	private SceneBase[] scenes = null;

	public bool enableStartInitSceneId = true;
	[SerializeField] private int initSceneId = 0;
    [SerializeField] bool isInTransition = false;

    [SerializeField] SceneWithSubBase parentScene = null;//このシーンコントローラーを持つSceneBase

    bool isSetup = false;
    // Use this for initialization
    void Start ()
	{
        Setup();
    }
    
    public void SetParentScene(SceneWithSubBase scene) {
        parentScene = scene;
    }

    public SceneWithSubBase GetParentScene() {
        return parentScene;
    }

    public void Setup() {
        if (isSetup) { return; }
        isSetup = true;

        currentScene = null;
        nextScene = null;

        // init All Scene
        for (int i = 0; i < scenes.Length; i++) {
            SceneBase scene = scenes[i];
            scene.gameObject.SetActive(false);
            scene.enabled = false;
            scene.Setup(this);
            scene.Initialization();
        }
        //
        if (enableStartInitSceneId) {
            SetScene(initSceneId);
        }
    }

    // 最初のシーンをセットする。
    public void SetInitialScene()
	{
        SetScene(initSceneId);
    }

    //[HideInInspector]
    //public bool isAutoTransition = false;

    private CancellationTokenSource transitionSceneCancelTokenSource = null;

    [ContextMenu("Cancel")]
    void Cancel()
    {
        if (transitionSceneCancelTokenSource == null) { return; }

        transitionSceneCancelTokenSource.Cancel();
        Debug.LogWarning("Cancel");
    }

    public async Task TransitionScene(int sceneId)
	{
        transitionSceneCancelTokenSource = new CancellationTokenSource();
        try
        {
            isInTransition = true;

            if (sceneId > scenes.Length - 1)
            {
                // 不正なsceneId
                Debug.LogWarning("Id Error: " + this.gameObject.name + " / sceneId: " + sceneId);
                isInTransition = false;
                return;
            }

            if (sceneId < 0)
            {
                // オープンしているシーンを閉じる。
                Debug.LogWarning(">> ClearScene");
                await ClearScene();
                isInTransition = false;
                return;
            }

            SceneBase scene = scenes[sceneId];// 新しく表示するシーン
            if (nextScene == scene)
            {
                // 新しくセットするシーンが同じときは何もしない
                Debug.LogWarning($"[TransitionScene] SameScene scene: {scene}");
                isInTransition = false;
                return;
            }

            nextScene = scene; //新しくセットするシーン


            if (eventTransition != null)
            {
                eventTransition(scene, TransitionType.SET_SCENE);
            }

            // 次のシーンを開く。現在開いているシーン ( currentScene) があれば閉じる。
            if (currentScene != null)
            {
                // 現在のシーン ( currentScene) を閉じる処理開始
                // 完了後、nextSceneを表示
                Debug.LogWarning($"[TransitionScene] CloseScene currentScene: {currentScene}");

                await CloseScene(currentScene);

                Debug.LogWarning($"[TransitionScene] OpenScene nextScene: {nextScene}");
                // 次のシーン ( nextScene )があればオープンする。
                if (nextScene != null)
                {
                    await OpenScene(nextScene);
                }
            }
            else
            {
                Debug.LogWarning("[TransitionScene] 現在のシーン無し: " + this.gameObject.name + " / " + nextScene);
                await OpenScene(nextScene);
            }

            isInTransition = false;
        }
        catch (OperationCanceledException e)
        {
            Debug.Log($"[TransitionScene] Cancel {e}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        isInTransition = false;
        Debug.LogWarning("[TransitionScene] Complete");
    }

    public async Task SetSceneAsync (int sceneId)
	{
		//if (!isAutoTransition) {//自動遷移時にはユーザーの操作を無効に
			await TransitionScene (sceneId);
		//}
	}
    public void SetScene(int sceneId)
    {
        //if (!isAutoTransition) {//自動遷移時にはユーザーの操作を無効に
        _ = TransitionScene(sceneId);
        //}
    }

    // 表示している現在のシーンを閉じる
    public async Task ClearScene()
	{
		if (currentScene != null) {
            // 現在のシーンを閉じる処理開始
            Debug.LogWarning("ClearScene");

            nextScene = null;
            await CloseScene (currentScene);
        }
	}

    async Task OpenScene (SceneBase scene)
	{
        if (scene == null) { 
			Debug.LogError ("[OpenScene] Wrong Scene");
			return;
		}
        if (currentScene == scene) {
            Debug.LogWarning("[OpenScene] Same Scene #####");

            // 同じシーンであれば何もしない
            if (eventTransition != null) {
                eventTransition(currentScene, TransitionType.OPEN_COMP);
            }
            return;
        }
        //if (isInTransition) {
        //    Debug.LogWarning("########## isInTransition ##########");
        //    return;
        //}

        Debug.Log("シーンオープン開始: " + scene);
        currentScene = scene; // 現在開かれている（Open,Close時含め）シーン
        // 
        SceneOpenBegin(scene);

		await scene.OpenSceneAsync ();

        SceneOpenComplete(scene);
    }

    void SceneOpenBegin(SceneBase scene) {
        if (!scene.gameObject.activeSelf) {
            scene.gameObject.SetActive(true);
        }
        if (!scene.enabled) {
            scene.enabled = true;
        }

        scene.Initialization(); // シーンの初期化

        // EVENT / オープン開始
        if (eventTransition != null)
        {
            eventTransition(scene, TransitionType.OPEN_START);
        }
    }

    void SceneOpenComplete (SceneBase scene)
	{
        // シーンのトランジションが完了した時
        Debug.Log("シーンオープン完了: " +scene);

        // EVENT
        if (eventTransition != null) {
			eventTransition (scene, TransitionType.OPEN_COMP);
		}
    }

    //
    async Task CloseScene(SceneBase closeScene) {
        if (closeScene == null) {
            Debug.LogError("closeScene is null");
            return;
        }

        // 今のシーンと次に表示するシーンが同じ時は何もしない。
        if (currentScene == nextScene) {
            Debug.LogWarning("[CloseScene] Same Scene #####");
            if (eventTransition != null) {
                eventTransition(currentScene, TransitionType.OPEN_COMP);
            }
            return;
        }
        //
        //if (isInTransition) {
        //    Debug.LogWarning("########## isInTransition ##########");
        //    return;
        //}

        // シーンのクローズ開始
        SceneCloseBegin(closeScene);

        // シーンClose処理開始
        await closeScene.CloseScene();

        // シーンのクローズが完了
        SceneCloseComplete(closeScene);

        currentScene = null;//終了完了
    }

    void SceneCloseBegin(SceneBase closeScene)
    {
        Debug.Log("現在のシーンクローズ開始: " + closeScene);
        // EVENT
        if (eventTransition != null)
        {
            eventTransition(closeScene, TransitionType.CLOSE_START);
        }
    }
    
    /// <summary>
    /// シーンの非表示、初期化を行う
    /// </summary>
    /// <param name="closeScene"></param>
    void SceneCloseComplete(SceneBase closeScene) {
        Debug.Log("シーンクローズ完了: " + closeScene);

        // EVENT
        if (eventTransition != null)
        {
            eventTransition(closeScene, TransitionType.CLOSE_COMP);
        }

        if (closeScene.gameObject.activeSelf) {
            closeScene.gameObject.SetActive(false);
        }
        if (closeScene.enabled) {
            closeScene.enabled = false;
        }
        closeScene.Initialization();
    }

    #region cross
 //   public void SetSceneCross (int sceneId)
	//{
	//	SceneBase scene = scenes [sceneId];
	//	nextScene = scene;
	//	if (currentScene != null) {
	//		if (currentScene == scene) { 
	//			Debug.Log ("Same Scene");
	//			return;
	//		}

	//		currentScene.eventCloseComplete += HandleCurrentSceneEventEndCrossComplete;
	//		CloseScene (currentScene);

	//		// set nextScene
	//		if (nextScene != null) {
	//			OpenScene (nextScene);
	//		}

	//	} else {
	//		OpenScene (nextScene);
	//	}
	//}

	//void HandleCurrentSceneEventEndCrossComplete (SceneBase scene)
	//{
	//	scene.eventCloseComplete -= HandleCurrentSceneEventEndCrossComplete;

	//	if (scene.gameObject.activeSelf) {
	//		scene.gameObject.SetActive (false);
	//	}
	//	if (scene.enabled) {
	//		scene.enabled = false;
	//	}
	//}
    #endregion


    public int GetCurrentSceneId() {
        return GetSceneIdByScene(currentScene);
    }

    public int GetSceneIdByScene(SceneBase scene) {
        if (scene == null) {
            return -1;
        }
        for (int i = 0; i < scenes.Length; i++) {
            if (scene == scenes[i]) {
                return i;
            }
        }
        return -1;
    }

    public SceneBase[] GetScenes()
	{
		return scenes;
	}

	public void NextScene()
	{
		int current = GetCurrentSceneId ();
		current++;
		int nextId = (int)Mathf.Repeat (current, scenes.Length);
		SetScene (nextId);
	}

    [ContextMenu("Test_SetScene")]
    void Test_SetScene() {
        SetScene(initSceneId);
    }
    [ContextMenu("Test_TransitionScene")]
    void Test_TransitionScene() {
        _ = TransitionScene(initSceneId);
    }
    [ContextMenu("Test_ClearScene")]
    void Test_ClearScene() {
        _ = ClearScene();
    }
}

