using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraDirector : MonoBehaviour {
    [SerializeField] private SkillCinematics skillCinematics;
    [SerializeField] private CinemachineVirtualCamera characterCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake() {
        noise = characterCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    
    public void PlaySkillCinematics() {
        skillCinematics.Play();
    }

    #region Shake Character's Camera
    private Coroutine shakeCoroutine;
    public void LockNoise(bool state) {
        noise.enabled = !state;
    }
    public void ShakeCamera(float time, float scale) {
        if(noise == null) {
            Debug.LogWarning("Can not found noise in the virtual camera.");
            return;
        }

        if(shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine(time, scale, false));
    }
    public void ShakeCameraCurved(float time, float scale) {
        if(noise == null) {
            Debug.LogWarning("Can not found noise in the virtual camera.");
            return;
        }

        if(shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine(time, scale, true));
    }
    private IEnumerator ShakeCoroutine(float time, float scale, bool curved) {
        float offset = 0;
        float step = scale / time;
        while(offset < time) {
            noise.m_AmplitudeGain = scale - (curved ? offset * step : 0);
            offset += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        noise.m_AmplitudeGain = 0;
    }
    #endregion Shake Character's Camera
}