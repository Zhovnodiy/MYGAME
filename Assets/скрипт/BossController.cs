using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public Transform player;  // Посилання на гравця

    public float moveSpeed = 2f;  // Швидкість руху боса
    public float attackDistance = 5f;  // Дистанція для атаки
    public float summonDistance = 10f;  // Дистанція для призову монстрів
    public float fireballSpeed = 5f;  // Швидкість фаєрболів
    public GameObject fireballPrefab;  // Префаб фаєрболу
    public GameObject monsterPrefab;  // Префаб монстра

    public Animator animator;  // Аніматор боса
    public int HealsBoss = 10;      // ХП боса  

    public Image bossHealthBar;  // Полоска здоров'я
    public Image bossHealthFill;  // Заповнення полоски здоров'я
    private bool isMusicPlaying = false;
    public AudioSource bossMusic;
    public AudioSource oHello;
    public float fadeSpeed = 1.0f;

    public Camera mainCamera;

    // Нові змінні для керування атаками
    public float attackInterval = 3f;  // Інтервал між атаками
    private bool isAttacking = false;   // Стан атаки

    private bool hasBeenSeen = false; // Додано: щоб перевірити, чи бос уже в камері

    void Start()
    {
        bossHealthBar.gameObject.SetActive(false);
        StartCoroutine(AttackRoutine());  // Запускаємо корутину для атаки
        StartCoroutine(SummonMonstersRoutine()); // Запускаємо корутину для спавну монстрів
        bossMusic.volume = 0;
        oHello.volume = 0;

        bossMusic.Play();
    }

    void Update()
    {
        CheckIfBossInCameraView();

        if (Input.GetKeyDown(KeyCode.M)) // Перевірка натискання клавіші M
        {
            TakeDamage(1); // Зменшуємо здоров'я на 1 (або інше значення урону)
        }

        if (HealsBoss <= 0)
        {
            animator.SetTrigger("dead"); // Запускаємо анімацію смерті
            return; // Вихід, якщо бос мертвий
        }

        MoveTowardsPlayer();
    }

    private void CheckIfBossInCameraView()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool isInView = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        bossHealthBar.gameObject.SetActive(isInView);

        if (isInView)
        {
            Debug.Log("босс в камері");
            if (!isMusicPlaying)
            {
                bossMusic.volume = 1.0f; // Устанавливаем громкость на максимум
                bossMusic.Play();
                isMusicPlaying = true;
            }

            // Додано: перевірка, чи бос уже в камері
            if (!hasBeenSeen)
            {
                oHello.volume = 1.0f;
                oHello.Play(); // Граємо аудіо при першому вході в камеру
                hasBeenSeen = true; // Тепер бос вважається баченим
            }
        }
        else
        {
            Debug.Log("босс не в камері");
            if (bossMusic.volume > 0)
            {
                bossMusic.volume -= fadeSpeed * Time.deltaTime;
            }
            else
            {
                bossMusic.Stop();
                isMusicPlaying = false;
            }
        }

        // Оновлення значення здоров'я на полосці здоров'я
        if (bossHealthFill != null)
        {
            bossHealthFill.fillAmount = (float)HealsBoss / 100; // 10 - максимальне здоров'я боса (замініть на своє максимальне значення)
        }
    }

    private void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > attackDistance && distanceToPlayer <= 20f)
        {
            // Рухаємо боса тільки, якщо відстань до гравця більша за attackDistance
            if (distanceToPlayer > attackDistance)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

                // Змінюємо напрямок боса в залежності від позиції гравця
                if (player.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f); // Повертаємо боса вліво
                }
                else
                {
                    transform.localScale = new Vector3(1f, 1f, 1f); // Повертаємо боса вправо
                }
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (HealsBoss > 0)  // Виконуємо поки бос живий
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Атакуємо гравця, якщо він в межах attackDistance
            if (distanceToPlayer <= attackDistance)
            {
                animator.SetTrigger("Attack"); // Запускаємо анімацію атаки

                // Чекаємо завершення анімації атаки
                yield return new WaitForSeconds(1.5f); // Змініть це на час, який потрібен для завершення анімації

                FireFireball(); // Створюємо фаєрбол
            }

            // Чекаємо між атаками
            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void FireFireball()
    {
        // Створюємо фаєрбол і задаємо йому напрямок
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        fireball.GetComponent<Rigidbody2D>().velocity = direction * fireballSpeed; // Встановлюємо швидкість фаєрболу
        StartCoroutine(DestroyFireballAfterTime(fireball, 5f));
    }

    private IEnumerator DestroyFireballAfterTime(GameObject fireball, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(fireball); // Знищуємо фаєрбол
    }

    private IEnumerator SummonMonstersRoutine()
    {
        while (HealsBoss > 0)  // Виконуємо поки бос живий
        {
            // Генеруємо випадковий час спавну від 5 до 15 секунд
            float spawnTime = Random.Range(5f, 15f);
            yield return new WaitForSeconds(spawnTime);

            // Призиваємо монстрів
            SummonMonsters();
        }
    }

    private void SummonMonsters()
    {
        animator.SetTrigger("Summon");
        // Призиваємо монстра
        GameObject monster = Instantiate(monsterPrefab, transform.position, Quaternion.identity);
        // Отримуємо компонент Enemy і встановлюємо посилання на гравця
        Enemy enemyComponent = monster.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.player = player; // Встановлюємо посилання на гравця
        }
    }

    public void TakeDamage(int damage)
    {
        HealsBoss -= damage;  // Зменшуємо здоров'я боса на значення урону

        // Викликаємо анімацію "hit"
        animator.SetTrigger("Hit");

        // Перевіряємо, чи здоров'я боса впало до нуля
        if (HealsBoss <= 0)
        {
            animator.SetTrigger("dead"); // Запускаємо анімацію смерті
            StartCoroutine(DestroyBossAfterAnimation()); // Запускаємо корутину для знищення боса
        }
    }

    private IEnumerator DestroyBossAfterAnimation()
    {
        // Чекаємо, поки анімація смерті завершиться
        yield return new WaitForSeconds(2f); // Змініть на тривалість вашої анімації смерті

        Destroy(gameObject); // Знищуємо боса
        bossHealthBar.gameObject.SetActive(false);
    }
}
