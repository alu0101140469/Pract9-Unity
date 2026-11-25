
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    // Declaración de una variable de tipo AudioSource
    // para utilizarla en el código.
    private AudioSource altavoces;

    // AudioClip generado por el micrófono.
    private AudioClip microphoneClip;

    // AudioClip definitivo con el sonido grabado.
    private AudioClip grabacion;

    // Nombre del dispositivo de entrada.
    private string dispositivo;

    // Indica si el micrófono está grabando.
    private bool grabando = false;

    // Parámetros de la grabación.
    public int duracion = 30;
    public int frecuencia = 44100;

    // Índice del micrófono que se utilizará.
    public int indiceMicrofono = 0;


    // Start se ejecuta antes del primer Update.
    void Start()
    {
        // Recuperar el componente AudioSource
        // que se agrega a los altavoces.
        altavoces = GetComponent<AudioSource>();

        altavoces.playOnAwake = false;

        // Recuperar los dispositivos de micrófono disponibles.
        string[] dispositivos = Microphone.devices;

        // Comprobar que existe al menos un micrófono.
        if (dispositivos.Length == 0)
        {
            Debug.LogError("No se ha detectado ningún micrófono.");
            enabled = false;
            return;
        }

        // Mostrar los micrófonos disponibles en la consola.
        for (int i = 0; i < dispositivos.Length; i++)
        {
            Debug.Log("Micrófono disponible: " + dispositivos[i]);
        }

        // Seleccionar el micrófono que se utilizará.
        indiceMicrofono = Mathf.Clamp(
            indiceMicrofono,
            0,
            dispositivos.Length - 1
        );

        dispositivo = dispositivos[indiceMicrofono];

        Debug.Log("Micrófono seleccionado: " + dispositivo);


        // Iniciar la recogida de audio por el micrófono.
        // El cuarto parámetro establece la frecuencia
        // de muestreo recomendada, que es 44100 Hz.
        microphoneClip = Microphone.Start(
            dispositivo,
            false,
            duracion,
            frecuencia
        );

        if (microphoneClip == null)
        {
            Debug.LogError("No se ha podido iniciar la grabación.");
            enabled = false;
            return;
        }

        grabando = true;

        Debug.Log("Grabación iniciada.");
    }


    // Update se ejecuta una vez por frame.
    void Update()
    {
        // En cada frame comprobar si se ha pulsado R
        // para reproducir el clip obtenido por el micrófono.
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Si todavía se está grabando, finalizar
            // primero la captura del micrófono.
            if (grabando)
            {
                DetenerMicrofono();
            }

            // Reproducir el clip generado por el micrófono.
            ReproducirGrabacion();
        }


        // Comprobar si el usuario ha decidido detener
        // la captación de audio por el micrófono.
        // Utilizamos la tecla E para esta operación.
        if (Input.GetKeyDown(KeyCode.E))
        {
            DetenerMicrofono();
        }
    }


    // Finalizar la captación de audio por el micrófono.
    void DetenerMicrofono()
    {
        if (!grabando)
        {
            return;
        }

        // Recuperar la posición actual de la grabación.
        // Indica cuántas muestras se han capturado.
        int muestrasGrabadas =
            Microphone.GetPosition(dispositivo);

        // Finalizar la grabación.
        Microphone.End(dispositivo);

        grabando = false;

        if (muestrasGrabadas <= 0 || microphoneClip == null)
        {
            Debug.LogWarning("La grabación está vacía.");
            return;
        }

        // Evitar superar el tamaño del AudioClip original.
        muestrasGrabadas = Mathf.Min(
            muestrasGrabadas,
            microphoneClip.samples
        );

        // Recperar los datos de audio.
        int canales = microphoneClip.channels;

        float[] datosAudio =
            new float[muestrasGrabadas * canales];

        bool datosCorrectos =
            microphoneClip.GetData(datosAudio, 0);

        if (!datosCorrectos)
        {
            Debug.LogError("No se han podido recuperar los datos del audio.");
            return;
        }

        // Crear un nuevo AudioClip con la duración real grabada.
        grabacion = AudioClip.Create(
            "GrabacionMicrofono",
            muestrasGrabadas,
            canales,
            microphoneClip.frequency,
            false
        );

        // Copiar las muestras al nuevo AudioClip.
        grabacion.SetData(datosAudio, 0);

        // Liberar el clip temporal.
        Destroy(microphoneClip);

        microphoneClip = null;

        Debug.Log("Grabación finalizada correctamente.");
    }


    // Reproducir el sonido capturado por el micrófono.
    void ReproducirGrabacion()
    {
        if (grabacion == null)
        {
            Debug.LogWarning(
                "No existe ninguna grabación para reproducir."
            );

            return;
        }

        // Asociar el AudioClip al AudioSource.
        altavoces.clip = grabacion;

        // Reproducir el sonido.
        altavoces.Stop();

        altavoces.Play();

        Debug.Log("Reproduciendo la grabación del micrófono.");
    }


    // Detener la grabación y liberar los recursos
    // cuando se cierra o descarga la escena.
    void OnDestroy()
    {
        if (grabando && !string.IsNullOrEmpty(dispositivo))
        {
            Microphone.End(dispositivo);
        }

        if (microphoneClip != null)
        {
            Destroy(microphoneClip);
        }

        if (grabacion != null)
        {
            Destroy(grabacion);
        }
    }
}
