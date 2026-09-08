using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class SoliderRUNforHero : MonoBehaviour
{
    public Transform hero; // Перетворення героя
    public float speed = 5f; // Швидкість бігу солдата
    public Vector3 stopPosition; // Позиція, де солдат повинен зупинитись
    private Animator animator;

    public GameObject dialogueCanvas; // Canvas для діалогу
    public Text dialogueText; // Поле тексту для діалогу
    private List<string> dialogues = new List<string>(); // Список фраз
    private int currentDialogueIndex = 0; // Індекс поточної фрази
    private bool dialoguePlayed = false; // Флаг для перевірки, чи діалог вже відбувся
    public GameObject elementToHide;
    public GameObject titleQuest1;
    private void Start()
    {
        // Знаходимо компонент Animator на тому ж об'єкті
        animator = GetComponent<Animator>();

        // Додаємо фрази до списку
        dialogues.Add("Привіт, герой!");
        dialogues.Add("Я тут, щоб допомогти тобі!");
        dialogues.Add("Давай, рухайся далі!");

        // Сховати Canvas на початку
        dialogueCanvas.SetActive(false);
        titleQuest1.SetActive(false);
    }

    private void Update()
    {
        // Обчислюємо відстань до зупинки
        float distanceToStop = Vector3.Distance(transform.position, stopPosition);

        // Якщо солдат не досягнув зупинки, рухаємось до неї
        if (distanceToStop > 0.1f)
        {
            // Обчислюємо напрямок до зупинки
            Vector3 direction = (stopPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            animator.SetBool("IDL", false);
        }
        else
        {
            animator.SetBool("IDL", true);

            // Якщо Canvas не активний та діалог ще не відбувся, активуємо його
            if (!dialogueCanvas.activeSelf && !dialoguePlayed)
            {
                dialogueCanvas.SetActive(true);
                dialogueText.text = dialogues[currentDialogueIndex];
            }

            // Зміна фрази при натисканні клавіші "T"
            if (Input.GetKeyDown(KeyCode.T))
            {
                currentDialogueIndex++;
                if (currentDialogueIndex < dialogues.Count)
                {
                    // Якщо ще залишились фрази, змінюємо текст
                    dialogueText.text = dialogues[currentDialogueIndex];
                }
                else
                {
                    // Якщо фрази закінчились, закриваємо Canvas
                    dialogueCanvas.SetActive(false);
                    dialoguePlayed = true; // Встановлюємо флаг, що діалог відбувся
                    elementToHide.SetActive(false);
                    titleQuest1.SetActive(true);
                }
            }
        }
    }
}