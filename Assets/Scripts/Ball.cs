using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(AudioSource))]
public class Ball : MonoBehaviour
{
    #region Constantes
    private const float MIN_DRAG_DISTANCE = 1f;
    private const float VELOCITY_THRESHOLD = 0.2f;
    private const float LINE_WIDTH = 0.02f;
    private const int RESOLUTION = 20;
    private const float GRAVITY = -9.8f;
    #endregion

    #region Referencias
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private GameObject hitIndicatorPrefab;
    #endregion

    #region Atributos
    [Header("Atributos")]
    [SerializeField] private float maxPower = 10f;
    [SerializeField] private float powerMultiplier = 2f;
    [SerializeField] private float particleLifetime = 2f;
    [SerializeField] private float maxAdditionalForce = 5f;
    [SerializeField] private float airHitRadius = 1.5f;
    [SerializeField] private AnimationCurve forceCurve;
    [SerializeField] private float deadZoneY = -10f;
    [SerializeField] private float maxAirHitDistance = 3f;
    #endregion

    #region Colores de Trayectoria
    [Header("Colores de Trayectoria")]
    [SerializeField] private Color colorFuerzaMinima = Color.green;
    [SerializeField] private Color colorFuerzaMaxima = Color.red;
    #endregion

    #region Variables Privadas
    private bool isDragging = false;
    private Vector2 dragStartPosition;
    private bool isAiming = false;
    private Vector2 aimStartPosition;
    private GameObject currentHitIndicator;
    private Gradient lineGradient;
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
        
        lineGradient = new Gradient();
    }

    private void Start()
    {
        ConfigurarLineRenderer();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameOver)
        {
            ManejarEntradaDelJugador();
            VerificarDeadZone();
        }
    }

    private void ConfigurarLineRenderer()
    {
        lr.positionCount = RESOLUTION;
        lr.startWidth = LINE_WIDTH;
        lr.endWidth = LINE_WIDTH;
        lr.numCapVertices = 10;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.useWorldSpace = true;
    }

    private void ManejarEntradaDelJugador()
    {
        if (rb.velocity.magnitude < VELOCITY_THRESHOLD)
        {
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

    private Vector2 ObtenerPosicionDelMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    #region Modo Suelo: Drag & Drop

    private void IniciarArrastre(Vector2 posicion)
    {
        lr.positionCount = RESOLUTION;
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

        EmitirSonidoGolpe();
        InstanciarParticulas(fuerzaAplicada.magnitude);
    }

    #endregion

    #region Modo Aire: Golpe con Click

    private void IniciarAiming(Vector2 posicion)
    {
        if (hitIndicatorPrefab != null)
        {
            currentHitIndicator = Instantiate(hitIndicatorPrefab, transform.position, Quaternion.identity, transform);
        }
        lr.positionCount = RESOLUTION;
    }

    private void ActualizarAiming(Vector2 posicion)
    {
        if (currentHitIndicator != null)
        {
            Vector2 direccion = (Vector2)transform.position - posicion;
            float distancia = direccion.magnitude;
            float fuerzaNormalizada = Mathf.Clamp01(distancia / maxAirHitDistance);
            float fuerza = fuerzaNormalizada * maxAdditionalForce;

            float curveValue = forceCurve.Evaluate(fuerzaNormalizada);
            fuerza = curveValue * maxAdditionalForce;

            float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            currentHitIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);

            float scale = Mathf.Lerp(0.5f, 2f, fuerzaNormalizada);
            currentHitIndicator.transform.localScale = new Vector3(scale, scale, 1f);

            Vector2 fuerzaInicial = direccion.normalized * fuerza;
            DibujarTrayectoria(transform.position, fuerzaInicial);
        }
    }

    private void SoltarAiming(Vector2 posicion)
    {
        if (currentHitIndicator != null)
        {
            Destroy(currentHitIndicator);
        }

        OcultarLinea();
        isAiming = false;

        Vector2 direccion = (Vector2)transform.position - posicion;
        float distancia = direccion.magnitude;
        float fuerzaNormalizada = Mathf.Clamp01(distancia / maxAirHitDistance);
        float fuerza = fuerzaNormalizada * maxAdditionalForce;

        float curveValue = forceCurve.Evaluate(fuerzaNormalizada);
        fuerza = curveValue * maxAdditionalForce;

        if (fuerza < MIN_DRAG_DISTANCE)
            return;

        Vector2 fuerzaAplicada = direccion.normalized * fuerza;
        rb.AddForce(fuerzaAplicada, ForceMode2D.Impulse);

        EmitirSonidoGolpe();
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
            GameObject particulas = Instantiate(particlePrefab, transform.position, Quaternion.identity, transform);

            float scale = Mathf.Lerp(0.5f, 2f, fuerza / maxPower);
            particulas.transform.localScale = Vector3.one * scale;

            Destroy(particulas, particleLifetime);
        }
    }

    #endregion

    #region Trayectoria Parabólica

    private void DibujarTrayectoria(Vector2 startPos, Vector2 fuerzaInicial)
    {
        float tiempoDeVuelo = (2 * fuerzaInicial.y / -GRAVITY);
        float pasoDeTiempo = tiempoDeVuelo / RESOLUTION;

        float fuerzaNormalizada = fuerzaInicial.magnitude / (maxPower * powerMultiplier);

        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(colorFuerzaMinima, 0f);
        colorKeys[1] = new GradientColorKey(colorFuerzaMaxima, 1f);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(0f, 1f);

        lineGradient.SetKeys(colorKeys, alphaKeys);

        lr.colorGradient = lineGradient;

        for (int i = 0; i < RESOLUTION; i++)
        {
            float tiempo = pasoDeTiempo * i;
            Vector2 posicion = CalcularPosicionParabolica(startPos, fuerzaInicial, tiempo);
            lr.SetPosition(i, posicion);
        }

        Color startColor = Color.Lerp(colorFuerzaMinima, colorFuerzaMaxima, fuerzaNormalizada);
        startColor.a = 1f;
        Color endColor = startColor;
        endColor.a = 0f;

        lr.startColor = startColor;
        lr.endColor = endColor;
    }

    private Vector2 CalcularPosicionParabolica(Vector2 startPos, Vector2 velocidadInicial, float tiempo)
    {
        float posX = startPos.x + velocidadInicial.x * tiempo;
        float posY = startPos.y + velocidadInicial.y * tiempo + 0.5f * GRAVITY * Mathf.Pow(tiempo, 2);
        return new Vector2(posX, posY);
    }

    private void OcultarLinea()
    {
        lr.positionCount = 0;
    }

    #endregion

    #region Interacción con GameManager y SaveSystem

    private void VerificarDeadZone()
    {
        if (transform.position.y < deadZoneY && !GameManager.Instance.IsGameOver)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void ResetBall(Vector3 startPosition)
    {
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void LoadBallData(Vector3 position, Vector2 velocity)
    {
        transform.position = position;
        rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            GameManager.Instance.AddCoin();
            Destroy(collision.gameObject);
        }
    }

    #endregion
}