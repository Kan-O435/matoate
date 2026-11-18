using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Game シーン・ShooterConfig・Projectile Prefab を Unity の API で生成する。
/// メニュー Matoate > Build Game Scene、または -executeMethod GameSceneBuilder.Build で実行。
/// </summary>
public static class GameSceneBuilder
{
    private const string ConfigPath = "Assets/Config/ShooterConfig.asset";
    private const string PrefabPath = "Assets/Prefabs/Projectile.prefab";
    private const string ScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("Matoate/Build Game Scene")]
    public static void Build()
    {
        EnsureFolder("Assets/Config");
        EnsureFolder("Assets/Prefabs");

        var config = AssetDatabase.LoadAssetAtPath<ShooterConfig>(ConfigPath);
        if (config == null)
        {
            config = ScriptableObject.CreateInstance<ShooterConfig>();
            AssetDatabase.CreateAsset(config, ConfigPath);
        }

        var prefab = BuildProjectilePrefab();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(5f, 1f, 5f);

        // NewScene で未参照のオブジェクトがアンロードされることがあるため、読み込み直す
        config = AssetDatabase.LoadAssetAtPath<ShooterConfig>(ConfigPath);
        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);

        var shooterObj = new GameObject("Shooter");
        shooterObj.transform.position = new Vector3(0f, 1f, 0f);
        var shooter = shooterObj.AddComponent<Shooter>();
        var so = new SerializedObject(shooter);
        so.FindProperty("config").objectReferenceValue = config;
        so.FindProperty("projectilePrefab").objectReferenceValue = prefab.GetComponent<Projectile>();
        so.ApplyModifiedPropertiesWithoutUndo();

        var cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(0f, 2.5f, -5f);
            cam.transform.rotation = Quaternion.Euler(10f, 0f, 0f);
        }

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        Debug.Log("GameSceneBuilder: Game シーンを生成しました。");
    }

    private static GameObject BuildProjectilePrefab()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Projectile";
        go.transform.localScale = Vector3.one * 0.3f;
        go.AddComponent<Rigidbody>();
        go.AddComponent<Projectile>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabPath);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(Path.GetDirectoryName(path).Replace('\\', '/'), Path.GetFileName(path));
    }
}
