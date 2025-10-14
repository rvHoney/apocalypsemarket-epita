using UnityEngine;

public class GunFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;        // Handles left-right rotation (yaw)
    [SerializeField] private Transform cameraParent;  // Handles up-down rotation (pitch)
    [SerializeField] private Transform hand;          // The hand or gun anchor point

    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float positionSpeed = 10f;
    [SerializeField] private float swayAmount = 1f;
    [SerializeField] private float swaySmooth = 6f;

    [Header("Lag Settings")]
    [SerializeField] private float cameraLagAmount = 0.05f;
    [SerializeField] private float cameraLagSmooth = 10f;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilKickBack = 0.1f;
    [SerializeField] private float recoilKickUp = 2f;
    [SerializeField] private float recoilRecoverySpeed = 10f;

    private Vector3 cameraLagOffset;
    private Vector2 previousLookDir;
    private Vector2 smoothedInput;
    private Vector2 currentInput;

    private Vector3 recoilPosOffset;
    private Vector3 targetRecoilPos;
    private Quaternion recoilRotOffset = Quaternion.identity;
    private Quaternion targetRecoilRot = Quaternion.identity;

    private InputMaster inputMaster;

    void Awake()
    {
        inputMaster = new InputMaster();
        inputMaster.Player.Shoot.performed += ctx => AddRecoil();
    }

    void OnEnable() => inputMaster.Enable();
    void OnDisable() => inputMaster.Disable();

    private void LateUpdate()
    {
        // --- CAMERA LAG ---
        Vector2 lookDir = new Vector2(cameraParent.eulerAngles.x, player.eulerAngles.y);
        Vector2 lookDelta = lookDir - previousLookDir;

        if (lookDelta.x > 180) lookDelta.x -= 360;
        if (lookDelta.x < -180) lookDelta.x += 360;
        if (lookDelta.y > 180) lookDelta.y -= 360;
        if (lookDelta.y < -180) lookDelta.y += 360;

        Vector3 targetOffset = new Vector3(-lookDelta.y, -lookDelta.x, 0f) * cameraLagAmount;
        cameraLagOffset = Vector3.Lerp(cameraLagOffset, targetOffset, Time.deltaTime * cameraLagSmooth);

        // --- RECOIL SMOOTH RETURN ---
        recoilPosOffset = Vector3.Lerp(recoilPosOffset, targetRecoilPos, Time.deltaTime * recoilRecoverySpeed);
        recoilRotOffset = Quaternion.Slerp(recoilRotOffset, targetRecoilRot, Time.deltaTime * recoilRecoverySpeed);

        // --- ROTATION BASE ---
        Quaternion yaw = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
        Quaternion pitch = Quaternion.Euler(cameraParent.eulerAngles.x, 0f, 0f);
        Quaternion baseRotation = yaw * pitch;

        Quaternion swayRotation = GetSwayRotation(baseRotation);
        Quaternion targetRotation = baseRotation * swayRotation * recoilRotOffset;

        // --- APPLY POSITION (recoil in local space) ---
        Vector3 localRecoil = targetRotation * recoilPosOffset; // convert to world direction
        transform.position = Vector3.Lerp(
            transform.position,
            hand.position + cameraLagOffset + localRecoil,
            1f - Mathf.Exp(-positionSpeed * Time.deltaTime)
        );

        // --- APPLY ROTATION ---
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            1f - Mathf.Exp(-rotationSpeed * Time.deltaTime)
        );

        previousLookDir = lookDir;
    }

    private Quaternion GetSwayRotation(Quaternion baseRotation)
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        currentInput = new Vector2(mouseX, mouseY);
        smoothedInput = Vector2.Lerp(smoothedInput, currentInput, Time.deltaTime * swaySmooth);

        float swayX = -smoothedInput.y * swayAmount;
        float swayY = smoothedInput.x * swayAmount;

        return Quaternion.Euler(swayX, swayY, 0f);
    }

    // --- 🔫 RECOIL ---
    public void AddRecoil()
    {
        Debug.Log("Recoil Added");

        // Recoil applied in local space (backward & upward)
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
}
