using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera camera;
    public CinemachineBasicMultiChannelPerlin perlinNoise;

    public float ShakeDuration = 0.3f;
    public float Max_ShakeAmplitude = 1.3f;
    public float Max_ShakeFreq = 3.6f;




    private void Start()
    {
        camera = this.GetComponent<CinemachineVirtualCamera>();
        perlinNoise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlinNoise.m_AmplitudeGain = 0;
        perlinNoise.m_FrequencyGain = 0;
    }

    public IEnumerator CinemachineShake(float duration, float magnitude)
    {
        float elapsed_t = 0f;
        perlinNoise.m_AmplitudeGain = Mathf.Clamp(magnitude, 0, Max_ShakeAmplitude);
        perlinNoise.m_FrequencyGain = Mathf.Clamp(magnitude, 0, Max_ShakeFreq);
        //print("starting...");
        //print("A:" + perlinNoise.m_AmplitudeGain + " ||  F: " + perlinNoise.m_FrequencyGain);
        while (elapsed_t < duration)
        {
            elapsed_t += Time.deltaTime;
            yield return null;
        }
        //print("ended.");

        perlinNoise.m_AmplitudeGain = 0;
        perlinNoise.m_FrequencyGain = 0;

    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed_t = 0f ;
        while(elapsed_t < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed_t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
