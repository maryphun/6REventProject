using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] SpriteRenderer[] night;
    [SerializeField] SpriteRenderer[] morning;
    [SerializeField] SpriteRenderer lightbulb;
    [SerializeField] SpriteRenderer dayswwitchAlpha;

    [Header("Parameter")]
    [SerializeField] float lightBulbTime = 2f;

    private bool lightBulbOn = false;
    private bool isNight = false;
    // Start is called before the first frame update
    void Start()
    {
        Night();
    }

    public void SwitchLightBulb(bool boolean)
    {
        lightBulbOn = boolean;
        if (boolean)
        {
            StartCoroutine(TurnOnLightBulbPattern(lightBulbTime/2f));
        }
        else
        {
            lightbulb.DOFade(0.0f, lightBulbTime);
            night[1].DOFade(1.0f, lightBulbTime);
            night[2].DOFade(0.0f, lightBulbTime);
        }
    }

    private IEnumerator TurnOnLightBulbPattern(float time)
    {
        lightbulb.DOFade(Random.Range(0.2f, 0.5f), 0.0f);

        float alp = 0.0f;

        for (int i = 0; i < 8; i++)
        {
            if (!lightBulbOn) yield break;
            yield return new WaitForSeconds(time / 30.0f);
            alp = alp == 0.0f ? Random.Range(0.2f, 0.5f) : 0.0f;
            lightbulb.DOFade(alp, 0.0f);
        }

        if (!lightBulbOn) yield break;
        yield return new WaitForSeconds(time / 5.0f);

        if (!lightBulbOn) yield break;
        lightbulb.DOFade(1.0f, time / 5.0f);

        if (isNight)
        {
            night[1].DOFade(0.0f, time / 5.0f);
            night[2].DOFade(1.0f, time / 5.0f);
        }
    }

    public void Night()
    {
        isNight = true;
        foreach (SpriteRenderer sprite in morning)
        {
            sprite.DOFade(0.0f, 0.0f);
        }

        if (lightBulbOn)
        {
            night[1].DOFade(0.0f, 0.0f);
            night[2].DOFade(1.0f, 0.0f);
        }
        else
        {
            night[1].DOFade(1.0f, 0.0f);
            night[2].DOFade(0.0f, 0.0f);
        }
    }
}
