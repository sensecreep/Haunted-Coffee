using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class DialogeBox : MonoBehaviour
{
    public static event Action OnDialogueEnded;
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;

    /*void Start()
    {
        textComponent.text = string.Empty;
    }*/

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }
    /*
    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }
    */
    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
            OnDialogueEnded?.Invoke(); // 🔥 СИГНАЛ О КОНЦЕ ДИАЛОГА
        }
    }
    /*
    public void StartDialogueExtern()
    {
        textComponent.text = string.Empty;
        index = 0;
        StartCoroutine(TypeLine());
    }
    */
    public void StartDialogue(string[] newLines)
    {
        if (newLines == null || newLines.Length == 0)
        {
            Debug.LogError("DialogeBox: lines is null or empty");
            return;
        }

        if (textComponent == null)
        {
            Debug.LogError("DialogeBox: textComponent is NULL");
            return;
        }

        lines = newLines;
        textComponent.text = string.Empty;
        index = 0;
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }


}
