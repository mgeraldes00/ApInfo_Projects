using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    private Waiter waiter;

    [SerializeField] private Transform position;

    private void Start()
    {
        waiter = FindObjectOfType<Waiter>();
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked trash");

        if (!waiter.GetComponent<Waiter>().IsMoving)
        {
            waiter.CurrentTable = "";
            StartCoroutine(waiter.GetComponent<Waiter>().Move(
                position.transform.position));
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && waiter.HasMeal)
        {
            waiter.RemoveFromInventory();
        }
    }
}