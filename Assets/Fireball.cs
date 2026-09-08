using UnityEngine;

public class Fireball : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Перевірка на колізію з об'єктом з тегом "Player"
        {
            Debug.Log("Снаряд боса влучив у гравця!");
            Destroy(gameObject); // Знищити снаряд після зіткнення
        }
    }
}
