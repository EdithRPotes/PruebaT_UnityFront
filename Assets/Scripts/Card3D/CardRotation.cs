using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRotation : MonoBehaviour
{
    public float rotationSpeed = 180f; // Velocidad del giro (grados por segundo)
    private bool hasRotated = false;   // Controla si ya se realiz� la rotaci�n

    public void StartRotation()
    {
        if (!hasRotated)  // Solo gira si no ha girado antes
        {
            StartCoroutine(RotateOnce());
        }
    }

    private IEnumerator RotateOnce()
    {
        hasRotated = true;
        float startAngle = transform.eulerAngles.y;
        float targetAngle = startAngle + 360f; // Un giro completo

        while (Mathf.Abs(transform.eulerAngles.y - targetAngle) > 0.1f)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, step, 0);
            yield return null; // Espera al siguiente frame
        }

        // Asegura la rotaci�n exacta
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            targetAngle % 360,
            transform.eulerAngles.z
        );
    }

    // M�todo para reiniciar la rotaci�n
    public void ResetRotation()
    {
        hasRotated = false;  // Permite una nueva rotaci�n
        transform.eulerAngles = Vector3.zero;  // Restaura la orientaci�n original
    }
}
