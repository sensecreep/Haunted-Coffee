using UnityEngine;
using TMPro;

public class SpiceHoverUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject panel;

    public float distance = 6f;

    [Header("Assembly Station")]
    [SerializeField] private DrinkAssemblyTrigger assemblyTrigger;

    void Start()
    {
        if (assemblyTrigger == null)
            assemblyTrigger = FindObjectOfType<DrinkAssemblyTrigger>();

        Hide();
    }

    void Update()
    {
        if (assemblyTrigger == null || !assemblyTrigger.IsUsing)
        {
            Hide();
            return;
        }

        Camera cam = CameraFocusController.Instance.GetActiveCamera();

        if (cam == null)
        {
            Hide();
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            SpiceItem spice = hit.transform.GetComponentInParent<SpiceItem>();

            if (spice != null)
            {
                Show(spice.displayName);
                return;
            }
        }

        Hide();
    }

    void Show(string name)
    {
        if (panel != null)
            panel.SetActive(true);

        if (text != null)
            text.text = name;
    }

    void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}