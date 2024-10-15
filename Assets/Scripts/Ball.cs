using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(AudioSource))]
public class Ball : MonoBehaviour
{
    #region Constantes
    private const float MIN_DRAG_DISTANCE = 1f;    // Distancia mínima para aplicar fuerza
    private const float VELOCITY_THRESHOLD = 0.2f; // Umbral de velocidad para permitir nuevo arrastre
    private const float LINE_WIDTH = 0.02f;        // Ancho de la línea del LineRenderer
    private const int RESOLUTION = 20;             // Resolución de la parábola (cantidad de puntos)
    private const float GRAVITY = -9.8f;           // Gravedad simulada
    #endregion

    #region Referencias
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb;                // Referencia al Rigidbody2D de la pelota
    [SerializeField] private LineRenderer lr;               // Referencia al LineRenderer para la línea de dirección en el suelo
    [SerializeField] private Camera mainCamera;             // Referencia a la cámara principal
    [SerializeField] private AudioSource audioSource;       // Referencia al AudioSource para los sonidos
    [SerializeField] private GameObject particlePrefab;     // Prefab del sistema de partículas
    [SerializeField] private AudioClip[] hitSounds;         // Lista de sonidos de golpeo
    [SerializeField] private GameObject hitIndicatorPrefab; // Prefab del indicador de golpeo
    #endregion

    #region Atributos
    [Header("Atributos")]
    [SerializeField] private float maxPower = 10f;             // Potencia máxima aplicable al tiro
    [SerializeField] private float powerMultiplier = 2f;       // Multiplicador de la potencia aplicada
    [SerializeField] private float particleLifetime = 2f;      // Tiempo que durará el prefab de partículas
    [SerializeField] private float maxAdditionalForce = 5f;    // Fuerza máxima adicional para golpes en el aire
    [SerializeField] private float airHitRadius = 1.5f;        // Radio del área de clic para golpes en el aire
    #endregion

    #region Variables Privadas
    private bool isDragging = false;        // Indica si el jugador está arrastrando la pelota (modo suelo)
    private Vector2 dragStartPosition;      // Posición inicial del arrastre (modo suelo)

    private bool isAiming = false;          // Indica si el jugador está apuntando para un golpe en el aire
    private Vector2 aimStartPosition;       // Posición inicial al empezar a apuntar
    private GameObject currentHitIndicator;  // Instancia actual del indicador de golpeo
    #endregion

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (lr == null)
            lr = GetComponent<LineRenderer>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        ConfigurarLineRenderer();
    }

    private void Update()
    {
        ManejarEntradaDelJugador();
    }

    private void ConfigurarLineRenderer()
    {
        lr.positionCount = 0;
        lr.startWidth = LINE_WIDTH;
        lr.endWidth = LINE_WIDTH;
        lr.numCapVertices = 10; // Opcional, para bordes más suaves
        lr.material = new Material(Shader.Find("Sprites/Default"));
        Color lineColor = Color.white;
        lineColor.a = 0f; // Transparente al final de la línea
        lr.startColor = lineColor;
        lr.endColor = lineColor;
    }

    private void ManejarEntradaDelJugador()
    {
        if (rb.velocity.magnitude < VELOCITY_THRESHOLD)
        {
            // Modo Suelo: Drag & Drop
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 posicionEntrada = ObtenerPosicionDelMouse();
                RaycastHit2D hit = Physics2D.Raycast(posicionEntrada, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    isDragging = true;
                    dragStartPosition = posicionEntrada;
                    IniciarArrastre(posicionEntrada);
                }
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector2 posicionActual = ObtenerPosicionDelMouse();
                ActualizarArrastre(posicionActual);
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                Vector2 posicionFinal = ObtenerPosicionDelMouse();
                SoltarArrastre(posicionFinal);
            }
        }
        else
        {
            // Modo Aire: Golpe con Click dentro de airHitRadius
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 posicionEntrada = ObtenerPosicionDelMouse();
                float distancia = Vector2.Distance(posicionEntrada, rb.position);

                if (distancia <= airHitRadius)
                {
                    isAiming = true;
                    aimStartPosition = posicionEntrada;
                    IniciarAiming(posicionEntrada);
                }
            }

            if (Input.GetMouseButton(0) && isAiming)
            {
                Vector2 posicionActual = ObtenerPosicionDelMouse();
                ActualizarAiming(posicionActual);
            }

            if (Input.GetMouseButtonUp(0) && isAiming)
            {
                Vector2 posicionFinal = ObtenerPosicionDelMouse();
                SoltarAiming(posicionFinal);
            }
        }
    }

    private bool EstaListo()
    {
        return rb.velocity.magnitude < VELOCITY_THRESHOLD;
    }

    private Vector2 ObtenerPosicionDelMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z; // Asegurar que la posición Z sea correcta
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    #region Modo Suelo: Drag & Drop

    private void IniciarArrastre(Vector2 posicion)
    {
        lr.positionCount = RESOLUTION;
        MostrarLinea();
    }

    private void ActualizarArrastre(Vector2 posicion)
    {
        Vector2 direccion = (Vector2)transform.position - posicion;
        Vector2 fuerzaInicial = Vector2.ClampMagnitude(direccion, maxPower) * powerMultiplier;
        DibujarTrayectoria(transform.position, fuerzaInicial);
    }

    private void SoltarArrastre(Vector2 posicion)
    {
        OcultarLinea();
        isDragging = false;

        Vector2 direccion = (Vector2)transform.position - posicion;
        float distancia = direccion.magnitude;

        if (distancia < MIN_DRAG_DISTANCE)
            return;

        Vector2 fuerzaAplicada = Vector2.ClampMagnitude(direccion, maxPower) * powerMultiplier;
        rb.AddForce(fuerzaAplicada, ForceMode2D.Impulse);

        // Emitir sonido de golpe aleatorio
        EmitirSonidoGolpe();

        // Instanciar el prefab de partículas
        InstanciarParticulas(fuerzaAplicada.magnitude);
    }

    #endregion

    #region Modo Aire: Golpe con Click

    private void IniciarAiming(Vector2 posicion)
    {
        // Instanciar el indicador de golpeo
        if (hitIndicatorPrefab != null)
        {
            currentHitIndicator = Instantiate(hitIndicatorPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    private void ActualizarAiming(Vector2 posicion)
    {
        if (currentHitIndicator != null)
        {
            // Calcula la dirección y fuerza basada en la posición actual del mouse
            Vector2 direccion = (Vector2)transform.position - posicion;
            float fuerza = Mathf.Clamp(direccion.magnitude, 0f, maxAdditionalForce);

            // Ajusta la orientación del indicador
            float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            currentHitIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Ajusta la escala del indicador para mostrar la fuerza
            float scale = Mathf.Lerp(0.5f, 2f, fuerza / maxAdditionalForce);
            currentHitIndicator.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    private void SoltarAiming(Vector2 posicion)
    {
        if (currentHitIndicator != null)
        {
            Destroy(currentHitIndicator);
        }

        isAiming = false;

        Vector2 direccion = (Vector2)transform.position - posicion;
        float fuerza = Mathf.Clamp(direccion.magnitude, 0f, maxAdditionalForce);

        if (fuerza < MIN_DRAG_DISTANCE)
            return;

        // Aplicar fuerza basada en la dirección y magnitud
        Vector2 fuerzaAplicada = Vector2.ClampMagnitude(direccion, maxAdditionalForce) * powerMultiplier;
        rb.AddForce(fuerzaAplicada, ForceMode2D.Impulse);

        // Emitir sonido de golpe aleatorio
        EmitirSonidoGolpe();

        // Instanciar el prefab de partículas
        InstanciarParticulas(fuerzaAplicada.magnitude);
    }

    #endregion

    #region Sonidos y Partículas

    private void EmitirSonidoGolpe()
    {
        if (hitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            audioSource.PlayOneShot(hitSounds[randomIndex]);
        }
    }

    private void InstanciarParticulas(float fuerza)
    {
        if (particlePrefab != null)
        {
            // Instanciar el prefab en la posición actual de la pelota
            GameObject particulas = Instantiate(particlePrefab, transform.position, Quaternion.identity, transform);

            // Ajustar la escala de las partículas según la fuerza
            float scale = Mathf.Lerp(0.5f, 2f, fuerza / maxPower);
            particulas.transform.localScale = Vector3.one * scale;

            // Destruir las partículas después de un tiempo definido
            Destroy(particulas, particleLifetime);
        }
    }

    #endregion

    #region Trayectoria Parabólica

    private void DibujarTrayectoria(Vector2 startPos, Vector2 fuerzaInicial)
    {
        float tiempoDeVuelo = (2 * fuerzaInicial.y / -GRAVITY);
        float pasoDeTiempo = tiempoDeVuelo / RESOLUTION;

        for (int i = 0; i < RESOLUTION; i++)
        {
            float tiempo = pasoDeTiempo * i;
            Vector2 posicion = CalcularPosicionParabolica(startPos, fuerzaInicial, tiempo);
            lr.SetPosition(i, posicion);
        }
    }

    private Vector2 CalcularPosicionParabolica(Vector2 startPos, Vector2 velocidadInicial, float tiempo)
    {
        float posX = startPos.x + velocidadInicial.x * tiempo;
        float posY = startPos.y + velocidadInicial.y * tiempo + 0.5f * GRAVITY * Mathf.Pow(tiempo, 2);
        return new Vector2(posX, posY);
    }

    private void MostrarLinea()
    {
        Color colorInicial = Color.white;
        colorInicial.a = 1f; // Opaco al inicio de la línea
        lr.startColor = colorInicial;

        Color colorFinal = Color.white;
        colorFinal.a = 0f; // Transparente al final de la línea
        lr.endColor = colorFinal;
    }

    private void OcultarLinea()
    {
        lr.positionCount = 0;
    }

    #endregion
}
