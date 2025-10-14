using UnityEngine;

public class Shoot : MonoBehaviour
{
    InputMaster inputMaster;

    [SerializeField] ShakeTransformEventData2 shootPositionShake;
    [SerializeField] ShakeTransformEventData2 shootRotationShake;
    [SerializeField] ShakeTransform2 shakeTransform;

    void Awake()
    {
        inputMaster = new InputMaster();
        inputMaster.Player.Shoot.performed += ctx => ShootBullet();
    }

    void OnEnable()
    {
        inputMaster.Player.Enable();
    }
    void OnDisable()
    {
        inputMaster.Player.Disable();
    }

    void ShootBullet()
    {
        Debug.Log("Pew Pew!");
        shakeTransform.AddShakeEvent(shootPositionShake);
        shakeTransform.AddShakeEvent(shootRotationShake);
    }
}
