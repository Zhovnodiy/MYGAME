using UnityEngine;

public class Fireball2 : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}