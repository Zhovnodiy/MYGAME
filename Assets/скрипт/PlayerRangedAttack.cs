using UnityEngine;

public class PlayerRangedAttack : MonoBehaviour
{
    [Header("Fireball")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float fireballSpeed = 8f;
    [SerializeField] private float attackCooldown = 0.3f;

    private Camera mainCamera;
    private float nextAttackTime;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            ShootFireball();
            nextAttackTime = Time.time + attackCooldown;
        }
    }
 
    private void ShootFireball()
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("Fireball Prefab is not assigned.");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("FirePoint is not assigned.");
            return;
        }

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        mouseWorldPosition.z = firePoint.position.z;

        Vector2 direction =
            (mouseWorldPosition - firePoint.position).normalized;

        GameObject fireball = Instantiate(
            fireballPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction * fireballSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Fireball у sprite sheet намальований головою в протилежний бік
        angle += 180f;

        fireball.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}