using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private Dictionary<string, string[]> dialogues =
        new Dictionary<string, string[]>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        LoadDialogues();
    }

    void LoadDialogues()
    {
        TextAsset xmlFile = Resources.Load<TextAsset>("dialogues");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlFile.text);

        XmlNodeList dialogueNodes = xmlDoc.GetElementsByTagName("dialogue");

        foreach (XmlNode node in dialogueNodes)
        {
            string id = node.Attributes["id"].Value;
            List<string> lines = new List<string>();

            foreach (XmlNode line in node.ChildNodes)
            {
                lines.Add(line.InnerText);
            }

            dialogues[id] = lines.ToArray();
        }
    }

    public string[] GetDialogue(string id)
    {
        if (dialogues.ContainsKey(id))
            return dialogues[id];

        Debug.LogError("Dialogue not found: " + id);
        return null;
    }
}
