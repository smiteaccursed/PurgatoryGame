using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string Name;
    public int Strength;
    public int Speed;
    public int Initiative;
    public int Health;
    public int BaseCost;
    public IEnemyBehavior Behavior;

    protected NavMeshAgent agent;
    protected Transform target;

    public void Initialize(string name, int strength, int speed, int initiative, int health, int baseCost, IEnemyBehavior behavior)
    {
        Name = name;
        Strength = strength;
        Speed = speed;
        Initiative = initiative;
        Health = health;
        BaseCost = baseCost;
        Behavior = behavior;
    }

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = Speed;
        GameObject[] playerGO = GameObject.FindGameObjectsWithTag("Player");
        if(playerGO[0]!=null)
        {
            target = playerGO[0].transform;
        }
    }

    protected virtual void Update()
    {
        if(target!=null)
        {
           // Behavior.Execute(this, target);
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if(Health<=0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

}

