using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossAtack : StateMachineBehaviour
{
    public GameObject fireballPrefab; // Префаб фаєрбола
    public float attackRange = 5f; // Дистанція досяжності для атаки
    private Transform player; // Посилання на гравця
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null && Vector3.Distance(animator.transform.position, player.position) <= attackRange)
        {
            // Виконати атаку, якщо гравець в зоні досяжності
            // Логіка для атаки може бути реалізована тут
        }
    }

    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null && Vector3.Distance(animator.transform.position, player.position) <= attackRange)
        {
            SpawnFireball(animator.transform.position);
        }
    }
    private void SpawnFireball(Vector3 position)
    {
        // Спавн фаєрбола
        Instantiate(fireballPrefab, position, Quaternion.identity);
    }

}
