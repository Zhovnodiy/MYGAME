using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogControlls : MonoBehaviour
{
    public Text dialogueText; // Текстовий бокс для відображення діалогу
    public GameObject dialoguePanel; // Панель діалогу
    public string[] dialogueSentences; // Масив реплік діалогу, доступний в інспекторі
    private Queue<string> sentences; // Черга для реплік

    void Start()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false); // Сховати панель на початку
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartDialogue(); // Запустити діалог
        }
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true); // Показати панель діалогу
        sentences.Clear(); // Очищення черги

        foreach (string sentence in dialogueSentences)
        {
            sentences.Enqueue(sentence); // Додаємо репліки в чергу
        }

        DisplayNextSentence(); // Відобразити першу репліку
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue(); // Завершити діалог, якщо реплік більше немає
            return;
        }

        string sentence = sentences.Dequeue(); // Отримати наступну репліку
        StopAllCoroutines(); // Зупинити попередню анімацію
        StartCoroutine(TypeSentence(sentence)); // Показати текст по літерах
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = ""; // Очищення тексту перед виведенням
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter; // Додаємо по одній літері
            yield return new WaitForSeconds(0.05f); // Затримка між літерами
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false); // Сховати панель по завершенні
    }
}