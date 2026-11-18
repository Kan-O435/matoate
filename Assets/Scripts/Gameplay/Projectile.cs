using UnityEngine;

/// <summary>発射される球。発射と後始末を担当する。</summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Tooltip("発射からこの秒数で自動的に消える")]
    [SerializeField] private float lifetime = 10f;

    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        // 球は高速になりやすいので、すり抜け防止
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    /// <summary>質量を設定し、力積を加えて発射する。</summary>
    public void Launch(Vector3 impulse, float mass)
    {
        body.mass = mass;
        body.AddForce(impulse, ForceMode.Impulse);
        Destroy(gameObject, lifetime);
    }
}
