using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ポインタ(マウス/タッチ共通)のドラッグで狙い、離したときに球を発射する。
/// パチンコ式: 引いた方向と逆に飛ぶ。下に引くほど高く、横に引くと逆側へ、引く量が強さ。
/// </summary>
public class Shooter : MonoBehaviour
{
    [SerializeField] private ShooterConfig config;
    [SerializeField] private Projectile projectilePrefab;
    [Tooltip("球が出る位置。未設定ならこのオブジェクトの位置")]
    [SerializeField] private Transform muzzle;

    private bool dragging;
    private Vector2 dragStart;

    private void Update()
    {
        // Pointer は現在使用中のマウス/タッチ/ペンのうち最後に操作されたもの
        var pointer = Pointer.current;
        if (pointer == null) return;

        if (pointer.press.wasPressedThisFrame)
        {
            dragging = true;
            dragStart = pointer.position.ReadValue();
        }
        else if (dragging && pointer.press.wasReleasedThisFrame)
        {
            dragging = false;
            Vector2 drag = pointer.position.ReadValue() - dragStart;
            TryFire(drag);
        }
    }

    private void TryFire(Vector2 dragPixels)
    {
        if (config == null || projectilePrefab == null)
        {
            Debug.LogWarning("Shooter: config または projectilePrefab が未設定です。", this);
            return;
        }

        float screenH = Screen.height;
        float dragFraction = dragPixels.magnitude / screenH;
        if (dragFraction < config.minDragScreenFraction) return;

        // 強さ: ドラッグ量に比例し、min〜max にクランプ
        float t = Mathf.Clamp01(dragFraction / config.maxDragScreenFraction);
        float power = Mathf.Lerp(config.minPower, config.maxPower, t);

        Vector3 direction = ComputeDirection(dragPixels.normalized);

        Transform origin = muzzle != null ? muzzle : transform;
        Projectile p = Instantiate(projectilePrefab, origin.position, origin.rotation);
        p.Launch(direction * power, config.projectileMass);
    }

    /// <summary>引いた向き(正規化)から、このオブジェクト基準の発射方向を求める。</summary>
    private Vector3 ComputeDirection(Vector2 pullDir)
    {
        // 下に引く(pullDir.y = -1)ほど仰角が大きい
        float pitch = Mathf.Lerp(config.minPitchDeg, config.maxPitchDeg, Mathf.InverseLerp(1f, -1f, pullDir.y));
        // 左に引く(pullDir.x < 0)と右へ飛ぶ
        float yaw = -pullDir.x * config.maxYawDeg;
        // Unity では x 軸まわりの負の回転が「上向き」
        return transform.rotation * Quaternion.Euler(-pitch, yaw, 0f) * Vector3.forward;
    }
}
