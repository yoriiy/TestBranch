using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneOpenWindow : EditorWindow {

    // シーン一覧を表示するウィンドウスクリプト
	[MenuItem("Window/SceneOpenWindow")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow<SceneOpenWindow>();
    }

    private string[] _scenePaths; // シーンファイルパス

    private void OnEnable()
    {
        // 表示されているときはシーンリストを追加
        EditorApplication.projectWindowItemOnGUI += RefreshSceneList;
        RefreshSceneList(string.Empty, new Rect());
    }

    private void OnDisable()
    {
        // 表示されていないときはシーンリストを解除
        EditorApplication.projectWindowItemOnGUI -= RefreshSceneList;
    }

    // EditorWindow内のインターフェイス
    private void OnGUI()
    {
        // 格納されているシーンのパスを取得
        foreach(var scenePath in _scenePaths)
        {
            // シーン名のボタンを作成
            if (GUILayout.Button (scenePath))
            {
                // 各ボタンをクリックしたら表示されているシーンを開く
                EditorApplication.OpenScene(scenePath);
            }
        }
        // スペースを空ける
        EditorGUILayout.Space();
        
        // 新規シーン作成ボタン
        if(GUILayout.Button("New Scene"))
        {
            EditorApplication.NewScene();   // 新しいシーンを開く
        }
    }

    // guidアセットリストから全シーンパスを取得
    private void RefreshSceneList (string unusedArg1, Rect unusedArg2)
    {
        // AssetDatabaseからSceneアセットを検索してguidを取得
        string[] guids = AssetDatabase.FindAssets("t:Scene");
        // シーン個数分のパスの配列作成
        _scenePaths = new string[guids.Length];
        for(var i = 0; i < guids.Length; i++)
        {
            // シーンのパスをguidから取得
            _scenePaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
        }
    }

}
