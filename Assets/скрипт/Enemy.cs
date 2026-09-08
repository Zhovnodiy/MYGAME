using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;                // Посилання на гравця
    public float moveSpeed = 3f;            // Швидкість руху
    public float attackDistance = 0.7f;     // Дистанція для атаки
    public int health = 100;                // Здоров'я ворога
    public int attackDamage = 10;           // Шкода, яку завдає ворог
    public float attackCooldown = 2f;       // Час затримки між атаками в секундах

    private float lastAttackTime = 0f;      // Час останньої атаки
    private bool isAttacking = false;       // Змінна для контролю атаки
    private Animator animator;              // Аніматор ворога

    private void Start()
    {
        animator = GetComponent<Animator>(); // Отримання компонента Animator
    }

    private void Update()
    {
       
        if (health <= 0)
        {
            Die();
            return; // Вихід, якщо ворог мертвий
        }

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer > attackDistance)
            {
                MoveTowardsPlayer(); // Рух до гравця
            }
            else if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
            {
                StartAttack(); // Атака гравця
            }
        }
        
    }

    private void MoveTowardsPlayer()
    {
        // Обчислюємо напрямок до гравця
        Vector2 direction = (player.position - transform.position).normalized;

        // Рух монстра до гравця
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        // Поворот монстра залежно від позиції гравця по осі X
        if (player.position.x < transform.position.x)
        {
            Debug.Log("Повертаємо монстра вліво"); // Додано для тестування
            transform.localScale = new Vector3(-1f, 1f, 1f); // Повертаємо вліво
        }
        else
        {
            Debug.Log("Повертаємо монстра вправо"); // Додано для тестування
            transform.localScale = new Vector3(1f, 1f, 1f); // Повертаємо вправо
        }

        // Увімкнення анімації руху
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }
    }



    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Запуск анімації атаки
        if (animator != null)
        {
            animator.SetTrigger("attack");
        }

        // Нанесення шкоди гравцю (потрібно, щоб у гравця був метод TakeDamage)
        if (player.TryGetComponent<PlayerControlls>(out PlayerControlls playerScript))
        {
            playerScript.TakeDamage(attackDamage);
        }

        // Завершуємо атаку після короткої затримки
        Invoke(nameof(EndAttack), 0.5f); // Затримка для завершення анімації атаки
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die"); // Запуск анімації смерті
        }

        Destroy(gameObject, 1f); // Видаляємо ворога після затримки
    }
}
