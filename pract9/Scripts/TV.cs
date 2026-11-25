
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TV : MonoBehaviour
{
    // Declaración de variables:
    // Material, WebCamTexture y String para almacenar
    // el path del directorio donde guardar las imágenes.
    private Material tvMaterial;

    private WebCamTexture webcamTexture;

    private string savePath;

    int captureCounter = 1;

    // Dispositivos de cámara.
    public int indiceCamara = 0;

    // Resolución solicitada para la cámara.
    public int ancho = 1280;
    public int alto = 720;
    public int frecuencia = 30;

    // Lista para conservar las fotos en memoria.
    private List<Texture2D> capturasEnMemoria =
        new List<Texture2D>();


    // Start se ejecuta antes del primer Update.
    void Start()
    {
        // Inicializar el material al que posteriormente
        // se asignará cada imagen de la cámara.
        // Para ello obtenemos el Renderer del objeto
        // sobre el que se pinta y accedemos a su material.
        Renderer pantallaRenderer = GetComponent<Renderer>();

        if (pantallaRenderer == null)
        {
            Debug.LogError(
                "No se ha encontrado el componente Renderer."
            );

            enabled = false;
            return;
        }

        tvMaterial = pantallaRenderer.material;


        // Recuperar los dispositivos de cámara disponibles.
        WebCamDevice[] dispositivos = WebCamTexture.devices;

        if (dispositivos.Length == 0)
        {
            Debug.LogError("No se ha detectado ninguna cámara.");

            enabled = false;
            return;
        }


        // Mostrar el nombre de las cámaras en la consola.
        for (int i = 0; i < dispositivos.Length; i++)
        {
            Debug.Log(
                "Cámara disponible: " + dispositivos[i].name
            );
        }


        // Seleccionar la cámara que se utilizará.
        indiceCamara = Mathf.Clamp(
            indiceCamara,
            0,
            dispositivos.Length - 1
        );

        string nombreCamara = dispositivos[indiceCamara].name;

        Debug.Log("Cámara seleccionada: " + nombreCamara);


        // Usar el constructor de WebCamTexture
        // para inicializar una variable de ese tipo.
        webcamTexture = new WebCamTexture(
            nombreCamara,
            ancho,
            alto,
            frecuencia
        );


        // Establecer la ruta donde se guardarán las fotos.
        savePath = Path.Combine(
            Application.persistentDataPath,
            "CapturasCamara"
        );

        // Crear el directorio si todavía no existe.
        Directory.CreateDirectory(savePath);

        Debug.Log("Directorio de capturas: " + savePath);
    }


    // Update se ejecuta una vez por frame.
    void Update()
    {
        if (webcamTexture == null)
        {
            return;
        }


        // Iniciar la cámara al pulsar S.
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Asignar a la mainTexture del material
            // lo que está captando la cámara.
            tvMaterial.mainTexture = webcamTexture;

            // Iniciar la captura.
            webcamTexture.Play();

            Debug.Log("Cámara iniciada.");
        }


        // Pausar la captura al pulsar P.
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (webcamTexture.isPlaying)
            {
                webcamTexture.Pause();

                Debug.Log("Cámara pausada.");
            }
        }


        // Detener la captura al pulsar T.
        if (Input.GetKeyDown(KeyCode.T))
        {
            webcamTexture.Stop();

            Debug.Log("Cámara detenida.");
        }


        // Capturar un fotograma al pulsar X.
        if (Input.GetKeyDown(KeyCode.X))
        {
            TomarFotograma();
        }
    }


    // Capturar y guardar un fotograma de la cámara.
    void TomarFotograma()
    {
        // Comprobar que la cámara está funcionando.
        if (!webcamTexture.isPlaying)
        {
            Debug.LogWarning(
                "Debes iniciar la cámara antes de capturar."
            );

            return;
        }

        // Comprobar que existe un fotograma válido.
        if (webcamTexture.width <= 16 ||
            webcamTexture.height <= 16)
        {
            Debug.LogWarning(
                "La cámara todavía no ha generado un fotograma válido. " +
                "Resolución actual: " +
                webcamTexture.width + "x" + webcamTexture.height
            );

            return;
        }


        // Crear un objeto Texture2D.
        Texture2D snapshot = null;

        try
        {
            snapshot = new Texture2D(
                webcamTexture.width,
                webcamTexture.height
            );

            // Pasar a la textura los píxeles que capta la cámara.
            snapshot.SetPixels(
                webcamTexture.GetPixels()
            );

            // Aplicar los cambios realizados en la textura.
            snapshot.Apply();

            // Generar el nombre del archivo.
            string nombreArchivo =
                captureCounter.ToString() + ".png";

            string rutaCompleta = Path.Combine(
                savePath,
                nombreArchivo
            );


            // Evitar sobrescribir fotod anteriores.
            while (File.Exists(rutaCompleta))
            {
                captureCounter++;

                nombreArchivo =
                    captureCounter.ToString() + ".png";

                rutaCompleta = Path.Combine(
                    savePath,
                    nombreArchivo
                );
            }


            // Almacenar la textura en un fichero PNG.
            File.WriteAllBytes(
                rutaCompleta,
                snapshot.EncodeToPNG()
            );


            // Conservar la foto como imagen fija en memoria.
            capturasEnMemoria.Add(snapshot);

            // La lista se hace responsable de conservarla.
            // No hay que destruirla después de guardarla.
            snapshot = null;


            Debug.Log(
                "Foto guardada: " + rutaCompleta
            );

            captureCounter++;
        }
        catch (System.Exception excepcion)
        {
            Debug.LogError(
                "Error al guardar la foto: " +
                excepcion.Message
            );
        }
        finally
        {
            // Si ha ocurrido un error antes de añadir
            // la foto a la lista, liberar la textura.
            if (snapshot != null)
            {
                Destroy(snapshot);
            }
        }
    }


    // Liberar los recursos al dejar la escena.
    void OnDestroy()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();

            if (tvMaterial != null &&
                tvMaterial.mainTexture == webcamTexture)
            {
                tvMaterial.mainTexture = null;
            }

            Destroy(webcamTexture);
        }


        // Liberar el material creado para la pantalla.
        if (tvMaterial != null)
        {
            Destroy(tvMaterial);
        }


        // Liberar las fotos almacenadas en memoria.
        foreach (Texture2D imagen in capturasEnMemoria)
        {
            if (imagen != null)
            {
                Destroy(imagen);
            }
        }

        capturasEnMemoria.Clear();
    }
}
