using UnityEngine;
using System.Collections;

public class SteamWandController : MonoBehaviour
{
    [Header("Links")]
    public Transform wandClickable;      // куда кликаем
    public Transform pitcherSlot;        // куда вставляется питчер
    public GameObject outlineObject;

    [Header("Pitcher")]
    public PitcherController pitcher;

    [Header("FX")]
    public GameObject steamFX;

    [Header("Sound")]
    public AudioSource steamAudioSource;
    public AudioClip steamSound;
    [Range(0f, 1f)] public float steamSoundVolume = 1f;

    [Header("UI To Hide While Steaming")]
    public GameObject coffeeMachinePressEUI;

    [Header("Settings")]
    public float steamTime = 2f;

    private bool isBusy = false;
    private bool coffeeMachineEWasActive = false;

    private void Start()
    {
        outlineObject.SetActive(false);

        if (steamFX != null)
            steamFX.SetActive(false);

        if (steamAudioSource == null)
            steamAudioSource = GetComponent<AudioSource>();

        if (steamAudioSource != null)
        {
            steamAudioSource.playOnAwake = false;
            steamAudioSource.loop = true;
            steamAudioSource.volume = steamSoundVolume;

            if (steamSound != null)
                steamAudioSource.clip = steamSound;
        }
    }

    void Update()
    {
        if (isBusy) return;

        if (Input.GetMouseButtonDown(0) && pitcher.state == PitcherController.PitcherState.InHand)
        {
            outlineObject.SetActive(false);
            TryStartSteaming();
        }
    }

    void TryStartSteaming()
    {
        if (pitcher.milkLevel == 0)
        {
            Debug.Log("Нет молока");
            return;
        }

        Camera cam = CameraFocusController.Instance.GetActiveCamera();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
            return;

        if (!hit.transform.IsChildOf(wandClickable))
            return;

        StartCoroutine(SteamProcess(pitcher));
    }

    IEnumerator SteamProcess(PitcherController pitcher)
    {
        isBusy = true;

        Debug.Log("Начали взбивание");

        HideCoffeeMachinePressEUI();

        pitcher.InsertToSteamWand(pitcherSlot);

        if (steamFX != null)
            steamFX.SetActive(true);

        PlaySteamSound();

        yield return new WaitForSeconds(steamTime);

        if (steamFX != null)
            steamFX.SetActive(false);

        StopSteamSound();

        pitcher.SetSteamed(true);

        pitcher.ReturnToHand();

        Debug.Log("Молоко готово");

        isBusy = false;
    }

    private void PlaySteamSound()
    {
        if (steamAudioSource == null || steamSound == null)
            return;

        steamAudioSource.Stop();

        steamAudioSource.clip = steamSound;
        steamAudioSource.volume = steamSoundVolume;
        steamAudioSource.loop = true;

        steamAudioSource.Play();
    }

    private void StopSteamSound()
    {
        if (steamAudioSource == null)
            return;

        steamAudioSource.Stop();
    }

    private void HideCoffeeMachinePressEUI()
    {
        if (coffeeMachinePressEUI == null)
            return;

        coffeeMachineEWasActive = coffeeMachinePressEUI.activeSelf;
        coffeeMachinePressEUI.SetActive(false);
    }

    private void RestoreCoffeeMachinePressEUI()
    {
        if (coffeeMachinePressEUI == null)
            return;

        coffeeMachinePressEUI.SetActive(coffeeMachineEWasActive);
    }

    void OnMouseEnter()
    {
        if (isBusy) return;
        outlineObject.SetActive(true);
    }

    void OnMouseExit()
    {
        if (isBusy) return;
        outlineObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopSteamSound();

        if (steamFX != null)
            steamFX.SetActive(false);

        RestoreCoffeeMachinePressEUI();

        isBusy = false;
    }
}