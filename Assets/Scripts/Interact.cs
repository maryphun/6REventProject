using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class Interact : MonoBehaviour
{
    public PlayerController player;
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private float offsetY = 0.8f;
    [SerializeField] private float dialogueOffsetY = 150f;
    [SerializeField] private float floatSpeed = 2.0f;
    [SerializeField] private float floatOffset = 0.1f;

    SpriteRenderer UI;
    Vector2 originalPosition;
    bool isInRange = false;
    float floatingCount;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        UI = GetComponent<SpriteRenderer>();

        originalPosition = GetComponentInParent<Transform>().position; 
        originalPosition = originalPosition + GetComponentInParent<Collider2D>().offset;

        isInRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        float range = Vector2.Distance(player.transform.position, originalPosition);
        if (range < interactRange)
        {
            if (!isInRange)
            {
                isInRange = true;
                transform.localPosition = new Vector2(0.0f, offsetY);
                floatingCount = 0.0f;
                UI.DOFade(1.0f, 0.5f);
            }
        }
        else
        {
            if (isInRange)
            {
                isInRange = false;
                UI.DOFade(0.0f, 0.5f);
            }
        }

        if (!isInRange) return;

        // move
        floatingCount += Time.deltaTime * floatSpeed;
        transform.localPosition = new Vector2(0.0f, offsetY + (Mathf.Sin(floatingCount) * floatOffset));

        // trigger
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Vector2 windowPos = new Vector2(Camera.main.WorldToScreenPoint(originalPosition).x - Screen.width/2f, 
                                            Camera.main.WorldToScreenPoint(originalPosition).y + dialogueOffsetY - Screen.height / 2f);
            WindowManager.Instance.CreateWindow("dialogue", windowPos, new Vector2(7f * 50f, 100f));
            WindowManager.Instance.Open("dialogue", 0.5f);
            WindowManager.Instance.SetText("dialogue", "你好我是箱子。", 0.075f);
            WindowManager.Instance.SetTextAlignment("dialogue", CustomTextAlignment.center);
            WindowManager.Instance.SetTextColor("dialogue", Color.yellow);
        }
    }
}
