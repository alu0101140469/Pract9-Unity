
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoGuerrero : MonoBehaviour
{
    // Velocidad de movimientp.
    public float velocidad = 4f;

    private Rigidbody rb;

    private Vector3 direccion;

    void Start()
    {
        // Recuperar el componente Rigidbody.
        rb = GetComponent<Rigidbody>();

        // El movimiento se controla mediante el script.
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update()
    {
        // Obtener el movimiento horizontal y vertical.
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Crear la dirección del movimiento sobre el suelo.
        direccion = new Vector3(horizontal, 0f, vertical);

        // Evitar que el movimiento diagonal sea más rápido.
        direccion = Vector3.ClampMagnitude(direccion, 1f);
    }

    void FixedUpdate()
    {
        if (direccion.sqrMagnitude > 0.01f)
        {
            // Mover.
            rb.MovePosition(
                rb.position + direccion * velocidad * Time.fixedDeltaTime
            );

            // Girar hacia la dirección del movimiento.
            rb.MoveRotation(
                Quaternion.LookRotation(direccion)
            );
        }
    }
}
