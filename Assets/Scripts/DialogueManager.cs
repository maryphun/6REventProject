using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject SelectionPrefab;
    [SerializeField] Canvas SelectionCanvas;
    [SerializeField] int textSize = 36;
    [SerializeField] float textInterval = 0.075f;
    [SerializeField] Image screenAlpha;
    [SerializeField] TMP_Text narrativeText;

    Selection currentSelection;

    // narrative
    bool narrativeMode;
    private bool isTypeWrting;
    private bool isSkipTypeWriter;
    private bool isPlayTypeWriterSE;
    private int typewriteCnt;

    private void Awake()
    {
        narrativeMode = false;
        screenAlpha.color = new Color(0, 0, 0, 0);
        narrativeText.text = string.Empty;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            RegisterNewNarrative("早安，我的名字叫做劉連，是這棟大樓的送報員，每天早上都會去分送報紙給各戶人家。");
        }
    }

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

    public void RegisterNewNarrative(string text, bool playSE = true)
    {
        if (!narrativeMode)
        {
            narrativeMode = true;
            screenAlpha.DOFade(0.7f, 1.0f);

            // reset variables
            isSkipTypeWriter = false;
            isPlayTypeWriterSE = playSE;
            StartCoroutine(SetNarrativeTextLoop(text, textInterval));
        }
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

    private IEnumerator SetNarrativeTextLoop(string newText, float interval)
    {
        int wordCount = newText.Length - 1;
        int currentCount = 0;
        string patternDetect = string.Empty;
        string text = string.Empty;
        bool[] highlight = new bool[3];
        for (int i = 0; i < 3; i++) highlight[i] = false;

        isTypeWrting = true;
        typewriteCnt = 0;

        while (currentCount <= wordCount)
        {
            // initiate wait time
            float waitTime = isSkipTypeWriter ? 0.0f : interval;

            //don't add text yet if the window is not fully open
            if (!narrativeMode || screenAlpha.color.a < 0.7f)
            {
                yield return new WaitForSeconds(waitTime);
                continue;
            }

            text = text + newText[currentCount];

            // update text
            narrativeText.text = text;

            // check pattern
            patternDetect = patternDetect + newText[currentCount];

            if (CheckPatterns(patternDetect, waitTime, out float newWaitTime))
            {
                patternDetect = string.Empty;
                waitTime = newWaitTime;
            }

            // update count at the last
            currentCount++;

            // play SE
            if (isPlayTypeWriterSE)
            {
                if (typewriteCnt % 2 == 0)
                {
                    AudioManager.Instance.PlaySFX("Letter_1", 0.1f);
                }
                typewriteCnt++;
            }
            yield return new WaitForSeconds(waitTime);
        }

        isTypeWrting = false;
        isSkipTypeWriter = false;
    }


    private bool CheckPatterns(string pattern, float originalWaitTime, out float newWaitTime)
    {
        newWaitTime = originalWaitTime;

        if (CheckComma(pattern))
        {
            newWaitTime = originalWaitTime * 3.5f;
            return true;
        }

        if (CheckSpace(pattern))
        {
            newWaitTime = 0.0f;
            return true;
        }

        if (CheckPeriod(pattern))
        {
            newWaitTime = originalWaitTime * 7.5f;
            return true;
        }

        return false;
    }

    private bool CheckComma(string pattern)
    {
        return (pattern.Contains(",") || pattern.Contains("、") || pattern.Contains("，"));
    }

    private bool CheckSpace(string pattern)
    {
        return (pattern.Contains(" "));
    }

    private bool CheckPeriod(string pattern)
    {
        return (pattern.Contains(".") || pattern.Contains("。") || pattern.Contains("?") || pattern.Contains("!"));
    }

}
