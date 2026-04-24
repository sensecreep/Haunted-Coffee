using UnityEngine;
using System.Collections;

public class CameraDialogueController : MonoBehaviour
{
    public float rotateSpeed = 6f;

    private Quaternion originalCameraRotation;
    private Quaternion originalPlayerRotation;

    private FirstPersonCamera fpsCamera;
    private Transform playerBody;

    private float originalXRotation;
    private bool inDialogue;

    void Awake()
    {
        fpsCamera = GetComponent<FirstPersonCamera>();
        playerBody = fpsCamera.playerBody;
    }

    public void StartDialogueLook(Transform npcLookPoint)
    {
        if (inDialogue) return;

        inDialogue = true;

        originalCameraRotation = transform.rotation;
        originalPlayerRotation = playerBody.rotation;
        originalXRotation = fpsCamera.CurrentXRotation;

        fpsCamera.enabled = false;

        StopAllCoroutines();
        StartCoroutine(LookAtNPC(npcLookPoint));
    }

    public void EndDialogueLook()
    {
        if (!inDialogue) return;

        StopAllCoroutines();
        StartCoroutine(ReturnLook());
    }

    IEnumerator LookAtNPC(Transform target)
    {
        while (Quaternion.Angle(transform.rotation,
               Quaternion.LookRotation(target.position - transform.position)) > 0.2f)
        {
            Quaternion targetRot =
                Quaternion.LookRotation(target.position - transform.position);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * rotateSpeed);

            yield return null;
        }
    }

    IEnumerator ReturnLook()
    {
        while (Quaternion.Angle(transform.rotation, originalCameraRotation) > 0.2f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                originalCameraRotation,
                Time.deltaTime * rotateSpeed);

            playerBody.rotation = Quaternion.Slerp(
                playerBody.rotation,
                originalPlayerRotation,
                Time.deltaTime * rotateSpeed);

            yield return null;
        }

        fpsCamera.SetXRotation(originalXRotation);
        fpsCamera.enabled = true;
        inDialogue = false;
    }
}
