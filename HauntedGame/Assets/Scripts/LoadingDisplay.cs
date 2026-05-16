using UnityEngine;
using System.Collections;

public class SequentialImageDisplay : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject[] images; // Массив из 3 изображений
    public float stepDelay = 0.85f; // Задержка между шагами

    private int currentStep = 0;

    void Start()
    {
        // Проверка наличия изображений
        if (images == null || images.Length != 3)
        {
            Debug.LogError("Назначьте ровно 3 изображения!");
            return;
        }

        // Выключаем все изображения при старте
        SetAllImages(false);

        // Запускаем цикл
        StartCoroutine(ImageSequence());
    }

    IEnumerator ImageSequence()
    {
        while (true)
        {
            switch (currentStep)
            {
                case 0: // Первая секунда - 1 изображение
                    SetImageState(0, true);
                    SetImageState(1, false);
                    SetImageState(2, false);
                    break;

                case 1: // Вторая секунда - 2 изображения
                    SetImageState(0, true);
                    SetImageState(1, true);
                    SetImageState(2, false);
                    break;

                case 2: // Третья секунда - 3 изображения
                    SetImageState(0, true);
                    SetImageState(1, true);
                    SetImageState(2, true);
                    break;

                case 3: // Четвертая секунда - все выключены
                    SetAllImages(false);
                    break;
            }

            // Ждем указанное время
            yield return new WaitForSeconds(stepDelay);

            // Переходим к следующему шагу
            currentStep = (currentStep + 1) % 4;
        }
    }

    void SetAllImages(bool state)
    {
        foreach (var img in images)
        {
            if (img != null) img.SetActive(state);
        }
    }

    void SetImageState(int index, bool state)
    {
        if (index >= 0 && index < images.Length && images[index] != null)
        {
            images[index].SetActive(state);
        }
    }
}
