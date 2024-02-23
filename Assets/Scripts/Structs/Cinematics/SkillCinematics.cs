using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Utility;

public class SkillCinematics : MonoBehaviour {
    [SerializeField] private PixelPerfectCamera pixelCam;
    [SerializeField] private Volume volume;

    private Vignette vignette;
    private MotionBlur motionBlur;
    private ChromaticAberration chromaticAberration;

    private record Status(int ppu, Color vignetteColor);

    private Status origin;
    private Status effect;

    [Header("Destination")]
    [SerializeField] private int resolution;
    [SerializeField] private Color vignetteColor;

    private Coroutine cinematicCoroutine;
    [SerializeField] private Animator uiAnimator;

    private void Start() {
        #if UNITY_EDITOR
        if(!volume.profile.TryGet(out vignette)) {
            Debug.LogWarning("Can't not find vignette in profile in the volume");
            return;
        }
        if(!volume.profile.TryGet(out motionBlur)) {
            Debug.LogWarning("Can't not find motion blur in profile in the volume");
            return;
        }
        if(!volume.profile.TryGet(out chromaticAberration)) {
            Debug.LogWarning("Can't not find chromatic aberration in profile in the volume");
            return;
        }
        #endif
        
        origin = new Status(pixelCam.assetsPPU, vignette.color.value);
        effect = new Status(resolution, vignetteColor);
    }

    public void Play() {
        uiAnimator.SetTrigger("Play");
        if(cinematicCoroutine != null)
            StopCoroutine(cinematicCoroutine);
        cinematicCoroutine = StartCoroutine(CinematicCoroutine());
    }
    private IEnumerator CinematicCoroutine() {
        motionBlur.active = true;
        chromaticAberration.active = true;
        GameManager.instance.StageManager.CameraDirector.LockNoise(true);

        float offset = 0;
        StageManager.SetTimeScale(0.1f);

        offset = 0.1f;
        while(offset < 1) {
            StageManager.SetTimeScale(offset);
            vignette.color.value = Color.Lerp(origin.vignetteColor, effect.vignetteColor, 1-Mathf.Pow(1-offset, 2));
            pixelCam.assetsPPU = (int) Mathf.Lerp(origin.ppu, effect.ppu, 1-Mathf.Pow(1-offset, 2));
            offset += Time.deltaTime / Mathf.Max(Time.timeScale, 0.01f);
            yield return null;
        }

        motionBlur.active = false;
        chromaticAberration.active = false;
        GameManager.instance.StageManager.CameraDirector.LockNoise(false);
        StageManager.SetTimeScale(1);

        offset = 0;
        while(offset < 1) {
            vignette.color.value = Color.Lerp(effect.vignetteColor, origin.vignetteColor, offset * offset);
            pixelCam.assetsPPU = (int) Mathf.Lerp(effect.ppu, origin.ppu, offset * offset);
            offset += Time.deltaTime * 2f;
            yield return null;
        }

        vignette.color.value = origin.vignetteColor;
        pixelCam.assetsPPU = origin.ppu;
    }
}