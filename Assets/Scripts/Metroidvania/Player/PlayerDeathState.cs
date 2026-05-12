using UnityEngine;

public class PlayerDeathState : PlayerState
{
    private float knockbackVelocity;
    private float knockbackDuration;
    private bool isTimeSlow;
    private bool hasTriggeredUI = false;
    public PlayerDeathState(Player player) : base(player){}

    public override void Enter()
    {
        base.Enter();
        hasTriggeredUI = false;
        Time.timeScale = .3f;
        isTimeSlow = true;
        animator.SetBool("isDead", true);

        player.groundCheckRadius = .2f;
        knockbackDuration = damage.knockbackDuration;
        player.rb.linearVelocity = new Vector2(knockbackVelocity, player.rb.linearVelocity.y);
    }

    public void SetParameters(int knockbackDirection)
    {
        knockbackVelocity = knockbackDirection * damage.knockbackForce;
    }

    public override void FixedUpdate()
    {
        knockbackDuration -= Time.fixedDeltaTime;
        if (knockbackDuration <= 0)
        {
            if (isTimeSlow)
            {
                Time.timeScale = 1f;
                isTimeSlow = false;
            }
            if(player.isGrounded)
            {
                player.rb.linearVelocity = Vector2.zero;
                DeathScreenUI.Instance.Show();
                if(!hasTriggeredUI)
                {
                    hasTriggeredUI = true;
                    player.StartCoroutine(FaintRoutine());
                }
            }
        }
    }

    private System.Collections.IEnumerator FaintRoutine()
    {
        if (player.faintPanel != null) player.faintPanel.SetActive(true);
        if (player.faintText != null) player.faintText.text = "You fainted...";
        yield return new WaitForSeconds(player.faintDuration);
        
        if (SaveSystem.I != null) SaveSystem.I.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(player.farmSceneName);
    }


    public override void Exit()
    {
        base.Exit();
        animator.SetBool("isDead", false);
        player.groundCheckRadius = .35f;
    }

}