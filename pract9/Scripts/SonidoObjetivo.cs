
using UnityEngine;

// Script para reproducir un sonido cuando
// el guerrero alcanza un objetivo.

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]

public class SonidoObjetivo : MonoBehaviour
{
    // Sonido que se reproducirá al alcanzar el objetivo.
    public AudioClip sonidoObjetivo;

    // Etiqueta que identifica al guerrero.
    public string etiquetaJugador = "Player";

    // Componente encargado de reproducir el sonido.
    private AudioSource altavoz;

    // Evita reproducir varias veces el mismo sonido
    // si se activa esta opción.
    public bool reproducirUnaSolaVez = true;

    private bool sonidoReproducido = false;


    // Start se ejecuta antes del primer Update.
    void Start()
    {
        // Recuperar el componente AudioSource.
        altavoz = GetComponent<AudioSource>();

        // No reproducir el sonido automáticamente.
        altavoz.playOnAwake = false;

        // El Collider del objetivo será un Trigger.
        GetComponent<Collider>().isTrigger = true;
    }


    // Se ejecuta cuando otro Collider entra en el objetivo.
    void OnTriggerEnter(Collider otro)
    {
        // Comprobar si el objeto que ha entrado
        // corresponde al guerrero.
        bool esJugador =
            otro.CompareTag(etiquetaJugador) ||
            otro.transform.root.CompareTag(etiquetaJugador);

        if (!esJugador)
        {
            return;
        }

        // Comprobar si el sonido ya se ha reproducido.
        if (reproducirUnaSolaVez && sonidoReproducido)
        {
            return;
        }

        // Comprobar que existe un sonido asignado.
        if (sonidoObjetivo == null)
        {
            Debug.LogWarning("No se ha asignado ningún AudioClip.");
            return;
        }

        // Reproducir el sonido.
        altavoz.PlayOneShot(sonidoObjetivo);

        sonidoReproducido = true;

        Debug.Log("El guerrero ha alcanzado el objetivo.");
    }
}
