using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] Powerup powerup;
    
    private Animator animator;
    private PlayerMovement movement;

    enum Powerup
    {
        None,
        DoubleJump,
        Slide
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        animator.SetTrigger("Open");
        GetComponent<BoxCollider2D>().enabled = false;
        ApplyPowerup();
    }

    private void ApplyPowerup()
    {
        switch (powerup)
        {
            case Powerup.DoubleJump:
                GameManager.instance.ActivateDoubleJump();
                Debug.Log("activated double jump");
                break;
            default:
                Debug.Log("No powerup selected");
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            movement = collision.GetComponent<PlayerMovement>();
        }
    }

}
