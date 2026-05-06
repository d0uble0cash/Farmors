using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Health health;
    public Animator anim;

    private void OnEnable() 
    {
        health.OnDamaged += HandleDamage;
    }

    private void OnDisable() 
    {
        health.OnDamaged -= HandleDamage;
    }

    void HandleDamage()
    {
        anim.SetTrigger("isDamaged");
    }
}
