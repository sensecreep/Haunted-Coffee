using UnityEngine;
using static PortafilterController;

public class CoffeeGrinderTrigger : MonoBehaviour
{
    public GameObject pressEUI;
    public CoffeeGrinderUI grinderUI;
    public Transform cameraPoint;
    public Transform playerSpot;
    public Transform portafilterSlot;
    public PortafilterController portafilter;
    //public Transform grinderClickable; // сама модель кофемолки

    private bool playerInRange;
    private bool isUsing;

    void Start()
    {
        if (pressEUI != null)
            pressEUI.SetActive(false);

        grinderUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenGrinder();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseGrinder();
        }
    }

    void OpenGrinder()
    {
        isUsing = true;

        if (pressEUI != null)
            pressEUI.SetActive(false);

        MovePlayerToSpot();

        PlayerLock.Instance.Lock();

        CameraFocusController.Instance.FocusOn(cameraPoint);

        grinderUI.gameObject.SetActive(true);
        grinderUI.Open(this);
    }

    void MovePlayerToSpot()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.transform.position = playerSpot.position;
        player.transform.rotation = playerSpot.rotation;
    }
    public bool TryGrindSelectedBeans(CoffeeBeans beans)
    {
        if (beans == null)
        {
            Debug.LogWarning("Не выбраны зерна");
            return false;
        }

        if (portafilter == null)
        {
            Debug.LogError("Холдер не назначен в CoffeeGrinderTrigger");
            return false;
        }

        if (portafilter.state != PortafilterController.PortafilterState.InGrinder)
        {
            Debug.Log("Сначала вставьте холдер в кофемолку");
            return false;
        }

        portafilter.FillWithGroundCoffee(beans);

        Debug.Log("Помол выполнен. Зерна в холдере: " + beans.beanName);
        return true;
    }
    public void CloseGrinder()
    {
        isUsing = false;

        grinderUI.gameObject.SetActive(false);

        CameraFocusController.Instance.Return();

        PlayerLock.Instance.Unlock();
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (pressEUI != null)
            pressEUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (pressEUI != null)
            pressEUI.SetActive(false);
    }
    public bool IsUsing()
    {
        return isUsing;
    }
}
