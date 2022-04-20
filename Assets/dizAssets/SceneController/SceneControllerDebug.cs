using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class SceneControllerDebug : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[SerializeField]
	Rect drawRect = new Rect(10,10,200,200);

	[SerializeField]
	SceneController sceneController;

	void OnGUI()
	{
		GUILayout.BeginArea (drawRect);
		SceneBase[] scenes = sceneController.GetScenes ();
		for (int i = 0; i < scenes.Length; i++) {
			SceneBase s = scenes[i];
			if (GUILayout.Button (s.name)) {
				sceneController.SetScene (i);
			}
		}

        if (GUILayout.Button("Close")) {
            _= sceneController.ClearScene();
        }
		if (GUILayout.Button("SetScene -1"))
		{
			sceneController.SetScene(-1);
		}
		GUILayout.Space(20);
		for (int i = 0; i < scenes.Length; i++)
		{
			SceneBase s = scenes[i];
			if (GUILayout.Button("async: "+s.name))
			{
				_ = SetSceneAsync(i);
			}
		}
		GUILayout.EndArea ();
	}

	async Task SetSceneAsync(int id)
    {
		await sceneController.SetSceneAsync(id);
		Debug.LogWarning("///// SetSceneAsync.Complete /////");
	}

	public SceneController GetSceneController()
	{
		return sceneController;
	}
}
