using UnityEngine;

public class VisitorSkinController : MonoBehaviour
{
    [Header("Skin Settings")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private GameObject[] skinPrefabs;

    [Header("Random")]
    [SerializeField] private bool randomizeOnStart = true;

    private GameObject currentSkin;

    private void Start()
    {
        if (randomizeOnStart)
        {
            ApplyRandomSkin();
        }
    }

    public void ApplyRandomSkin()
    {
        if (skinPrefabs == null || skinPrefabs.Length == 0)
        {
            Debug.LogWarning("Скины посетителей не назначены");
            return;
        }

        int randomIndex = Random.Range(0, skinPrefabs.Length);
        ApplySkin(randomIndex);
    }

    public void ApplySkin(int skinIndex)
    {
        if (visualRoot == null)
        {
            Debug.LogError("VisualRoot не назначен у VisitorSkinController");
            return;
        }

        if (skinPrefabs == null || skinPrefabs.Length == 0)
        {
            Debug.LogWarning("Нет доступных скинов посетителя");
            return;
        }

        if (skinIndex < 0 || skinIndex >= skinPrefabs.Length)
        {
            Debug.LogError("Неверный индекс скина: " + skinIndex);
            return;
        }

        ClearCurrentSkin();

        GameObject selectedSkin = skinPrefabs[skinIndex];

        currentSkin = Instantiate(selectedSkin, visualRoot);
        currentSkin.transform.localPosition = Vector3.zero;
        currentSkin.transform.localRotation = Quaternion.identity;
        currentSkin.transform.localScale = Vector3.one;

        DisableUnwantedComponents(currentSkin);

        Debug.Log("Посетителю назначен скин: " + selectedSkin.name);
    }

    private void ClearCurrentSkin()
    {
        if (currentSkin != null)
        {
            Destroy(currentSkin);
            currentSkin = null;
        }
    }

    private void DisableUnwantedComponents(GameObject skinObject)
    {
        Collider[] colliders = skinObject.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        Rigidbody[] rigidbodies = skinObject.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}