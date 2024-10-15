using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ParticleSystem))]
public class Ball : MonoBehaviour
{
    #region Constantes
    private const float MIN_DRAG_DISTANCE = 1f;    // Distancia mínima para aplicar fuerza
    private const float VELOCITY_THRESHOLD = 0.2f; // Umbral de velocidad para permitir nuevo arrastre
    private const float LINE_WIDTH = 0.02f;        // Ancho de la línea del LineRenderer
    private const int LINE_CAP_VERTICES = 10;      // Cantidad de vértices para bordes redondeados en el LineRenderer
    private const int RESOLUTION = 20;             // Resolución de la parábola (cantidad de puntos)
    private const float GRAVITY = -9.8f;           // Gravedad simulada
    #endregion

    #region Referencias
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb;       // Referencia al Rigidbody2D de la pelota
    [SerializeField] private LineRenderer lr;       // Referencia al LineRenderer para la línea de dirección
    [SerializeField] private Camera mainCamera;     // Referencia a la cámara principal
    [SerializeField] private AudioSource audioSource; // Referencia al AudioSource para los sonidos
    [SerializeField] private ParticleSystem particleSystem; // Referencia al sistema de partículas
    [SerializeField] private AudioClip[] hitSounds; // Lista de sonidos de golpeo
    #endregion

    #region Atributos
    [Header("Atributos")]
    [SerializeField] private float maxPower = 10f;         // Potencia máxima aplicable al tiro
    [SerializeField] private float powerMultiplier = 2f;   // Multiplicador de la potencia aplicada
    [SerializeField] private float maxGoalSpeed = 4f;      // Velocidad máxima permitida para entrar en el hoyo
    #endregion

    #region Variables Privadas
    private bool isDragging = false;   // Indica si el jugador está arrastrando la pelota
    private Vector2 dragStartPosition; // Posición inicial del arrastre
    #endregion

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (lr == null)
            lr = GetComponent<LineRenderer>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();
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
        lr.numCapVertices = LINE_CAP_VERTICES;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        Color lineColor = Color.white;
        lineColor.a = 0f; // Transparente al final de la línea
        lr.startColor = lineColor;
        lr.endColor = lineColor;
    }

    private void ManejarEntradaDelJugador()
    {
        if (!EstaListo())
            return;

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

        // Emitir partículas, escalando según la fuerza aplicada
        EmitirParticulas(fuerzaAplicada.magnitude);
    }

    private void EmitirSonidoGolpe()
    {
        if (hitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            audioSource.PlayOneShot(hitSounds[randomIndex]);
        }
    }

    private void EmitirParticulas(float fuerza)
    {
        var emission = particleSystem.emission;
        var main = particleSystem.main;

        // Ajustar la cantidad de partículas según la fuerza
        emission.rateOverTime = fuerza * 10;

        // Ajustar la escala de las partículas según la fuerza
        main.startSize = Mathf.Lerp(0.1f, 1f, fuerza / maxPower);

        // Reproducir el sistema de partículas
        particleSystem.Play();
    }

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
        colorInicial.a = 1f; // Opaque al inicio de la línea
        lr.startColor = colorInicial;

        Color colorFinal = Color.white;
        colorFinal.a = 0f; // Transparente al final de la línea
        lr.endColor = colorFinal;
    }

    private void OcultarLinea()
    {
        lr.positionCount = 0;
    }
}
