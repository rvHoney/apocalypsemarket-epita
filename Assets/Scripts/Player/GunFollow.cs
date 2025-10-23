using UnityEngine;

public class GunFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraTransform; // use actual Camera.main.transform
    [SerializeField] private Transform hand;
    [SerializeField] private Gun gun;

    [Header("Follow Settings")]
    [SerializeField] private float positionFollowSpeed = 20f;
    [SerializeField] private float rotationFollowSpeed = 20f;

    [Header("Sway Settings")]
    [SerializeField] private float swayAmount = 1f;
    [SerializeField] private float swaySmooth = 6f;

    [Header("Camera Lag")]
    [SerializeField] private float cameraLagAmount = 0.05f;
    [SerializeField] private float cameraLagSmooth = 10f;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilKickBack = 0.08f;
    [SerializeField] private float recoilKickUp = 2f;
    [SerializeField] private float recoilReturnSpeed = 15f;

    private Vector3 cameraLagOffset;
    private Vector2 previousLookDir;

    private Vector3 recoilPosOffset;
    private Vector3 targetRecoilPos;
    private Quaternion recoilRotOffset = Quaternion.identity;
    private Quaternion targetRecoilRot = Quaternion.identity;

    private Vector2 smoothedInput;

    private void OnEnable()
    {
        if (gun != null)
            gun.OnShoot += AddRecoil;
    }

    private void OnDisable()
    {
        if (gun != null)
            gun.OnShoot -= AddRecoil;
    }

    private void LateUpdate()
    {
        Vector2 lookDir = new Vector2(cameraTransform.eulerAngles.x, player.eulerAngles.y);
        Vector2 lookDelta = lookDir - previousLookDir;
        lookDelta.x = NormalizeAngle(lookDelta.x);
        lookDelta.y = NormalizeAngle(lookDelta.y);

        Vector3 targetOffset = new Vector3(-lookDelta.y, -lookDelta.x, 0f) * cameraLagAmount;
        cameraLagOffset = Vector3.Lerp(cameraLagOffset, targetOffset, Time.deltaTime * cameraLagSmooth);

        recoilPosOffset = Vector3.Lerp(recoilPosOffset, targetRecoilPos, Time.deltaTime * recoilReturnSpeed);
        recoilRotOffset = Quaternion.Slerp(recoilRotOffset, targetRecoilRot, Time.deltaTime * recoilReturnSpeed);

        Quaternion swayRotation = GetSwayRotation();

        Quaternion targetRotation = cameraTransform.rotation * swayRotation * recoilRotOffset;

        Vector3 desiredPosition = hand.position + cameraLagOffset + (targetRotation * recoilPosOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionFollowSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationFollowSpeed);

        previousLookDir = lookDir;
    }

    private Quaternion GetSwayRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        Vector2 input = new Vector2(mouseX, mouseY);
        smoothedInput = Vector2.Lerp(smoothedInput, input, Time.deltaTime * swaySmooth);

        float swayX = -smoothedInput.y * swayAmount;
        float swayY = smoothedInput.x * swayAmount;

        return Quaternion.Euler(swayX, swayY, 0f);
    }

    private void AddRecoil()
    {
        targetRecoilPos = new Vector3(0f, 0f, -recoilKickBack);
        targetRecoilRot = Quaternion.Euler(-recoilKickUp, 0f, 0f);

        CancelInvoke(nameof(ResetRecoil));
        Invoke(nameof(ResetRecoil), 0.05f);
    }

    private void ResetRecoil()
    {
        targetRecoilPos = Vector3.zero;
        targetRecoilRot = Quaternion.identity;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return angle;
    }
}
