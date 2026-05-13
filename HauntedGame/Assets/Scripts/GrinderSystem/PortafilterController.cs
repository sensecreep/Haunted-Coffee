using UnityEngine;

public class PortafilterController : MonoBehaviour
{
    public Transform handPoint;
    private Camera cam;
    private Renderer rend;
    public GameObject outlineObject;
    public Camera interactionCamera;
    public GameObject coffeeVisual; // объект с кофе внутри
    public PortafilterState state = PortafilterState.OnTable;
    public bool isLocked = false;
    Collider col;
    //public Transform defaultParent; // куда возвращается (например стол/стойка)
    public Transform defaultPoint;  // позиция

    public bool hasCoffee = false;

    void Awake()
    {
        col = GetComponent<Collider>();
    }
    void Start()
    {
        cam = interactionCamera;
        rend = GetComponent<Renderer>();
        outlineObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPickUp();
        }
    }
    public enum PortafilterState
    {
        OnTable,
        InHand,
        InGrinder
    }

    void TryPickUp()
    {
        if (state == PortafilterState.InHand || isLocked) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 2f)) return;

        if (hit.transform != transform) return;

        PickUp();
    }

    void PickUp()
    {
        state = PortafilterState.InHand;

        outlineObject.SetActive(false);
        transform.SetParent(handPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void InsertIntoGrinder(Transform slot)
    {
        state = PortafilterState.InGrinder;

        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // показываем кофе
        coffeeVisual.SetActive(true);
    }

    void OnMouseEnter()
    {
        if (state == PortafilterState.InHand || isLocked) return;
        outlineObject.SetActive(true);
    }

    void OnMouseExit()
    {
        if (state == PortafilterState.InHand || isLocked) return;
        outlineObject.SetActive(false);
    }

    void OnMouseDown()
    {
        if (isLocked) return;

        if (state == PortafilterState.InGrinder)
        {
            PickUp();
            // 👉 ВЫХОД ИЗ РЕЖИМА
            FindFirstObjectByType<CoffeeGrinderTrigger>().CloseGrinder();
        }
    }

    public void ResetToDefault()
    {
        state = PortafilterState.OnTable;

        transform.SetParent(defaultPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        hasCoffee = false;
        isLocked = false;
        coffeeVisual.SetActive(false);

        Debug.Log("Холдер сброшен");
    }
}
