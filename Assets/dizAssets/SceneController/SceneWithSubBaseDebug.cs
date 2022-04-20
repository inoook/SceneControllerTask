using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class SceneWithSubBaseDebug : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[SerializeField]
	Rect drawRect = new Rect(10,10,200,200);

	[SerializeField] SceneWithSubBase sceneWithSubBase = null;

	void OnGUI()
	{
		GUILayout.BeginArea (drawRect);
		SceneBase[] scenes = sceneWithSubBase.GetScenes ();
		for (int i = 0; i < scenes.Length; i++) {
			SceneBase s = scenes[i];
			if (GUILayout.Button (s.name)) {
                sceneWithSubBase.SetScene (i);
			}
		}
        if (GUILayout.Button("Close")) {
            sceneWithSubBase.ClearScene();
        }
		//
		GUILayout.Space(20);
		for (int i = 0; i < scenes.Length; i++)
		{
			SceneBase s = scenes[i];
			if (GUILayout.Button("async: " + s.name))
			{
                _ = SetSceneAsync(i);
			}
		}
		GUILayout.EndArea ();
	}
	async Task SetSceneAsync(int id)
	{
		await sceneWithSubBase.SetSceneAsync(id);
		Debug.LogWarning("///// Complete /////");
	}
}
