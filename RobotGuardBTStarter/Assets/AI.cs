using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;


public class AI : MonoBehaviour
{
    public Transform player;  //Player
    public Transform bulletSpawn; //Spawner da bala
    public Slider healthBar; //Barra de vida
    public GameObject bulletPrefab; //Prefab da bala

    NavMeshAgent agent; //Agent NavMesh
    public Vector3 destination; //Movimentação de destino 
    public Vector3 target; //Posição de mira 
    float health = 100.0f; //Valor da vida
    float rotSpeed = 5.0f; //Velocidade de rotação

    float visibleRange = 80.0f; 
    float shotRange = 40.0f; //Range do tiro

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5;
        InvokeRepeating("UpdateHealth", 5, 0.5f);
    }

    void Update() 
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);
    }

    void UpdateHealth() //Chama barra de vida 
    {
        if (health < 100)
            health++;
    }

    void OnCollisionEnter(Collision col) //Colisão
    {
        if (col.gameObject.tag == "bullet")
        {
            health -= 10; //Perda de 10 de vida caso seja atingido pela bala
        }
    }

    [Task]
    public void PickRandomDestination() //Método para destino aleatório
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void MoveToDestination() //Método de movimentação e destino
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }
}




