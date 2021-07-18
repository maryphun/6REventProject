using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject SelectionPrefab;
    [SerializeField] Canvas SelectionCanvas;
    [SerializeField] int textSize = 36;
    [SerializeField] float textInterval = 0.075f;

    Selection currentSelection;

    public void RegisterNewChoice(List<string> selections)
    {
        var obj = Instantiate(SelectionPrefab, SelectionCanvas.transform);
        currentSelection = obj.GetComponent<Selection>();
        currentSelection.Initialization(selections, 0.5f);
    }

    public void RegisterNewDialogue(string text, Vector2 worldPosition)
    {
        // calculate window size base on text count
        float spacePerText = (textSize * 1.38888888889f);
        float textCount = (text.Length + 1);
        Vector2 windowSize = new Vector2(spacePerText * textCount, 100f);
        windowSize = AutoResizeWindow(windowSize);
        worldPosition.y += windowSize.y / 2f;

        WindowManager.Instance.CreateWindow("dialogue", worldPosition, windowSize);
        WindowManager.Instance.Open("dialogue", 0.5f);
        WindowManager.Instance.SetText("dialogue", text, textInterval);
        WindowManager.Instance.SetTextAlignment("dialogue", CustomTextAlignment.topLeft);
        WindowManager.Instance.SetTextColor("dialogue", Color.yellow);
        WindowManager.Instance.SetTextSize("dialogue", textSize);
        WindowManager.Instance.SetTextMargin("dialogue", new Vector4(14, 0, 0, 0));

        WindowManager.Instance.AddNewImage("dialogue", "arrow", new Vector2(windowSize.x/2f, -8f), new Vector2(50f, 50f), true);
    }

    private int GetResult()
    {
        if (currentSelection.IsSelected())
        {
            return -1;
        }

        return currentSelection.GetResult();
    }

    private Vector2 AutoResizeWindow(Vector2 original)
    {
        Vector2 rtn = original;

        while (rtn.x > 1000.0f)
        {
            rtn.x /= 2f;
            rtn.y += 85.0f;
        }

        return rtn;
    }
}
