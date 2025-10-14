using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeTransform2 : MonoBehaviour
{
    [System.Serializable]
    public class ShakeEvent
    {
        float duration;
        public float timeRemaining;
        public bool loop;

        ShakeTransformEventData2 data;

        public ShakeTransformEventData2.Target target
        {
            get
            {
                return data.target;
            }
        }

        Vector3 noiseOffset;
        public Vector3 noise;

        public ShakeEvent(ShakeTransformEventData2 data)
        {
            this.data = data;
            loop = data.loop;

            duration = data.duration;
            timeRemaining = duration;

            float rand = 32.0f;

            noiseOffset.x = Random.Range(0.0f, rand);
            noiseOffset.y = Random.Range(0.0f, rand);
            noiseOffset.z = Random.Range(0.0f, rand);
        }

        public void Update()
        {
            float deltaTime = Time.deltaTime;

            timeRemaining -= deltaTime;

            float noiseOffsetDelta = deltaTime * data.frequency;

            noiseOffset.x += noiseOffsetDelta;
            noiseOffset.y += noiseOffsetDelta;
            noiseOffset.z += noiseOffsetDelta;
            
           

            noise.x = Mathf.PerlinNoise(noiseOffset.x, 0.0f);
            noise.y = Mathf.PerlinNoise(noiseOffset.y, 1.0f);
            noise.z = Mathf.PerlinNoise(noiseOffset.z, 1.0f);
            
            

            noise -= Vector3.one * 0.5f;

            float zValue = data.z_amplitude * noise.z;
            if(target == ShakeTransformEventData2.Target.Position) { zValue = 0f; }

            noise = new Vector3(data.x_amplitude * noise.x, data.y_amplitude * noise.y, zValue);    
            
            float agePercent = 1.0f - (timeRemaining / duration);
            noise *= data.blendOverLifetime.Evaluate(agePercent);
        }
        public bool IsAlive()
        {
            return timeRemaining > 0.0f;
        }
    }

    // ...

    List<ShakeEvent> shakeEvents = new List<ShakeEvent>();

    // ...

    public void AddShakeEvent(ShakeTransformEventData2 data)
    {
        shakeEvents.Add(new ShakeEvent(data));
    }
    public void AddShakeEvent(float x_amplitude, float y_amplitude, float z_amplitude, float frequency, float duration, AnimationCurve blendOverLifetime, ShakeTransformEventData2.Target target, bool loop)
    {
        ShakeTransformEventData2 data = ShakeTransformEventData2.CreateInstance<ShakeTransformEventData2>();
        data.Init(x_amplitude, y_amplitude, z_amplitude, frequency, duration, blendOverLifetime, target, loop);

        AddShakeEvent(data);
    }

    void Update()
    {
        Vector3 positionOffset = Vector3.zero;
        Vector3 rotationOffset = Vector3.zero;

        for (int i = shakeEvents.Count - 1; i != -1; i--)
        {
            ShakeEvent se = shakeEvents[i]; se.Update();

            if (se.target == ShakeTransformEventData2.Target.Position)
            {
                positionOffset += se.noise;
            }
            else
            {
                rotationOffset += se.noise;
            }

            if (!se.IsAlive() && !se.loop)
            {
                shakeEvents.RemoveAt(i);
            }
        }

        transform.localPosition = positionOffset;
        transform.localEulerAngles = rotationOffset;
    }
}
