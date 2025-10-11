using Ali.Helper;
using Ali.Helper.Audio;
using DG.Tweening;
using NBitcoin;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using viol3.SuiWorks.Accounts;
using viol3.SuiWorks.Transactions;

namespace viol3.SuiWorks.UI
{
    public class SuiAccountUI : LocalSingleton<SuiAccountUI>
    {
        [SerializeField] private string _enokiPublicKey;
        [SerializeField] private string _redirectUri;
        [SerializeField] private string _googleClientId;
        [Space]
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _userAddressText;
        [SerializeField] private TMP_Text _userAddressFullText;
        [SerializeField] private Image _userImage;
        [Space]
        [SerializeField] private Image _fadeImage;
        [SerializeField] private RectTransform _initPanel;
        [SerializeField] private RectTransform _loginPanel;
        [SerializeField] private RectTransform _loggingPanel;
        [SerializeField] private RectTransform _directWalletPanel;
        [SerializeField] private RectTransform _userPanel;
        [Space]
        [SerializeField] private Button _loginCopyButton;
        [Space]
        [SerializeField] private TMP_InputField _privateKeyInput;
        [Space]
        [SerializeField] private Image _loadingIcon;
        [SerializeField] private Image _checkIcon;
        [SerializeField] private Button _cancelLoggingButton;
        [SerializeField] private TMP_Text _loggingText;
        [SerializeField] private TMP_Text _loggedText;

        public UnityEvent OnInitialized = new UnityEvent();
        public UnityEvent OnLoginStarted = new UnityEvent();
        public UnityEvent<bool> OnLoginEnded = new UnityEvent<bool>();
        public UnityEvent OnDirectWalletButtonClicked = new UnityEvent();
        public UnityEvent OnGenerateWalletButtonClicked = new UnityEvent();
        public UnityEvent OnDirectWalletCancelButtonClicked = new UnityEvent();
        public UnityEvent OnCopyClipboardPrivateKeyClicked = new UnityEvent();
        public UnityEvent OnLogoutButtonClicked = new UnityEvent();
        public UnityEvent OnPlayButtonClicked = new UnityEvent();

        private CancellationTokenSource _cancellationTokenSource;

        private async void Start()
        {
            SuiAccountManager.Initialize("testnet", _googleClientId, _enokiPublicKey, _redirectUri);
            await SuiAccountManager.Instance.Validate();
            OnInitialized?.Invoke();
            _initPanel.gameObject.SetActive(false);
            SuiAccountManager.Instance.LoadTransactionKit(new TransactionKit());
            if (!SuiAccountManager.Instance.IsLoggedIn())
            {
                ActivateLoginPanel();
            }
            else
            {
                ActivateUserPanel();
            }
        }

        public async void OnGoogleLoginButtonClick()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            ActivateLoggingPanel();
            OnLoginStarted.Invoke();
            bool result = await SuiAccountManager.Instance.LoginWithGoogle(_cancellationTokenSource.Token);
            
            if (result) 
            {
                StartCoroutine(LoginCompleteProcess());
            }
            else
            {
                ActivateLoginPanel();
                OnLoginEnded.Invoke(false);
            }
        }

        IEnumerator LoginCompleteProcess()
        {
            SetLoggedPanel();
            OnLoginEnded.Invoke(true);
            yield return new WaitForSeconds(2f);
            ActivateUserPanel();
        }

        public void OnDirectWalletButtonClick()
        {
            DisableAllPanels();
            _directWalletPanel.gameObject.SetActive(true);
            OnDirectWalletButtonClicked.Invoke();
        }

        public void OnGenerateWalletButtonClick()
        {
            _privateKeyInput.text = SuiAccountManager.Instance.GeneratePrivateKey();
            OnGenerateWalletButtonClicked.Invoke();
        }

        public void OnImportWalletButtonClick()
        {
            string privateKeyHex = _privateKeyInput.text;
            OnLoginStarted?.Invoke();
            if (SuiAccountManager.Instance.IsPrivateKeyValid(privateKeyHex))
            {
                SuiAccountManager.Instance.LoginWithLocal(privateKeyHex);
                ActivateLoggingPanel();
                StartCoroutine(LoginCompleteProcess());
            }
            else
            {
                MessageBox.Instance.Show("Private key is not valid. You need to put in hex format");
                OnLoginEnded?.Invoke(false);
            }
            
        }

        public void OnDirectWalletCancelButtonClick()
        {
            DisableAllPanels();
            _loginPanel.gameObject.SetActive(true);
            OnDirectWalletCancelButtonClicked.Invoke();
        }

        public void OnCopyClipboardPrivateKeyClick()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLCopyAndPaste.WebGLCopyAndPasteAPI.CopyToClipboard(_privateKeyInput.text);
#else
            GUIUtility.systemCopyBuffer = _privateKeyInput.text;
#endif
            OnCopyClipboardPrivateKeyClicked.Invoke();
        }

        public void OnLoginCopyClipboardPrivateKeyClick()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLCopyAndPaste.WebGLCopyAndPasteAPI.CopyToClipboard(SuiAccountManager.Instance.GetPrivateKey());
#else
            GUIUtility.systemCopyBuffer = SuiAccountManager.Instance.GetPrivateKey();
#endif
            OnCopyClipboardPrivateKeyClicked.Invoke();
        }

        public void OnLogoutButtonClick()
        {
            SuiAccountManager.Instance.Logout();
            ActivateLoginPanel();
            OnLogoutButtonClicked?.Invoke();
        }

        public void OnCancelLoginButtonClick()
        {
            _cancellationTokenSource.Cancel();
        }

        public void OnPlayButtonClick()
        {
            _fadeImage.gameObject.SetActive(false);
            DisableAllPanels();
            OnPlayButtonClicked.Invoke();
        }

        public void Show()
        {
            _fadeImage.gameObject.SetActive(true);
            ActivateUserPanel();
        }

        public bool IsUIActive()
        {
            return _fadeImage.gameObject.activeSelf;
        }

        void SetupUserInfo()
        {
            string suiAddress = SuiAccountManager.Instance.GetSuiAddress();
            _userAddressFullText.text = suiAddress;
            _userAddressText.text = suiAddress.Substring(0, 5) + ".." + suiAddress.Substring(suiAddress.Length - 4, 3);
            _userNameText.text = SodaNameGenerator.GenerateNickname(suiAddress);
            _loginCopyButton.gameObject.SetActive(!SuiAccountManager.Instance.IsEnokiZKLogin());
        }

        void ResetLoggingPanel()
        {
            _loadingIcon.gameObject.SetActive(true);
            _loggingText.gameObject.SetActive(true);
            _cancelLoggingButton.gameObject.SetActive(true);
            _checkIcon.gameObject.SetActive(false);
            _loggedText.gameObject.SetActive(false);
        }

        void SetLoggedPanel()
        {
            _loadingIcon.gameObject.SetActive(false);
            _loggingText.gameObject.SetActive(false);
            _cancelLoggingButton.gameObject.SetActive(false);
            _checkIcon.gameObject.SetActive(true);
            _loggedText.gameObject.SetActive(true);
            _checkIcon.transform.DOKill(true);
            _checkIcon.transform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 6);
        }

        void ActivateLoggingPanel()
        {
            DisableAllPanels();
            ResetLoggingPanel();
            _loggingPanel.gameObject.SetActive(true);
        }

        void ActivateLoginPanel()
        {
            DisableAllPanels();
            _loginPanel.gameObject.SetActive(true);
        }

        void ActivateUserPanel()
        {
            DisableAllPanels();
            _userPanel.gameObject.SetActive(true);
            SetupUserInfo();
        }

        void DisableAllPanels()
        {
            _loginPanel.gameObject.SetActive(false);
            _loggingPanel.gameObject.SetActive(false);
            _directWalletPanel.gameObject.SetActive(false);
            _userPanel.gameObject.SetActive(false);
        }
    }
}

