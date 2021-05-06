using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{

    private Collider2D[] interactables;
    private int arrayCounter = 0;

    private void Start()
    {
        interactables = new Collider2D[5];
    }

    void OnInteract()
    {
        if (arrayCounter > 0)
        {
            interactables[arrayCounter - 1].gameObject.GetComponent<IInteractable>().Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable") && arrayCounter < interactables.Length)
        {
            interactables[arrayCounter] = collision;
            arrayCounter++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable") && arrayCounter > 0)
        {
            arrayCounter--;
            interactables[arrayCounter] = null;
            Debug.Log("removed collider");
        }
    }
}
