# Práctica 9: cámara y micrófono en Unity

**Asignatura:** Interfaces Inteligentes  
**Práctica:** Micrófono y cámara
**Entorno de desarrollo:** Unity 6.5

## Alumno: 
Daniel Palenzuela Álvarez alu0101140469

## 1. Introducción y objetivo

En esta práctica he desarrollado un proyecto en Unity para trabajar con distintos componentes de entrada y salida del ordenador. El objetivo principal es utilizar el micrófono y la cámara web, además de reproducir sonidos dentro de una escena 3D. La práctica está organizada en dos escenas independientes:

- **`pract9ej1.unity`**: escena de los guerreros y el objetivo. Cuando el personaje entra en el área del objetivo, se reproduce un sonido.
- **`pract9ej2.unity`**: escena multimodal con una pantalla central y altavoces. Permite grabar audio desde el micrófono, reproducirlo y mostrar la imagen captada por la cámara web, además de también permitir guardar fotografías en formato PNG.

Para la realización de la práctica he creado cuatro scripts: `SonidoObjetivo.cs`, `MovimientoGuerrero.cs`, `Recorder.cs` y `TV.cs`.

## 2. Recursos utilizados

- Un ordenador con cámara web y micrófono.
- El paquete de personaje **Mini Legion Footman PBR HP Polyart**, utilizado como modelo del guerrero. [Unity Asset Store](https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/mini-legion-footman-pbr-hp-polyart-86576).
- El paquete de sonidos proporcionado para la práctica (`Sonidos práctica Cámara-Micrófono-20261124.zip`), cuyos archivos de audio se importan en el proyecto para poder asignar un `AudioClip` al objetivo.

## 3. Preparación del proyecto

### 3.1. Creación del proyecto y organización

Creé un proyecto en Unity 6.5, organizando los recursos en carpetas para separar las escenas, los scripts, los modelos, los materiales y los sonidos, también importando el paquete del guerrero desde Unity Asset Store, descomprimiendo e importando los sonidos para la práctica, guardándolos dentro del proyecto, en `Assets/Audio`. He guardado las dos escenas del ejercicio con los nombres `pract9ej1.unity` y `pract9ej2.unity`, y he añadido los scripts a `Assets/Scripts` y los he asociado a los objetos correspondientes de cada escena.

Una organización orientativa de los recursos del proyecto es la siguiente:

```text
Assets/
├── Audio/                 # Sonidos proporcionados para la práctica
├── Models/                # Modelos y recursos del personaje
├── Scenes/
│   ├── pract9ej1.unity    # Guerrero y objetivo
│   └── pract9ej2.unity    # Micrófono y cámara
└── Scripts/
    ├── SonidoObjetivo.cs
    ├── MovimientoGuerrero.cs
    ├── Recorder.cs
    └── TV.cs
```

## 4. Primera escena: guerrero y objetivo (`pract9ej1.unity`)

### 4.1. Construcción de la escena

Para el primer ejercicio he preparado un espacio sencillo con un suelo (`Plane`), iluminación y una cámara principal. Después he incorporado el prefab `FootmanHP`, correspondiente al personaje guerrero del paquete importado.

En el objeto raíz del personaje he configurado:

- La etiqueta **`Player`**, para que el script pueda reconocer al guerrero.
- Un `CapsuleCollider`, para detectar colisiones con el área del objetivo.
- Un `Rigidbody`, configurado como cinemático y sin gravedad, ya que el desplazamiento se realiza mediante el script `MovimientoGuerrero`.
- El script `MovimientoGuerrero.cs`, que permite controlar el movimiento del personaje con el teclado.

A continuación he creado el objeto **`Objetivo`**. Este objeto tiene un `Collider` con **Is Trigger** activado, un `AudioSource` y el script `SonidoObjetivo.cs`. En el campo `Sonido Objetivo` del Inspector he asignado uno de los sonidos importados.

El `AudioSource` tiene desactivado `Play On Awake` para evitar que el sonido se reproduzca al iniciar la escena. El sonido se reproduce únicamente cuando se produce la entrada del personaje en el área del objetivo.

### 4.2. Script `SonidoObjetivo.cs`

Este script se encarga de reproducir el sonido cuando el guerrero alcanza el objetivo. Su funcionamiento es el siguiente:

1. En `Start()` obtiene el componente `AudioSource` del propio objetivo.
2. Desactiva `playOnAwake` y activa `isTrigger` en el `Collider`.
3. Unity llama a `OnTriggerEnter(Collider otro)` cuando otro collider entra en el área del objetivo.
4. El script comprueba si el objeto que ha entrado tiene la etiqueta `Player` o si la tiene su objeto raíz.
5. Comprueba que hay un `AudioClip` asignado y que, si está activada la opción `reproducirUnaSolaVez`, el sonido todavía no se ha reproducido.
6. Si se cumplen las condiciones, ejecuta `altavoz.PlayOneShot(sonidoObjetivo)` y muestra un mensaje en la consola.

La variable `etiquetaJugador` permite indicar la etiqueta que identifica al personaje. La variable `reproducirUnaSolaVez` evita que el sonido se repita al permanecer o volver a entrar en el objetivo. En esta escena está activada. El atributo `[RequireComponent]` indica que el objeto que utiliza este script debe disponer de un `Collider` y de un `AudioSource`.

### 4.3. Script `MovimientoGuerrero.cs`

Este script es una ampliación para poder desplazar el guerrero por la escena y alcanzar el objetivo. Su funcionamiento es el siguiente:

- En `Start()` recupera el componente `Rigidbody`, activa `isKinematic` y desactiva `useGravity`.
- En `Update()` lee los ejes `Horizontal` y `Vertical` mediante `Input.GetAxisRaw`.
- Con esos valores crea un vector de dirección sobre el plano XZ.
- Utiliza `Vector3.ClampMagnitude` para que el movimiento diagonal no sea más rápido que el movimiento horizontal o vertical.
- En `FixedUpdate()` desplaza el `Rigidbody` mediante `MovePosition` y gira el personaje hacia la dirección en la que se está moviendo con `MoveRotation` y `Quaternion.LookRotation`.

El script requiere un `Rigidbody`, indicado mediante `[RequireComponent(typeof(Rigidbody))]`.

**Controles de esta escena:**

| Tecla | Acción |
|---|---|
| `W` / Flecha arriba | Avanzar |
| `S` / Flecha abajo | Retroceder |
| `A` / Flecha izquierda | Desplazarse a la izquierda |
| `D` / Flecha derecha | Desplazarse a la derecha |

Al entrar en el collider del objetivo, se reproduce el `AudioClip` asignado.

## 5. Segunda escena: micrófono y cámara (`pract9ej2.unity`)

### 5.1. Construcción de la escena

Para el segundo ejercicio he creado un espacio abierto con un suelo (`Plane`), iluminación y una cámara principal. En la zona central he colocado un objeto `Quad` llamado **`Pantalla`**, que actúa como superficie donde se muestra la imagen captada por la cámara web. La pantalla está configurada con posición aproximada `(0, 1.5, 0)`, escala `(3, 2, 1)` y rotación `(0, 0, 0)`.

He añadido los objetos visuales `AltavozIzq` y `AltavozDer` a ambos lados de la pantalla. Además, he creado un objeto llamado **`Altavoces`**, al que he añadido un `AudioSource` y el script `Recorder.cs`. Este `AudioSource` es el componente que reproduce el audio grabado.

Por último, he añadido `TV.cs` al objeto `Pantalla`. El script obtiene automáticamente el componente `Renderer` del mismo objeto, por lo que no es necesario asignar manualmente el material desde el Inspector.

### 5.2. Script `Recorder.cs`

Este script utiliza el micrófono del ordenador para grabar audio y reproducir posteriormente la grabación mediante un `AudioSource`. En el proyecto está asociado al objeto `Altavoces`, que también contiene el componente `AudioSource`.

#### Inicialización

En `Start()`:

1. Recupera el componente `AudioSource` mediante `GetComponent<AudioSource>()` y desactiva `playOnAwake`.
2. Obtiene los micrófonos disponibles con `Microphone.devices`.
3. Muestra sus nombres en la consola.
4. Selecciona el dispositivo indicado por `indiceMicrofono`, limitando el índice para que esté dentro del rango disponible.
5. Inicia la grabación con `Microphone.Start(dispositivo, false, duracion, frecuencia)`.

Los parámetros configurados en la escena son:

- `duracion = 30`: duración máxima de la grabación en segundos.
- `frecuencia = 44100`: frecuencia de muestreo de 44.100 Hz.
- `indiceMicrofono = 0`: selecciona inicialmente el primer dispositivo detectado.

La grabación comienza automáticamente al ejecutar la escena. Si no se detecta ningún micrófono, el script muestra un error en la consola y se desactiva.

#### Finalización de la grabación

El método `DetenerMicrofono()` realiza las siguientes operaciones:

1. Comprueba que la grabación está activa.
2. Obtiene la posición actual del micrófono con `Microphone.GetPosition`, que permite conocer las muestras capturadas.
3. Finaliza la captación mediante `Microphone.End`.
4. Recupera los datos de audio con `GetData`.
5. Crea un nuevo `AudioClip` con la duración real de la grabación mediante `AudioClip.Create`.
6. Copia las muestras al nuevo clip con `SetData`.
7. Libera el clip temporal utilizado durante la grabación.

De este modo, el clip definitivo contiene el audio que se ha grabado y no necesariamente los 30 segundos completos reservados inicialmente.

#### Reproducción

El método `ReproducirGrabacion()` asigna el clip creado al `AudioSource` (`altavoces.clip`) y utiliza `Stop()` y `Play()` para reproducirlo. Si todavía no existe una grabación, muestra un aviso en la consola.

**Controles del micrófono:**

| Tecla | Acción |
|---|---|
| `R` | Finaliza la grabación si está activa y reproduce el audio obtenido. Si la grabación ya se había detenido con `E`, reproduce la última grabación disponible. |
| `E` | Finaliza la captación del micrófono sin reproducir inmediatamente el audio. |

El enunciado indica que al pulsar `R` debe reproducirse el sonido obtenido por el micrófono. La tecla `E` se ha añadido como una ampliación para poder detener la captación de forma explícita antes de reproducirla. También se puede pulsar `R` directamente mientras se está grabando, el script finaliza primero la grabación y después la reproduce. En `OnDestroy()` se detiene el micrófono si todavía estaba grabando y se liberan los clips creados, evitando mantener recursos de audio cuando se descarga la escena.

### 5.3. Script `TV.cs`

Este script controla la cámara web y permite mostrar su imagen en el objeto `Pantalla`, así como guardar fotografías aisladas.

#### Inicialización de la pantalla y de la cámara

En `Start()`:

1. Recupera el componente `Renderer` del objeto al que está asociado el script.
2. Obtiene una instancia del material mediante `pantallaRenderer.material`.
3. Recupera las cámaras disponibles con `WebCamTexture.devices`.
4. Muestra el nombre de cada cámara detectada en la consola.
5. Selecciona la cámara indicada por `indiceCamara`.
6. Crea un objeto `WebCamTexture` con el dispositivo elegido y los parámetros de resolución y frecuencia.
7. Prepara el directorio donde se guardarán las fotografías.

Los valores configurados en la escena son:

- `indiceCamara = 0`: selecciona inicialmente la primera cámara detectada.
- `ancho = 1280` y `alto = 720`: resolución solicitada de 1280 × 720 píxeles.
- `frecuencia = 30`: frecuencia solicitada de 30 fotogramas por segundo.

Si no se encuentra el componente `Renderer` o no se detecta ninguna cámara, el script muestra un error y se desactiva.

#### Visualización de la cámara

En `Update()` se comprueban las teclas del teclado:

- Al pulsar `S`, asigna `webcamTexture` a `tvMaterial.mainTexture` y llama a `webcamTexture.Play()`. Así, la imagen captada se muestra sobre la pantalla de la escena.
- Al pulsar `P`, llama a `webcamTexture.Pause()` si la cámara está reproduciendo.
- Al pulsar `T`, llama a `webcamTexture.Stop()` para detener la captación.
- Al pulsar `X`, llama al método `TomarFotograma()`.

#### Captura y almacenamiento de fotografías

El método `TomarFotograma()` comprueba primero que la cámara esté iniciada y que la textura tenga unas dimensiones válidas. A continuación:

1. Crea una nueva `Texture2D` con la anchura y altura de la imagen de la cámara.
2. Copia los píxeles captados mediante `snapshot.SetPixels(webcamTexture.GetPixels())`.
3. Aplica los cambios con `snapshot.Apply()`.
4. Genera un nombre de archivo numérico, por ejemplo `1.png`, `2.png`, etc.
5. Comprueba si el nombre ya existe y busca el siguiente número disponible para no sobrescribir fotografías anteriores.
6. Convierte la textura a PNG con `EncodeToPNG()` y escribe los bytes en un fichero mediante `File.WriteAllBytes()`.
7. Añade la textura a `capturasEnMemoria`, de modo que también se conserve como imagen fija en memoria durante la ejecución.

El directorio de salida se construye con `Application.persistentDataPath` y el subdirectorio `CapturasCamara`. La ruta completa se muestra en la consola al iniciar la escena. Las fotografías se guardan fuera de `Assets`, en el directorio persistente de datos de la aplicación. En `OnDestroy()` se detiene la cámara, se desvincula la textura de la pantalla cuando corresponde y se liberan el objeto `WebCamTexture`, el material instanciado y las fotografías que permanecían en memoria.

**Controles de la cámara:**

| Tecla | Acción |
|---|---|
| `S` | Iniciar la cámara y mostrar su imagen en `Pantalla`. |
| `P` | Pausar la captación de vídeo. |
| `T` | Detener la cámara. |
| `X` | Capturar el fotograma actual y guardarlo como PNG. |

## 6. Ejecución y comprobación de la práctica

### 6.1. Comprobación de la escena del guerrero

1. Abrir `pract9ej1.unity`.
2. Pulsar **Play**.
3. Mover el personaje con WASD o las flechas del teclado.
4. Dirigirlo hacia el objeto `Objetivo`.
5. Al entrar en el área del objetivo, se reproduce el sonido asignado y aparece el mensaje correspondiente en la consola.

### 6.2. Comprobación de la escena multimodal

1. Abrir `pract9ej2.unity`.
2. Comprobar que el objeto `Altavoces` tiene los componentes `AudioSource` y `Recorder`, y que `Pantalla` tiene el script `TV`.
3. Pulsar **Play**. El micrófono comienza a grabar automáticamente.
4. Hablar cerca del micrófono.
5. Pulsar `R` para finalizar la grabación y escucharla, o pulsar `E` para detenerla y después `R` para reproducirla.
6. Pulsar `S` para iniciar la cámara y comprobar que la imagen aparece en el Quad `Pantalla`.
7. Pulsar `P` para pausar la cámara y `S` para volver a iniciarla. También se puede utilizar `T` para detenerla.
8. Con la cámara iniciada, pulsar `X` para guardar una fotografía.
9. Consultar en la consola la ruta del directorio `CapturasCamara` y comprobar que se ha creado el fichero PNG.

En las pruebas realizadas en mi ordenador, Unity detectó la cámara y el micrófono integrados en la **EMEET SmartCam C60E 4K**. El índice `0` seleccionó la EMEET en esa configuración. En otro ordenador los nombres, el número y el orden de los dispositivos pueden ser diferentes, en ese caso hay que ajustar `indiceCamara` o `indiceMicrofono` desde el Inspector.

## 7. Scripts incluidos

| Script | Función principal | Escena |
|---|---|---|
| `SonidoObjetivo.cs` | Detecta la entrada del guerrero en el objetivo y reproduce un sonido. | `pract9ej1.unity` |
| `MovimientoGuerrero.cs` | Permite desplazar y orientar el personaje mediante el teclado. | `pract9ej1.unity` |
| `Recorder.cs` | Graba audio desde el micrófono, crea un `AudioClip` con las muestras capturadas y lo reproduce. | `pract9ej2.unity` |
| `TV.cs` | Gestiona la cámara web, muestra la imagen en la pantalla y guarda capturas PNG. | `pract9ej2.unity` |
