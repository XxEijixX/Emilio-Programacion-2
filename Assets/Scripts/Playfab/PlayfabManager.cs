using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;

    [Header("Inputs Login")]
    [SerializeField] private TMP_InputField emailLogin;
    [SerializeField] private TMP_InputField passwordLogin;

    [Header("Inputs Register")]
    [SerializeField] private TMP_InputField usernameRegister;
    [SerializeField] private TMP_InputField emailRegister;
    [SerializeField] private TMP_InputField passwordRegister;

    [Header("Buttons")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    [Header("Feedback UI")]
    [SerializeField] private TextMeshProUGUI loginFeedback;
    [SerializeField] private TextMeshProUGUI registerFeedback;
    [SerializeField] private TextMeshProUGUI messageError;

    [Header("Al Terminar Login/Register")]
    public UnityEvent onLoginSuccess;

    public string UsernameActual { get; private set; } = "";

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        if (loginButton != null) loginButton.onClick.AddListener(Login);
        if (registerButton != null) registerButton.onClick.AddListener(Register);

        if (emailLogin != null) emailLogin.onValueChanged.AddListener(OnLoginInputChanged);
        if (passwordLogin != null) passwordLogin.onValueChanged.AddListener(OnLoginInputChanged);

        if (usernameRegister != null) usernameRegister.onValueChanged.AddListener(OnRegisterInputChanged);
        if (emailRegister != null) emailRegister.onValueChanged.AddListener(OnRegisterInputChanged);
        if (passwordRegister != null) passwordRegister.onValueChanged.AddListener(OnRegisterInputChanged);

        UpdateLoginButtonState();
        UpdateRegisterButtonState();

        if (loginFeedback != null)
        {
            loginFeedback.text = "";
            loginFeedback.transform.parent.gameObject.SetActive(false);
        }
        if (registerFeedback != null)
        {
            registerFeedback.text = "";
            registerFeedback.transform.parent.gameObject.SetActive(false);
        }
        if (messageError != null) messageError.text = "";
    }

    private void OnDestroy()
    {
        if (loginButton != null) loginButton.onClick.RemoveListener(Login);
        if (registerButton != null) registerButton.onClick.RemoveListener(Register);

        if (emailLogin != null) emailLogin.onValueChanged.RemoveListener(OnLoginInputChanged);
        if (passwordLogin != null) passwordLogin.onValueChanged.RemoveListener(OnLoginInputChanged);

        if (usernameRegister != null) usernameRegister.onValueChanged.RemoveListener(OnRegisterInputChanged);
        if (emailRegister != null) emailRegister.onValueChanged.RemoveListener(OnRegisterInputChanged);
        if (passwordRegister != null) passwordRegister.onValueChanged.RemoveListener(OnRegisterInputChanged);
    }

    // ── Validación en tiempo real ────────────────────────────────────────────
    private void OnLoginInputChanged(string _)
    {
        LimpiarError();
        UpdateLoginButtonState();
    }

    private void OnRegisterInputChanged(string _)
    {
        LimpiarError();
        UpdateRegisterButtonState();
    }

    private void UpdateLoginButtonState()
    {
        if (loginButton == null) return;
        bool emailOk = emailLogin != null && !string.IsNullOrWhiteSpace(emailLogin.text);
        bool passOk = passwordLogin != null && !string.IsNullOrEmpty(passwordLogin.text);
        loginButton.interactable = emailOk && passOk;
    }

    private void UpdateRegisterButtonState()
    {
        if (registerButton == null) return;
        bool userOk = usernameRegister != null && !string.IsNullOrWhiteSpace(usernameRegister.text);
        bool emailOk = emailRegister != null && !string.IsNullOrWhiteSpace(emailRegister.text);
        bool passOk = passwordRegister != null && passwordRegister.text != null && passwordRegister.text.Length >= 6;
        registerButton.interactable = userOk && emailOk && passOk;
    }

    #region Login
    public void Login()
    {
        LimpiarError();

        string email = emailLogin != null ? emailLogin.text.Trim() : "";
        string password = passwordLogin != null ? passwordLogin.text : "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            MostrarError("Email or password cannot be empty");
            return;
        }

        if (loginButton != null) loginButton.interactable = false;
        SetLoginFeedback("Logging in...");

        // Pedimos el username dentro del mismo login con InfoRequestParameters
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true // Esto nos trae el DisplayName/username
            }
        },
        result =>
        {
            // Obtenemos el username desde el perfil que pedimos arriba
            string username = result.InfoResultPayload?.PlayerProfile?.DisplayName;

            // Si PlayFab no tiene DisplayName guardado, pedimos AccountInfo por separado
            if (string.IsNullOrEmpty(username))
            {
                ObtenerUsernameYFinalizar(result.PlayFabId);
            }
            else
            {
                FinalizarLogin(username);
            }

            if (emailLogin != null) emailLogin.text = "";
            if (passwordLogin != null) passwordLogin.text = "";
            UpdateLoginButtonState();
        },
        error =>
        {
            MostrarError("Error logging in: " + error.ErrorMessage);
            UpdateLoginButtonState();
        });
    }

    // Llamada secundaria para obtener el username si no vino en el login
    private void ObtenerUsernameYFinalizar(string playFabId)
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { PlayFabId = playFabId },
        result =>
        {
            string username = result.AccountInfo?.Username ?? "Jugador";
            FinalizarLogin(username);
        },
        error =>
        {
            FinalizarLogin("Player"); // Generic fallback
        });
    }

    private void FinalizarLogin(string username)
    {
        UsernameActual = username;
        SetLoginFeedback($"{username}");
        onLoginSuccess?.Invoke();
    }

    #endregion Login

    #region Register
    public void Register()
    {
        LimpiarError();

        string username = usernameRegister != null ? usernameRegister.text.Trim() : "";
        string email = emailRegister != null ? emailRegister.text.Trim() : "";
        string password = passwordRegister != null ? passwordRegister.text : "";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            MostrarError("One or more fields are empty");
            return;
        }
        if (password.Length < 6)
        {
            MostrarError("Password must be at least 6 characters long");
            return;
        }

        if (registerButton != null) registerButton.interactable = false;
        SetRegisterFeedback("Registering...");

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = username,
            RequireBothUsernameAndEmail = true,
            DisplayName = username // Guardamos el username como DisplayName
        },
        result =>
        {
            SetRegisterFeedback($"Registration successful! Welcome, {result.Username}");
            if (usernameRegister != null) usernameRegister.text = "";
            if (emailRegister != null) emailRegister.text = "";
            if (passwordRegister != null) passwordRegister.text = "";
            UpdateRegisterButtonState();
        },
        error =>
        {
            MostrarError("Error registering: " + error.ErrorMessage);
            UpdateRegisterButtonState();
        });
    }

    #endregion Register

    #region Error Handling
    private void MostrarError(string mensaje)
    {
        if (messageError != null)
        {
            messageError.text = mensaje;
            messageError.color = Color.red;
        }
        else
        {
            if (loginFeedback != null) loginFeedback.text = mensaje;
            if (registerFeedback != null) registerFeedback.text = mensaje;
        }
    }

    private void LimpiarError()
    {
        if (messageError != null) messageError.text = "";
    }

    #endregion Error Handling

    #region Feedback
    private void SetLoginFeedback(string message)
    {
        if (loginFeedback != null)
        {
            loginFeedback.text = message;
            loginFeedback.transform.parent.gameObject.SetActive(true);
        }
    }

    private void SetRegisterFeedback(string message)
    {
        if (registerFeedback != null)
        {
            registerFeedback.text = message;
            registerFeedback.transform.parent.gameObject.SetActive(true);
        }
    }
    #endregion Feedback
}