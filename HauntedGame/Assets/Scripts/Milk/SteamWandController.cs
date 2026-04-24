using UnityEngine;
using System.Collections;

public class SteamWandController : MonoBehaviour
{
    [Header("Links")]
    public Transform wandClickable;      // куда кликаем
    public Transform pitcherSlot;        // куда вставляется питчер

    [Header("Pitcher")]
    public PitcherController pitcher;

    [Header("FX")]
    public GameObject steamFX;

    [Header("Settings")]
    public float steamTime = 2f;

    private bool isBusy = false;

    void Update()
    {
        if (isBusy) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryStartSteaming();
        }
    }

    void TryStartSteaming()
    {
        if (pitcher == null)
        {
            Debug.Log("Питчер не назначен");
            return;
        }

        if (pitcher.state != PitcherController.PitcherState.InHand)
        {
            Debug.Log("Питчер не в руках");
            return;
        }

        // ❌ нет молока
        if (pitcher.milkLevel == 0)
        {
            Debug.Log("Нет молока");
            return;
        }

        // 🎯 проверка клика по steam wand
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

        // 👉 вставляем питчер
        pitcher.InsertToSteamWand(pitcherSlot);

        // 👉 включаем пар
        if (steamFX != null)
            steamFX.SetActive(true);

        yield return new WaitForSeconds(steamTime);

        // 👉 выключаем пар
        if (steamFX != null)
            steamFX.SetActive(false);

        // 👉 помечаем как взбитый
        pitcher.SetSteamed(true);

        // 👉 возвращаем в руки
        pitcher.ReturnToHand();

        Debug.Log("Молоко готово");

        isBusy = false;
    }
}