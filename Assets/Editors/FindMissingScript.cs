using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// シーン内スクリプトの拡張エディッタウィンドウ表示
public class FindMissingScript
{
    [MenuItem("Assets/Find Missing Scripts")]
    public static void SearchAllPrefabs()   // 全てのアセット(prefabs)の検索
    {
        // パスからゲームオブジェクト取得
        var gameObjects = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject)); // シーン内の全てのGameObject

        foreach (var gob in gameObjects)
        {
            if (HasMissingScript(gob))
            {
                Debug.LogError(gob.name, gob);
            }
        }
    }

    // 設定が(Missing Script)になっているゲームオブジェクトを検出
    public static bool HasMissingScript(GameObject go)
    {
        // 取得したゲームオブジェクトのコンポーネントを取得
        Component[] components = go.GetComponentsInChildren<Component>(true);
        for (int k = 0; k < components.Length; k++)
        {
            // コンポーネントが空でなければ取得成功
            if (components[k] == null)
            {
                return true;
            }
        }
        return false;
    }

}