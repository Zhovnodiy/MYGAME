using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;  // Додайте цей using для доступу до UI елементів

public class PlayerControlls : MonoBehaviour
{
    public float moveSpeed = 4;
    private bool isMoving,dead;
    private Vector3 input;
    public LayerMask solidObjectLayer;
    private Animator animator;
    public LayerMask interactablesLayer;
    public int Heals = 100;
    public Image healthBar;  // Додайте змінну для полоси здоров'я
    private const int maxHeals = 100; // Максимальне здоров'я
    private void Start()
    {
        StartCoroutine(RestoreHealthOverTime()); // Запускаємо корутину для відновлення здоров'я
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            if (dead != true)
            {
                // Отримання введення для руху
                input.x = Input.GetAxis("Horizontal");
                input.y = Input.GetAxis("Vertical");

                if (input != Vector3.zero)
                {
                    animator.SetFloat("MoveX", input.x);
                    animator.SetFloat("MoveY", input.y);
                    Vector3 targetPos = transform.position + input; // Додати введення до поточного положення персонажа

                    // Перевірка, чи можна рухатися в цільову позицію
                    if (isWalkable(targetPos))
                    {
                        StartCoroutine(Move(targetPos));
                    }
                }
            }
        }

        animator.SetBool("isMoving", isMoving);

        // Додайте перевірку на натискання клавіші T для взаємодії
        if (Input.GetKeyDown(KeyCode.T))
        {
            Interact();
        }
    }

    void Interact()
    {
        var facingfir = new Vector3(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"));
        var interactPos = transform.position + facingfir;
        var colider = Physics2D.OverlapCircle(interactPos, 0.2f, interactablesLayer);
        if (colider != null)
        {
            colider.GetComponent<Interactable>()?.Interact();
        }
    }

    // Корутина для плавного руху персонажа до нової позиції
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        // Рух до цільової позиції
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

    // Перевірка, чи можна пройти на задану позицію
    private bool isWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectLayer | interactablesLayer) != null)
        {
            return false;
        }
        return true;
    }

    // Метод для зменшення здоров'я
    public void TakeDamage(int damage)
    {
        Heals -= damage; // Зменшуємо здоров'я

        
        if (Heals < 0)
        {
            Heals = 0;
            
        }
        if(Heals == 0)
        {
            
            animator.SetTrigger("deadPlayer");
            dead = true;
        }

        // Оновлюємо значення здоров'я на полосці здоров'я
        UpdateHealthBar();
    }
    private IEnumerator RestoreHealthOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(50f); // Чекаємо 50 секунд

            if (Heals < maxHeals) // Якщо здоров'я нижче максимуму
            {
                Heals += 1; // Відновлюємо 1 одиницю здоров'я

                // Переконайтеся, що здоров'я не перевищує максимум
                if (Heals > maxHeals)
                {
                    Heals = maxHeals;
                }

                // Оновлюємо значення здоров'я на полосці здоров'я
                UpdateHealthBar();
            }
        }
    }
    // Метод для оновлення полоски здоров'я
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)Heals / maxHeals; // Оновлюємо заповнення полоски
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Перевіряємо, чи зіткнення з снарядом боса
        if (collision.CompareTag("BULLET")) // Замініть на ваш тег снаряда боса
        {
            TakeDamage(10); // Зменшуємо здоров'я на 10
            Destroy(collision.gameObject); // Знищуємо снаряд після зіткнення
        }
    }
}
