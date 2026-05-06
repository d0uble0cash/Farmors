using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public event Action OnDamaged;
    public event Action OnDeath;
    public int health;
    public int maxHealth;


    private void Start()
    {
        health = maxHealth;
    }
    public void ChangeHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;

            else if(health <= 0)
                //Death
                OnDeath?.Invoke();

            else if(amount < 0)
                //Damage
                OnDamaged?.Invoke();
    }
}
