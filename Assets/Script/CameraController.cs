using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] public float smoothSpeed = 10f;
    [SerializeField] private float limitX = 25f;
    [SerializeField] private float limitY = 35f;

    private float targetRotationX = 0f;
    private float targetRotationY = 0f;
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;

    private Vector3 originalPosition;
    private Coroutine cameraShake;

    private void OnEnable()
    {
        originalPosition = transform.position;
        EventManager.OnBalanceLost += OccurCollision;
        EventManager.OnRockCollision += OccurCollision;
        EventManager.OnCargoUnitBroke += LostCargo;
    }

    private void OnDisable()
    {
        EventManager.OnBalanceLost -= OccurCollision;
        EventManager.OnRockCollision -= OccurCollision;
        EventManager.OnCargoUnitBroke -= LostCargo;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        targetRotationX += mouseX;
        targetRotationY -= mouseY;

        targetRotationX = Mathf.Clamp(targetRotationX, -limitX, limitX);
        targetRotationY = Mathf.Clamp(targetRotationY, -limitY, limitY);
        currentRotationX = Mathf.Lerp(currentRotationX, targetRotationX, smoothSpeed * Time.deltaTime);
        currentRotationY = Mathf.Lerp(currentRotationY, targetRotationY, smoothSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotationY, currentRotationX, 0f);
    }

    private void OccurCollision()
    {
        ShakeCamera(1.0f);
    }

    private void LostCargo(int power)
    {
        ShakeCamera(power * 0.5f);
    }

    private void ShakeCamera(float power)
    {
        if (cameraShake != null)
        {
            StopCoroutine(cameraShake);
            transform.localPosition = originalPosition;
        }

        cameraShake = StartCoroutine(ShakeCoroutine(power));
    }

    IEnumerator ShakeCoroutine(float power)
    {
        float shakeTimer = power;

        while(0 < shakeTimer)
        {
            Vector2 randomPosition = Random.insideUnitCircle * 0.05f;
            if (power * 0.3f < shakeTimer && shakeTimer < power * 0.7f) randomPosition *= 3;

            transform.localPosition = new Vector3(originalPosition.x + randomPosition.x, originalPosition.y + randomPosition.y, originalPosition.z);

            shakeTimer -= Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
