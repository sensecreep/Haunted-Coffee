using System.Collections;
using UnityEngine;
/*
public class CameraFocusController : MonoBehaviour
{
    public static CameraFocusController Instance;

    private Vector3 savedPosition;
    private Quaternion savedRotation;

    private Coroutine currentRoutine;
    private bool isFocused;

    void Awake()
    {
        Instance = this;
    }

    public void FocusOn(Transform targetPoint, float duration = 0.4f)
    {
        if (isFocused)
            return;

        isFocused = true;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        savedPosition = transform.position;
        savedRotation = transform.rotation;

        currentRoutine = StartCoroutine(
            MoveCamera(targetPoint.position, targetPoint.rotation, duration)
        );
    }

    public void Return(float duration = 0.4f)
    {
        if (!isFocused)
            return;

        isFocused = false;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(
            MoveCamera(savedPosition, savedRotation, duration)
        );
    }

    IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        float t = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }
}
*/
public class CameraFocusController : MonoBehaviour
{
    public static CameraFocusController Instance;

    public Camera fpsCamera;
    public Camera interactionCamera;
    public bool IsFocused { get; private set; }

    void Awake()
    {
        Instance = this;
        interactionCamera.enabled = false;
    }

    public Camera GetActiveCamera()
    {
        if (interactionCamera != null && interactionCamera.enabled)
            return interactionCamera;

        return fpsCamera;
    }

    public void FocusOn(Transform anchor)
    {
        IsFocused = true;
        // ВАЖНО: сначала включаем interaction
        interactionCamera.transform.position = anchor.position;
        interactionCamera.transform.rotation = anchor.rotation;

        interactionCamera.enabled = true;
        fpsCamera.enabled = false;
    }

    public void Return()
    {
        IsFocused = false;
        // ВАЖНО: сначала включаем fps
        fpsCamera.enabled = true;
        interactionCamera.enabled = false;
    }
}