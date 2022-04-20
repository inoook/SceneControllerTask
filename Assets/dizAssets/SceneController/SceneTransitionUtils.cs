﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionUtils {

	public class TransitionInfo
	{
		public SceneController controller;
		public int sceneId;

		public TransitionInfo (SceneController controller, int sceneId)
		{
			this.controller = controller;
			this.sceneId = sceneId;
		}
	}

	#region static

	/// <summary>
	/// 指定 scene までのPathの取得
	/// </summary>
	/// <returns>The scene path.</returns>
	/// <param name="rootSceneController">Root scene controller.</param>
	/// <param name="scene">Scene.</param>
	public static List<TransitionInfo> GetScenePath (SceneController rootSceneController, SceneBase scene)
	{
		List<TransitionInfo> toRootScenes = new List<TransitionInfo> ();
		SceneBase baseScene = GetParentScenes (scene, toRootScenes);
        Debug.LogWarning("GetParentScenes: "+baseScene + " / "+scene);

		int sceneId = rootSceneController.GetSceneIdByScene (baseScene);
        if (sceneId > -1) {
            TransitionInfo sceneAndId = new TransitionInfo(rootSceneController, sceneId);
            toRootScenes.Add(sceneAndId);
        }
		toRootScenes.Reverse ();

		return toRootScenes;
	}

	public static SceneBase GetParentScenes (SceneBase scene, List<TransitionInfo> toRootScenes)
	{
		if (scene.transform.parent == null) { 
			Debug.LogWarning("ROOT: " + scene.gameObject.name);
			return scene;
		}

		SceneController sceneController = scene.GetSceneController();
		if (sceneController == null) {
			Debug.LogWarning ("NULL parentSceneController: "+ scene.gameObject.name);
			return scene;
		}
		
		if (sceneController != null) {
			int id = sceneController.GetSceneIdByScene (scene);
			if (id > -1) {
				TransitionInfo sceneAndId = new TransitionInfo (sceneController, id);
				toRootScenes.Add (sceneAndId);

                // TODO: 非表示の時に取得できない問題あり。解決する必要あり。
                //SceneBase parentSceneBase = sceneController.gameObject.GetComponentInParent<SceneBase>();
                SceneBase parentSceneBase = sceneController.GetParentScene();
                if (parentSceneBase != null) {
                    Debug.LogWarning("&&& sceneController: " + sceneController.gameObject.name + " / parentSceneBase: " + parentSceneBase.name);
                }

				if (parentSceneBase != null) {
					return GetParentScenes (parentSceneBase, toRootScenes);
				}else{
					SceneBase mySceneBase = sceneController.gameObject.GetComponent<SceneBase>();
					return mySceneBase;
				}
			} else {
				Debug.LogWarning ("Error SceneId: " + scene);
				return scene;
			}
		}
		return scene;
	}

	//
	public static List<TransitionInfo> GetTransitionPath (List<TransitionInfo> fromPath, List<TransitionInfo> toPath)
	{
		List<TransitionInfo> newPath = new List<TransitionInfo>();
		int len = toPath.Count;

		// 同じScneControllerを探し、新しくPathを作成
		for (int i = len-1; i >= 0; i--) {
			TransitionInfo toScAndId = toPath[i];
			SceneController toSceneController = toScAndId.controller;
			//
			for (int j = fromPath.Count-1; j >= 0; j--) {
				TransitionInfo fromScAndId = fromPath[j];
				SceneController fromSceneController = fromScAndId.controller;

				if (toSceneController == fromSceneController) {
					int from_index = j;
					int to_index = i;

					fromPath.RemoveRange (0, from_index+1);
					fromPath.Reverse ();
					newPath.AddRange (fromPath);
					
					toPath.RemoveRange (0, to_index);
					newPath.AddRange (toPath);
					return newPath;
				}
			}
		}
		return null;
	}
	
	/// <summary>
	/// toScene まで自動遷移
	/// </summary>
	/// <param name="rootSceneController">Root scene controller.</param>
	/// <param name="toScene">To scene.</param>
	public static void TransrateScene(SceneController rootSceneController, SceneBase toScene)
	{
		SceneBase fromScene;
		if (GetCurrentScene (rootSceneController, out fromScene)) {
			TransrateSceneFromTo (rootSceneController, fromScene, toScene);
		}else{
			Debug.LogWarning ("error");
		}
	}

	private static void TransrateSceneFromTo(SceneController rootSceneController, SceneBase fromScene, SceneBase toScene)
	{
        Debug.LogWarning("fromScene: "+ fromScene + " / toScene: " + toScene);
		List<TransitionInfo> fromPath = GetScenePath(rootSceneController, fromScene);
		List<TransitionInfo> toPath = GetScenePath(rootSceneController, toScene);
        Debug.Log(">>> fromPath");
        for (int i = 0; i < fromPath.Count; i++) {
            Debug.Log(fromPath[i].controller.gameObject.name + " / " + fromPath[i].sceneId);
        }

        Debug.Log(">>> toPath");
        for (int i = 0; i < toPath.Count; i++) {
            Debug.Log(toPath[i].controller.gameObject.name + " / " + toPath[i].sceneId);
        }
        List<TransitionInfo> path = GetTransitionPath(fromPath, toPath);
        
        Debug.Log(">>> path");
        for (int i = 0; i < path.Count; i++) {
            Debug.Log(path[i].controller.gameObject.name + " / "+path[i].sceneId);
        }

        if (path != null) {
			TransitionContent (path, 0);
		}else{
			Debug.LogWarning ("error invalid path.");
		}
	}

	public static void TransitionContent (List<TransitionInfo> toRootScenes, int index)
	{
		TransitionInfo setScene = toRootScenes [index];

		SceneController controller = setScene.controller;
		int sceneId = setScene.sceneId;
		
		SceneController.TransitionHandler handler = null;
		handler = delegate(SceneBase s, SceneController.TransitionType transitionType) {
            //if (transitionType == SceneController.TransitionType.OPEN_COMP) {
            if (transitionType == SceneController.TransitionType.OPEN_COMP) {
                Debug.LogWarning("***** OPEN_COMP: " + s);
				controller.eventTransition -= handler;
				//controller.isAutoTransition = false;

				index ++;
				if (index < toRootScenes.Count) {
					TransitionContent (toRootScenes, index);
				} else {
                    Debug.LogWarning("***** TransitionContent END");
					return;
				}
			}
		};
		controller.eventTransition += handler;
		Debug.Log("***** TransitionContent: " + controller.gameObject.name+ " / "+sceneId);

		//controller.isAutoTransition = true;
		controller.TransitionScene (sceneId);
	}

	public static bool GetCurrentScene (SceneController rootSceneController, out SceneBase currentScene)
	{
		SceneController sceneController;
		SceneBase scene = rootSceneController.currentScene;
		bool isSub = (scene is SceneWithSubBase);
		if (isSub) {
			sceneController = ((SceneWithSubBase)scene).GetSubSceneController ();

			if (sceneController != null) {
				if (sceneController.currentScene != null) {
					return GetCurrentScene (sceneController, out currentScene);
				} else {
					currentScene = scene;
					return true;
				}
			}
		}
		currentScene = scene;
		return true;
	}
	#endregion

}
