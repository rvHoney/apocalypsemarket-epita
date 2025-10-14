using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Shake transform event", menuName = "Custom/Shake transform event 2", order = 1)]
public class ShakeTransformEventData2 : ScriptableObject
{
    public enum Target
    {
        Position,
        Rotation
    }
    public Target target = Target.Position;

    public float x_amplitude = 1f;
    public float y_amplitude = 1f;
    public float z_amplitude = 1f;
    public float frequency = 1f;

    public float duration = 1f;

    public AnimationCurve blendOverLifetime = new AnimationCurve(
        new Keyframe(0.0f, 0.0f, Mathf.Deg2Rad * 0.0f, Mathf.Deg2Rad * 720.0f),
        new Keyframe(0.2f, 1.0f),
        new Keyframe(1.0f, 0.0f));

    public bool loop = true;

    public void Init(float x_amplitude, float y_amplitude, float z_amplitude, float frequency, float duration, AnimationCurve blendOverLifetime, Target target, bool loop)
    {
        this.target = target;

        this.x_amplitude = x_amplitude;
        this.y_amplitude = y_amplitude;
        this.z_amplitude = z_amplitude;
      
        this.frequency = frequency;

        this.duration = duration;

        this.blendOverLifetime = blendOverLifetime;

        this.loop = loop;        
    }
}
