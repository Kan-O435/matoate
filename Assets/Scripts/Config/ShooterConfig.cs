using UnityEngine;

/// <summary>発射まわりの調整値。Project ウィンドウで右クリック > Create > Matoate > Shooter Config。</summary>
[CreateAssetMenu(fileName = "ShooterConfig", menuName = "Matoate/Shooter Config")]
public class ShooterConfig : ScriptableObject
{
    [Header("強さ")]
    [Tooltip("発射する力(インパルス)の最小値")]
    public float minPower = 2f;
    [Tooltip("発射する力(インパルス)の最大値")]
    public float maxPower = 12f;
    [Tooltip("この割合(画面の高さに対する比)までドラッグすると最大の強さになる")]
    [Range(0.05f, 1f)] public float maxDragScreenFraction = 0.3f;
    [Tooltip("これ未満のドラッグ量(画面の高さに対する比)は発射しない(誤タップ防止)")]
    [Range(0f, 0.2f)] public float minDragScreenFraction = 0.02f;

    [Header("狙い")]
    [Tooltip("真下に引いたときの仰角(度)")]
    [Range(0f, 89f)] public float maxPitchDeg = 60f;
    [Tooltip("真上に引いたときの仰角(度)")]
    [Range(0f, 89f)] public float minPitchDeg = 5f;
    [Tooltip("真横に引いたときの左右の振れ幅(度)")]
    [Range(0f, 90f)] public float maxYawDeg = 40f;

    [Header("球")]
    public float projectileMass = 0.5f;
}
