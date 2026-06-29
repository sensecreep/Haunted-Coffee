using TMPro;
using UnityEngine;

public class BookHoverHint : MonoBehaviour
{
    [Header("Hint")]
    [SerializeField] private TextMeshPro hintText;
    [SerializeField] private string hintMessage = "Нажмите Tab для просмотра рецептов";

    [Header("Raycast")]
    [SerializeField] private float hoverDistance = 5f;

    private Camera activeCamera;

    private void Start()
    {
        if (hintText != null)
        {
            hintText.text = hintMessage;
            hintText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        activeCamera = Camera.main;

        if (activeCamera == null || hintText == null)
            return;

        Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);

        bool isHoveringBook = false;

        if (Physics.Raycast(ray, out RaycastHit hit, hoverDistance))
        {
            isHoveringBook = hit.transform == transform ||
                             hit.transform.IsChildOf(transform) ||
                             transform.IsChildOf(hit.transform);
        }

        hintText.gameObject.SetActive(isHoveringBook);

        if (isHoveringBook)
        {
            hintText.transform.LookAt(activeCamera.transform);
            hintText.transform.Rotate(0f, 180f, 0f);
        }
    }
}