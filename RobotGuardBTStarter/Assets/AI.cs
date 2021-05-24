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



    [Task]
    public void PickDestination(int x, int z)
    { Vector3 dest = new Vector3(x, 0, z); 
        agent.SetDestination(dest); 
        Task.current.Succeed();
    }


    [Task] 
    public void TargetPlayer() 
    { 
        target = player.transform.position; //Pega o posicionamento do player
        Task.current.Succeed(); //Retorno da informação
    }
    [Task] 
    public bool Fire()
    { 
        GameObject bullet = GameObject.Instantiate(bulletPrefab, //instancia o prefab da bala contra o personagem
            bulletSpawn.transform.position, bulletSpawn.transform.rotation);

        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);

        return true; 
    }

    [Task] 
    public void LookAtTarget() //Movimento do personagem para mirar para o objeto
    {
        Vector3 direction = target - this.transform.position; //Posicionamento do personagem

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,//Rotação para atirar
         Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

        if (Task.isInspected) // Volta contra o player
            Task.current.debugInfo = string.Format("angle={0}",
                Vector3.Angle(this.transform.forward, direction)); 

        if (Vector3.Angle(this.transform.forward, direction) < 5.0f) //Execução do tiro
        {
            Task.current.Succeed();
        }
    }

    [Task]
    bool SeePlayer() //Observar o player
    {
        Vector3 distance = player.transform.position - this.transform.position; //Vetor de distância entre o player e o NPC
        RaycastHit hit; //Raycast para identificação
        bool seeWall = false; //Identificação das paredes
        Debug.DrawRay(this.transform.position, distance, Color.red); 
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                seeWall = true;
            }
        }
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("wall{0}", seeWall);

        if (distance.magnitude < visibleRange && !seeWall)
            return true;
        else
            return false;
    }

    [Task]
    bool Turn(float angle) //Mudança de ângulo para posição
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        target = p;
        return true;
    }



}




