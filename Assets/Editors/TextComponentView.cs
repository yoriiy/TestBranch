using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

// シーン内のテキストコンポーネントの拡張エディタウィンドウ表示
public class TextComponentView : EditorWindow
{
    Vector2 scrollPosition = Vector2.zero;      // 一覧のスクロール位置

    Text[] texts = new Text[0];
    List<string> objNames = new List<string>(); // テキストを定義しているオブジェクト

    // テキストメッセージビュー更新
    [MenuItem("Tools/Text Component Viewer")]
    static void Open()
    {
        GetWindow<TextComponentView>(); // ウィンドウ作成(<>内がウィンドウ名)
    }

    void OnGUI()
    {
        
        // ボタン表示
        if(GUILayout.Button("テキストコンポーネント一覧　表示"))
        {
            objNames.Clear();
            this.texts = this.GetTextComponents().ToArray();
        }

        // テキストコンポーネント一覧表示
        this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
        int objnum = 0;
        foreach (var text in this.texts)
        {
            if (text == null) { continue; } // nullチェック
            // オブジェクト毎のテキストを表示
            EditorGUILayout.ObjectField(objNames[objnum], text, typeof(Text), false);
            GUILayout.Space(2f);
            // オブジェクト毎のテキスト内容を表示
            text.text = EditorGUILayout.TextArea(text.text);
            GUILayout.Space(10f);
            objnum++;
            // アンドゥ時の値を保存しておく
            // これによりInspector側に変更を認識させることができます
            Undo.RecordObject(text, text.text);

            //EditorUtility.SetDirty(text);
        }
        EditorGUILayout.EndScrollView();
    }

    private IEnumerable<Text> GetTextComponents()
    {
        var gameObjects = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject)); // シーン内の全てのGameObject

        foreach (var gob in gameObjects)
        {
            if(gob.GetComponent<Text>() != null)
            {
                objNames.Add(gob.transform.name);          // オブジェクト名を格納
                yield return gob.GetComponent<Text>();     // テキストコンポーネントを取得
            }
        }
    }
}
