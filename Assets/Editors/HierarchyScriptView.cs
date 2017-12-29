using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

// シーン内スクリプトの拡張エディッタウィンドウ表示
public class HierarchyScriptView : EditorWindow
{
    Vector2 scrollPosition = Vector2.zero;      // 一覧のスクロール位置
    MonoScript[] scripts = new MonoScript[0];   // シーン内のスクリプト
    List<string> objNames = new List<string>(); // スクリプトを定義しているオブジェクト
    string includeScript = "";                  // 含まれるスクリプト文字
    string includeObject = "";                  // 含まれるオブジェクト文字
    string correctScript = "";                  // 添削するスクリプト文字
    string correctObject = "";                  // 添削するオブジェクト文字
    // カンマ区切りで分割して配列に格納する
    string[] stIncludeScriptData;
    string[] stIncludeObjectData;
    string[] stCorrectScriptData;
    string[] stCorrectObjectData;

    [MenuItem("Tools/Hierarchy Script Viewer")]
    static void Open()
    {
        GetWindow<HierarchyScriptView>();   // ウィンドウ作成(<>内がウィンドウ名)
    }


    void OnGUI()
    {
        GUILayout.Label("ヒエラルキー上で使用されている全スクリプトをオブジェクト毎に表示します");
        GUILayout.Space(2f);
        GUILayout.Label("※スクリプトが多くなると処理完了まで重くなる可能性があります。");
        GUILayout.Space(10f);
        GUILayout.Label("-検索する文字設定(フレーズ検索)-");
        //GUILayout.Space(1f);
        GUILayout.Label("スペースで区切ることで複数指定できます。(例:Manager Base)");
        GUILayout.Space(2f);
        includeObject = EditorGUILayout.TextField("検索するオブジェクト", includeObject);
        GUILayout.Space(2f);
        includeScript = EditorGUILayout.TextField("検索するスクリプト", includeScript);
        GUILayout.Space(10f);

        GUILayout.Label("-除外するアセット設定(マイナス完全一致検索)-");
        //GUILayout.Space(1f);
        GUILayout.Label("スペースで区切ることで複数指定できます。(例:Canvas Panel)");
        GUILayout.Space(2f);
        correctObject = EditorGUILayout.TextField("除外するオブジェクト", correctObject);
        GUILayout.Space(2f);
        correctScript = EditorGUILayout.TextField("除外するスクリプト", correctScript);
        GUILayout.Space(10f);

        // ボタン表示
        if (GUILayout.Button("スクリプト一覧 表示"))
        {
            objNames.Clear(); // ボタンを押した時に前回のオブジェクト名を解放
            this.scripts = this.GetScripts().ToArray();
        }

        // スクリプト一覧表示
        this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
        int objnum = 0;
        foreach (var script in this.scripts)
        {
            if (script == null) { continue; } // nullチェック
            // オブジェクト毎のスクリプトを表示
            EditorGUILayout.ObjectField(objNames[objnum], script, typeof(MonoScript), false);
            objnum++;
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// シーン内でアタッチされているすべての自作スクリプトを取得する
    /// </summary>
    private IEnumerable<MonoScript> GetScripts()
    {
        var gameObjects = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject)); // シーン内の全てのGameObject
        var monoScripts = Resources.FindObjectsOfTypeAll<MonoScript>(); // プロジェクト内の全てのスクリプト

        // スペースで文字を分ける
        stCorrectScriptData = correctScript.Split(' '); // 添削スクリプト文字を配列で分ける
        stCorrectObjectData = correctObject.Split(' '); // 添削オブジェクト文字を配列で分ける
        stIncludeScriptData = includeScript.Split(' '); // 検索スクリプト文字を配列で分ける
        stIncludeObjectData = includeObject.Split(' '); // 検索オブジェクト文字を配列で分ける

        bool matchWord = false;

        foreach (var gob in gameObjects)
        {
            foreach (var monoScript in monoScripts)
            {
                var classType = monoScript.GetClass();
                if (classType == null) { continue; }
                if (classType.Module.Name != "Assembly-CSharp.dll") { continue; } // 自作クラスかどうか
                if (!classType.IsSubclassOf(typeof(MonoBehaviour))) { continue; } // MonoBehaviour継承クラスかどうか
                // スクリプト名の除外
                foreach (string stData in stCorrectScriptData)
                {
                    if (stData == monoScript.name) { matchWord = true;break; } // スクリプト名除外の文字とスクリプト名が同じ場合は取得しない
                }
                if (matchWord) {    // 一致していたら取得しない
                    matchWord = false;
                    continue;
                }
                // オブジェクト名の除外
                foreach(string stData in stCorrectObjectData)
                {
                    if (stData == gob.name) { matchWord = true; break; } // オブジェクト名除外の文字とオブジェクト名が同じ場合は取得しない
                }
                if (matchWord)
                {    // 一致していたら取得しない
                    matchWord = false;
                    continue;
                }
                // スクリプト名の検索
                foreach (string stData in stIncludeScriptData)
                {
                    if (!monoScript.name.Contains(stData)) { matchWord = true; break; }
                }
                if (matchWord)
                {    // 一致してなかったら取得しない
                    matchWord = false;
                    continue;
                }
                // オブジェクト名の検索
                foreach (string stData in stIncludeObjectData)
                {
                    if (!gob.name.Contains(stData)) { matchWord = true; break; }
                }
                if (matchWord)
                {    // 一致してなかったら取得しない
                    matchWord = false;
                    continue;
                }
                if (gob.GetComponent(classType) != null) // GameObject側にスクリプトが存在するか
                {
                    objNames.Add(gob.transform.name);   // オブジェクト名を格納
                    yield return monoScript;            // スクリプトを取得
                }
            }
        } 
    }
}