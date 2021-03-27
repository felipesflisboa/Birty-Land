using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlueNebulaIntroController : MonoBehaviour {
    [SerializeField] AudioSource sfx;
    [SerializeField] Image fadeMaskImage;
    [SerializeField] string[] skipButtonNameArray = new string[0];
    [SerializeField] KeyCode[] skipKeyArray = new KeyCode[0];
    bool canSkipNow;
    bool skipping;
    [SerializeField] string nextSceneName;
    [Tooltip("To go into a different scene when skip. If empty, go into nextSceneName"), SerializeField] string skippableSceneName;
	

    IEnumerator Start() {
        FadeScreen(1f);
		yield return null;
        yield return FadeScreenRoutine(0.75f, true);
		sfx.Play();
		canSkipNow = skipButtonNameArray.Length > 0 || skipKeyArray.Length > 0;
        yield return new WaitForSeconds(2f);
        if (!skipping) {
            canSkipNow = false;
            yield return FadeScreenRoutine(0.75f, false);
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator SkipRoutine() {
        skipping = true;
        canSkipNow = false;
        yield return FadeScreenRoutine(0.25f, false);
        if (string.IsNullOrEmpty(skippableSceneName))
            skippableSceneName = nextSceneName;
        if (!string.IsNullOrEmpty(skippableSceneName))
            SceneManager.LoadScene(skippableSceneName);
    }

    void Update() {
        if (canSkipNow && HasSkipRequested())
            StartCoroutine(SkipRoutine());
    }

    bool HasSkipRequested() {
        foreach (var buttonName in skipButtonNameArray)
            if(Input.GetButtonDown(buttonName))
                return true;
        foreach (var key in skipKeyArray)
            if (Input.GetKeyDown(key))
                return true;
        return false;
    }

    IEnumerator FadeScreenRoutine(float duration, bool isFadeIn) {
        return Tween(isFadeIn ? 1 : 0, isFadeIn ? 0 : 1, duration, FadeScreen);
    }

    /// <summary>
    /// Simple tween
    /// </summary>
    /// <param name="start">Initial float</param>
    /// <param name="end">Final float</param>
    /// <param name="duration">Duration</param>
    /// <param name="action">Action to be called with float parameter</param>
    /// <param name="easeAction">Ease algorithm</param>
    /// <returns></returns>
    IEnumerator Tween(float start, float end, float duration, Action<float> action) {
        float ratioGain = 1f/duration;
        float ratioCounter = 0f;
        do {
            yield return null;
            ratioCounter = Mathf.Clamp01(ratioGain*Time.deltaTime + ratioCounter);
            action(start + ratioCounter* (end-start));
        } while (ratioCounter < 1f);
    }

    void FadeScreen(float alpha) {
        fadeMaskImage.color = new Color(fadeMaskImage.color.r, fadeMaskImage.color.g, fadeMaskImage.color.b, alpha);
    }
}
