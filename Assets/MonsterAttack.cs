using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 2f; // Затримка між атаками (2 секунди)
    private float lastAttackTime = -Mathf.Infinity; // Час останньої атаки
    private Coroutine damageCoroutine; // Корутіна для нанесення шкоди
    private float playerStayTime = 0f; // Час, протягом якого гравець перебуває в зоні атаки
    private Collider2D currentPlayer; // Зберігаємо посилання на гравця

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Перевірка, чи гравець зайшов у зону
        {
            currentPlayer = other; // Зберігаємо посилання на гравця
            animator.SetTrigger("attack");
            Debug.Log("Гравець зайшов в зону атаки.");
            playerStayTime = 0f; // Скидаємо таймер
            // Запускаємо корутіну для відстеження часу перебування гравця
            damageCoroutine = StartCoroutine(TrackPlayerStayTime());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Перевірка, чи гравець вийшов із зони
        {
            Debug.Log("Гравець вийшов з зони атаки.");
            // Зупиняємо корутіну при виході з зони
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
            currentPlayer = null; // Скидаємо посилання на гравця
        }
    }

    private IEnumerator TrackPlayerStayTime()
    {
        while (true)
        {
            playerStayTime += Time.deltaTime; // Збільшуємо час перебування гравця
            if (playerStayTime >= 0.5f) // Якщо гравець перебуває в зоні 0.5 секунди
            {
                TryAttack(); // Викликаємо атаку
                yield break; // Завершуємо корутіну
            }
            yield return null; // Чекаємо наступного кадру
        }
    }

    private void TryAttack()
    {
        // Перевіряємо, чи пройшло 2 секунди з моменту останньої атаки
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time; // Оновлюємо час останньої атаки
            animator.SetTrigger("attack"); // Запускаємо анімацію атаки
            Debug.Log("Монстр атакує.");

            // Викликаємо метод для нанесення шкоди через певний час (наприклад, через 0.5 секунди)
            Invoke(nameof(DealDamage), 0.5f); // Затримка на 0.5 секунди (підлаштуйте під тривалість вашої анімації)
        }
    }

    private void DealDamage()
    {
        if (currentPlayer != null)
        {
            // Завдаємо шкоди гравцеві
            currentPlayer.GetComponent<PlayerControlls>().TakeDamage(20); // Завдаємо 20 одиниць шкоди
            Debug.Log("Гравцеві завдано шкоду.");
        }
    }
}
