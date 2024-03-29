using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Waiter : MonoBehaviour
{
    private GameManager gm;

    [SerializeField] private InventorySlot inventorySlot;
    public InventorySlot InventorySlot { get => inventorySlot; }

    [SerializeField] private Vector2 obstacle;

    [SerializeField] private bool isMoving;
    public bool IsMoving { get { return isMoving; } }

    [SerializeField] private bool isOnCenter;
    public bool IsOnCenter => isOnCenter;

    [SerializeField] private bool isOnTable;
    public bool IsOnTable => isOnTable;

    [SerializeField] private string previousTable;
    public string PreviousTable => previousTable;

    [SerializeField] private string currentTable;
    public string CurrentTable 
    { 
        get => currentTable; 
        set => currentTable = value;
    }

    [SerializeField] private bool hasMeal;
    public bool HasMeal { get => hasMeal; }

    [SerializeField] private bool isInRange;

    public int MealIndex { get; set; }

    private NavMeshAgent agent;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();

        agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;

        currentTable = "Center";
        isOnCenter = true;
        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string tableName;

        if (other.name == "Counter")
            tableName = other.name;
        else
            tableName = other.transform.parent.name;

        Debug.Log(tableName);

        if (tableName == currentTable)
        {
            Debug.Log("On a table");

            CheckCurrentTable();
            isOnTable = true;
        }
    }

    public void Center() => isOnCenter = true;

    public void UpdateTables(string previous, string current)
    {
        previousTable = previous;
        currentTable = current;
    }

    public void CheckCurrentTable()
    {
        foreach (GameObject customer in gm.Customers)
        {
            Customer cust = customer.GetComponent<Customer>();

            if (currentTable == cust.Table)
            {
                if (!cust.IsAttended && cust.Decided)
                {
                    // Attend customer
                    Debug.Log(
                        $"Waiter is attending {customer.name} at {currentTable}");
                    StartCoroutine(cust.MakeRequest());
                }
                else if (cust.IsAttended && hasMeal && !cust.IsLeaving)
                {
                    // Serve customer
                    Debug.Log(
                        $"Waiter served {customer.name} at {currentTable}");
                    cust.GetServed();
                    RemoveFromInventory();
                }
            }
        }
    }

    public IEnumerator Move(Vector2 target, 
        bool moreTargets = false, Vector2? secTarg = null)
    {
        Debug.Log("Waiter moving...");

        Vector2 newTarget;

        isMoving = true;
        isOnTable = false;

        do
        {
            agent.SetDestination(target);
            yield return null;
        }
        while (Vector2.Distance(transform.position, target) > 0.1f);

        if (!moreTargets)
            isMoving = false;
        else
        {
            if (secTarg.HasValue)
            {
                newTarget = secTarg.Value;
                StartCoroutine(Move(newTarget));
            }
        }
    }

    public void AddToInventory(int i)
    {
        Debug.Log($"Added meal #{i} to inventory.");
        MealIndex = i;
        hasMeal = true;
        inventorySlot.UpdateInventory(i);
    }

    public void RemoveFromInventory()
    {
        Debug.Log("Removed meal from inventory.");
        hasMeal = false;
        inventorySlot.UpdateInventory(MealIndex);
    }
}
