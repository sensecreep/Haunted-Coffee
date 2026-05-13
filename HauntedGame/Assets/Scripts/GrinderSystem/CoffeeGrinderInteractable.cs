using UnityEngine;
using static PortafilterController;

public class CoffeeGrinderInteractable : MonoBehaviour
{
    public CoffeeGrinderTrigger grinderTrigger;
    public Transform portafilterSlot;
    public PortafilterController portafilter;

    void Update()
    {
        // ❗ работаем ТОЛЬКО когда игрок в режиме кофемолки
        if (!grinderTrigger.IsUsing())
            return;

        if (Input.GetMouseButtonDown(0))
        {

            TryInsert();
        }
    }

    void TryInsert()
    {
        if (portafilter.state != PortafilterState.InHand)
            return;

        Camera cam = CameraFocusController.Instance.GetActiveCamera();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
            return;

        // проверяем что клик именно по этой кофемолке
        if (!hit.transform.IsChildOf(transform))
            return;

        portafilter.InsertIntoGrinder(portafilterSlot);
    }
}