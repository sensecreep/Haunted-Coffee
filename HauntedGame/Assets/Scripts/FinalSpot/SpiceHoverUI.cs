using UnityEngine;
using TMPro;

public class SpiceHoverUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject panel;

    public float distance = 6f;

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        Camera cam = CameraFocusController.Instance.GetActiveCamera();

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
        panel.SetActive(true);
        text.text = name;
    }

    void Hide()
    {
        panel.SetActive(false);
    }
}