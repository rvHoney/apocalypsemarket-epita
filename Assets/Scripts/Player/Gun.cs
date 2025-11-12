using UnityEngine;
using System;
using System.Collections;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject currentGun;

    [Header("Fire")]
    public float fireRate = 5f;
    public float range = 100f;
    public int damage = 25;
    public LayerMask hitMask = ~0;

    [Header("Muzzle Flash")]
    public GameObject muzzlePrefab;
    public Transform muzzlePosition;

    [Header("Impact Effect")]
    public GameObject impactEffectPrefab;
    public float impactForce = 50f;

    [Header("Trail")]
    public GameObject tracerPrefab;
    [SerializeField] private float trailDuration = 0.05f;


    [Header("Audio")]
    public AudioClip shotSfx;
    public AudioSource audioSource;

    float nextFireTime = 0f;

    public event Action OnShoot;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        UpdateGunData();
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (CameraShakeHandler.Instance != null)
        {
            CameraShakeHandler.Instance.ShootCameraShake();
        }

        OnShoot?.Invoke();

        if (muzzlePrefab != null && muzzlePosition != null)
        {
            var flash = Instantiate(muzzlePrefab, muzzlePosition);
            Destroy(flash, 2f);
        }

        if (shotSfx != null && audioSource != null)
            audioSource.PlayOneShot(shotSfx);

        Vector3 origin = (muzzlePosition != null) ? muzzlePosition.position : transform.position;
        Vector3 forward = Camera.main.transform.forward;

        if (Physics.Raycast(Camera.main.transform.position, forward, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            print("HIT: " + hit.collider.name);
            var dmg = hit.collider.GetComponent<IDamageable>();
            if (dmg != null) dmg.TakeDamage(damage);

            if (hit.rigidbody != null)
                hit.rigidbody.AddForceAtPosition(forward * impactForce, hit.point, ForceMode.Impulse);

            if (impactEffectPrefab != null)
            {
                var impactGO = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                StartCoroutine(fadeMaterial(impactGO.GetComponent<Renderer>(), 4f));
            }

            if (tracerPrefab != null)
                StartCoroutine(SpawnTracer(origin, hit.point));

        }
        else
        {
            // show trail ending at max distance
            Vector3 endPoint = origin + forward * range;
            StartCoroutine(SpawnTracer(origin, endPoint));
        }
    }
    private IEnumerator SpawnTracer(Vector3 start, Vector3 end)
    {
        GameObject tracerInstance = Instantiate(tracerPrefab, start, Quaternion.identity);
        LineRenderer tracer = tracerInstance.GetComponent<LineRenderer>();
        tracer.SetPosition(0, start);
        tracer.SetPosition(1, end);

        // Get a unique material instance
        StartCoroutine(fadeMaterial(tracerInstance.GetComponent<Renderer>(), trailDuration));
        yield break;
    }
    private IEnumerator fadeMaterial(Renderer rend, float duration)
    {
        if (rend == null) yield break;

        // Create a unique material instance
        Material mat = rend.material;
        if (mat == null) yield break;

        float elapsed = 0f;
        Color initialColor = mat.color;
        Color initialEmission = Color.black;
        if (mat.HasProperty("_EmissionColor"))
            initialEmission = mat.GetColor("_EmissionColor");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (mat != null)
            {
                mat.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(initialColor.a, 0f, t));
                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", Color.Lerp(initialEmission, Color.black, t));
            }

            yield return null;
        }

        if (rend != null)
            Destroy(rend.gameObject);
    }
    void UpdateGunData()
    {
        if (currentGun)
        {
            GunProperties properties = currentGun.GetComponent<GunID>().properties;
            fireRate = properties.fireRate;
            range = properties.range;
            damage = properties.damage;
            AmoDisplay.Instance.UpdateAmoDisplay(properties.magazineCapacity, properties.magazineCapacity);
        }
    }
}



public interface IDamageable
{
    void TakeDamage(int amount);
}
