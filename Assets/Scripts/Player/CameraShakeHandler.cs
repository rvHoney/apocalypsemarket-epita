using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script created by Antoine(Icescream)

[System.Serializable]
public class ShakeEvents
{
    //That's dirty, but it works and looks clean in inspector
    //Basically taking all shake datas from the hierarchy to have a reference in script
    public ShakeTransformEventData2 CrouchingPosition;
    public ShakeTransformEventData2 CrouchingRotation;

    public ShakeTransformEventData2 IdlePosition;
    public ShakeTransformEventData2 IdleRotation;

    public ShakeTransformEventData2 WalkingPosition;
    public ShakeTransformEventData2 WalkingRotation;

    public ShakeTransformEventData2 RunningPosition;
    public ShakeTransformEventData2 RunningRotation;
}
public class CameraShakeHandler : MonoBehaviour
{

    public ShakeTransform2 st;
    public ShakeEvents events;
    private ShakeTransformEventData2 positionShake, rotationShake;
    private ShakeTransformEventData2 currentPositionShake, currentRotationShake;
    private ShakeTransformEventData2 targetPositionShake, targetRotationShake;

    private float t;
    public float transitionDuration = 1f;

    private PlayerMove pMove;

    PlayerMove.playerMovementStatus _moveStatus;
    private PlayerMove.playerMovementStatus MoveStatus
    {
        get { return _moveStatus; }
        set
        {
            if(_moveStatus != value)
            {
                _moveStatus = value;
                UpdateShake(value);
            }
        }
    }    

    bool changeStatus = false;

    private void Awake()
    {
        pMove = GetComponent<PlayerMove>();

        SetupDefaultCameraShakeEvents();    
    }

    private void Update()
    {
        MoveStatus = pMove.playerStatus;                    

        //smoothly transition between shake events with interpolation
        if (changeStatus)
        {
            t += Time.deltaTime / transitionDuration;

            positionShake.x_amplitude = Mathf.SmoothStep(currentPositionShake.x_amplitude, targetPositionShake.x_amplitude, t);
            positionShake.y_amplitude = Mathf.SmoothStep(currentPositionShake.y_amplitude, targetPositionShake.y_amplitude, t);
            positionShake.frequency = Mathf.SmoothStep(currentPositionShake.frequency, targetPositionShake.frequency, t);
            //------------------------------------------------------------------------------------
            rotationShake.x_amplitude = Mathf.SmoothStep(currentRotationShake.x_amplitude, targetRotationShake.x_amplitude, t);
            rotationShake.y_amplitude = Mathf.SmoothStep(currentRotationShake.y_amplitude, targetRotationShake.y_amplitude, t);
            rotationShake.z_amplitude = Mathf.SmoothStep(currentRotationShake.z_amplitude, targetRotationShake.z_amplitude, t);
            rotationShake.frequency = Mathf.SmoothStep(currentRotationShake.frequency, targetRotationShake.frequency, t);

            //countdown to stop the smoothing once completed            
            if(t >= 1f)
            {
                changeStatus = false;
            }
        }
        
    }
    void UpdateShake(PlayerMove.playerMovementStatus status)
    {
        currentPositionShake = ScriptableObject.CreateInstance<ShakeTransformEventData2>();
        currentPositionShake.x_amplitude = positionShake.x_amplitude;
        currentPositionShake.y_amplitude = positionShake.y_amplitude;
        currentPositionShake.z_amplitude = positionShake.z_amplitude;
        currentPositionShake.frequency = positionShake.frequency;

        currentRotationShake = ScriptableObject.CreateInstance<ShakeTransformEventData2>();
        currentRotationShake.x_amplitude = rotationShake.x_amplitude;
        currentRotationShake.y_amplitude = rotationShake.y_amplitude;
        currentRotationShake.z_amplitude = rotationShake.z_amplitude;
        currentRotationShake.frequency = rotationShake.frequency;

        changeStatus = true;
        t = 0f;
        switch (status)
        {           
            case PlayerMove.playerMovementStatus.idle:
                targetPositionShake = events.IdlePosition;
                targetRotationShake = events.IdleRotation;
                break;
            case PlayerMove.playerMovementStatus.walking:
                targetPositionShake = events.WalkingPosition;
                targetRotationShake = events.WalkingRotation;
                break;
            case PlayerMove.playerMovementStatus.running:
                targetPositionShake = events.RunningPosition;
                targetRotationShake = events.RunningRotation;
                break;
            case PlayerMove.playerMovementStatus.crouching:
                targetPositionShake = events.CrouchingPosition;
                targetRotationShake = events.CrouchingRotation;
                break;
        }
    }

    void SetupDefaultCameraShakeEvents()
    {       

        //instanciating  the shake events that will be assigned to the camera.
        positionShake = ScriptableObject.CreateInstance<ShakeTransformEventData2>();
        positionShake.target = ShakeTransformEventData2.Target.Position;
        positionShake.x_amplitude = 0f;
        positionShake.y_amplitude = 0f;
        positionShake.frequency = 0f;
        positionShake.duration = 1f;
        positionShake.blendOverLifetime = AnimationCurve.Constant(0, 1, 1);
        //-------------------------------
        rotationShake = ScriptableObject.CreateInstance<ShakeTransformEventData2>();
        rotationShake.target = ShakeTransformEventData2.Target.Rotation;
        rotationShake.x_amplitude = 0f;
        rotationShake.y_amplitude = 0f;
        rotationShake.frequency = 0f;
        rotationShake.duration = 1f;
        rotationShake.blendOverLifetime = AnimationCurve.Constant(0, 1, 1);

        st.AddShakeEvent(positionShake);
        st.AddShakeEvent(rotationShake);
    }
}
