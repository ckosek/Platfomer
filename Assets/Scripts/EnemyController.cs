using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float maxHealth, knockbackSpeedX, knockbackSpeedY, knockbackDuration;
    [SerializeField]
    private bool applyKnockback;
    [SerializeField]
    private GameObject hitParticle;

    private float currentHealth, knockbackStart;
    private int playerFacingDir;
    private bool playerOnLeft, knockback;
    private PlayerController pc;
    private GameObject AliveGO;
    private Rigidbody2D AliveRB;
    private Animator aliveAnim;

    private void Start()
    {
        currentHealth = maxHealth;
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        AliveGO = transform.Find("Alive").gameObject;
        aliveAnim = AliveGO.GetComponent<Animator>();
        AliveRB = AliveGO.GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        CheckKnockback();

    }
    
    private void Damage(float amount)
    {
        currentHealth -= amount;
        playerFacingDir = pc.GetFacingDir();
        Instantiate(hitParticle, aliveAnim.transform.position, Quaternion.Euler(0.0f,0.0f, Random.Range(0.0f, 360.0f)));
        if(playerFacingDir == 1)
        {
            playerOnLeft = true;
        }
        else
        {
            playerOnLeft = false;
        }
        aliveAnim.SetBool("playerOnLeft", playerOnLeft);
        aliveAnim.SetTrigger("Damage");
        if(applyKnockback && currentHealth > 0.0f)
        {
            //Knockback
            Knockback();
        }

        if(currentHealth <= 0.0f)
        {
            // Death
            Die();
        }
    }

    private void Knockback()
    {
        knockback = true;
        knockbackStart = Time.time;
        AliveRB.velocity = new Vector2(knockbackSpeedX*playerFacingDir, knockbackSpeedY);

    }

    private void CheckKnockback()
    {
        if(Time.time >= knockbackStart + knockbackDuration && knockback)
        {
            knockback = false;
            AliveRB.velocity = new Vector2(0.0f, AliveRB.velocity.y);
        }
    }

    private void Die()
    {
        aliveAnim.SetTrigger("Dead");
        AliveGO.SetActive(false);
    }
}
