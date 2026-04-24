using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [Header("Player")]
    public CharacterController playerController;
    public Camera playerCamera;

    [Header("Inventory")]
    public PickItem[] availableItems;
    public int inventorySize = 12;

    [Header("Pickup")]
    public float pickupDistance = 3f;

    private int[] itemSlots;
    private bool showInventory;

    private int hoveringOverIndex = -1;
    private int draggingIndex = -1;

    private int equippedSlot = -1;
    private GameObject equippedObject;

    private int pendingDropIndex = -1;

    void Start()
    {
        itemSlots = new int[inventorySize];
        for (int i = 0; i < itemSlots.Length; i++)
            itemSlots[i] = -1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleInventoryToggle();
        DetectItemPickup();
        HandlePendingDrop();
    }

    // ===================== INVENTORY TOGGLE =====================
    void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showInventory = !showInventory;

            Cursor.visible = showInventory;
            Cursor.lockState = showInventory ? CursorLockMode.None : CursorLockMode.Locked;

            if (playerController != null)
                playerController.enabled = !showInventory;
        }
    }

    // ===================== PICKUP =====================
    void DetectItemPickup()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            PickItem item = hit.collider.GetComponent<PickItem>();

            if (item && Input.GetKeyDown(KeyCode.E))
            {
                int index = System.Array.IndexOf(availableItems, item);
                if (index != -1)
                {
                    AddItem(index);
                    Destroy(item.gameObject);
                }
            }
        }
    }
    void AddItem(int itemIndex)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == -1)
            {
                itemSlots[i] = itemIndex;
                return;
            }
        }
    }

    // ===================== GUI =====================
    void OnGUI()
    {
        if (!showInventory) return;

        GUI.Box(new Rect(200, 100, 400, 320), "Inventory");
        hoveringOverIndex = -1;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            int x = i % 4;
            int y = i / 4;

            Rect slotRect = new Rect(220 + x * 90, 140 + y * 90, 80, 80);
            GUI.Box(slotRect, "");

            if (slotRect.Contains(Event.current.mousePosition))
                hoveringOverIndex = i;

            if (itemSlots[i] != -1)
            {
                GUI.DrawTexture(slotRect, availableItems[itemSlots[i]].icon);
            }
        }

        HandleMouseGUI();
    }

    // ===================== GUI INPUT =====================
    void HandleMouseGUI()
    {
        Event e = Event.current;

        // Drag
        if (e.type == EventType.MouseDown && e.button == 0 && hoveringOverIndex != -1)
        {
            draggingIndex = hoveringOverIndex;
        }

        if (e.type == EventType.MouseUp && draggingIndex != -1 && hoveringOverIndex != -1)
        {
            int temp = itemSlots[draggingIndex];
            itemSlots[draggingIndex] = itemSlots[hoveringOverIndex];
            itemSlots[hoveringOverIndex] = temp;
            draggingIndex = -1;
        }

        // Equip (ÏÊÌ)
        if (e.type == EventType.MouseDown && e.button == 1 && hoveringOverIndex != -1)
        {
            EquipItem(hoveringOverIndex);
        }

        // Mark drop
        if (hoveringOverIndex != -1 &&
            e.type == EventType.KeyDown &&
            e.keyCode == KeyCode.Z)
        {
            pendingDropIndex = hoveringOverIndex;
        }
    }

    // ===================== DROP HANDLER =====================
    void HandlePendingDrop()
    {
        if (pendingDropIndex != -1)
        {
            DropItem(pendingDropIndex);
            pendingDropIndex = -1;
        }
    }
    // ===================== EQUIP =====================
    void EquipItem(int slotIndex)
    {
        int itemIndex = itemSlots[slotIndex];
        if (itemIndex == -1) return;

        PickItem item = availableItems[itemIndex];
        if (!item.isEquipable || item.equippedPrefab == null) return;

        if (equippedObject)
            Destroy(equippedObject);

        equippedObject = Instantiate(item.equippedPrefab, playerCamera.transform);
        equippedObject.transform.localPosition = new Vector3(0.5f, -0.25f, 1f);
        equippedObject.transform.localRotation = Quaternion.identity;

        equippedSlot = slotIndex;

        itemSlots[slotIndex] = -1;
    }

    // ===================== DROP =====================
    void DropItem(int slotIndex)
    {
        int itemIndex = itemSlots[slotIndex];

        if (equippedSlot == slotIndex && equippedObject != null)
        {
            Destroy(equippedObject);
            equippedSlot = -1;
        }

        if (itemIndex == -1) return;

        PickItem item = availableItems[itemIndex];

        if (item != null && item.prefab != null)
        {
            Vector3 dropPosition = playerCamera.transform.position +
                                   playerCamera.transform.forward * 2f;
            Instantiate(item.prefab, dropPosition, Quaternion.identity);
        }

        itemSlots[slotIndex] = -1;
    }

}


/*using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public Texture crosshairTexture;
    public CharacterController playerController;
    public PickItem[] availableItems;

    int[] itemSlots = new int[12];
    bool showInventory = false;
    float windowAnimation = 1;
    float animationTimer = 0;

    PickItem detectedItem;
    int detectedItemIndex;

    int hoveringOverIndex = -1;
    int itemIndexToDrag = -1;
    Vector2 dragOffset = Vector2.zero;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i] = -1;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showInventory = !showInventory;
            animationTimer = 0;

            if (showInventory)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        if (animationTimer < 1)
        {
            animationTimer += Time.deltaTime;
        }
        if (showInventory)
        {
            windowAnimation = Mathf.Lerp(windowAnimation, 0, animationTimer);
            playerController.canMove = false;
        }
        else
        {
            windowAnimation = Mathf.Lerp(windowAnimation, 1f, animationTimer);
            playerController.canMove = true;
        }
        if (Input.GetMouseButtonDown(0) && hoveringOverIndex > -1 && itemSlots[hoveringOverIndex] > -1)
        {
            itemIndexToDrag = hoveringOverIndex;
        }
    }
    void DetectItem()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            PickItem item = hit.collider.GetComponent<PickItem>();
            if (item)
            {
                detectedItem = item;
                detectedItemIndex = System.Array.IndexOf(availableItems, item);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    AddItemToInventory(detectedItemIndex);
                    Destroy(item.gameObject);
                }
            }
            else
            {
                detectedItem = null;
            }
        }
    }
    void AddItemToInventory(int index)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == -1)
            {
                itemSlots[i] = index;
                return;
            }
        }
    }
    void OnGUI()
    {
        if (!showInventory) return;

        GUI.Box(new Rect(200, 100, 400, 300), "Inventory");

        for (int i = 0; i < itemSlots.Length; i++)
        {
            int x = i % 4;
            int y = i / 4;

            Rect slotRect = new Rect(220 + x * 90, 140 + y * 90, 80, 80);
            GUI.Box(slotRect, "");

            if (itemSlots[i] != -1)
            {
                GUI.DrawTexture(slotRect, availableItems[itemSlots[i]].icon);
            }
        }
    }


}*/
