using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using viol3.SuiWorks.Accounts;
using viol3.SuiWorks.Transactions;

namespace viol3.SuiWorks.UI
{
    public class SuiAccountUI : MonoBehaviour
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
        [SerializeField] private RectTransform _loginPanel;
        [SerializeField] private RectTransform _loggingPanel;
        [SerializeField] private RectTransform _directWalletPanel;
        [SerializeField] private RectTransform _userPanel;
        [Space]
        [SerializeField] private TMP_InputField _privateKeyInput;
        [Space]
        [SerializeField] private Image _loadingIcon;
        [SerializeField] private Image _checkIcon;
        [SerializeField] private Button _cancelLoggingButton;
        [SerializeField] private TMP_Text _loggingText;
        [SerializeField] private TMP_Text _loggedText;
        private async void Start()
        {
            SuiAccountManager.Initialize("testnet", _googleClientId, _enokiPublicKey, _redirectUri);
            await SuiAccountManager.Instance.Validate();
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
            ActivateLoggingPanel();
            bool result = await SuiAccountManager.Instance.LoginWithGoogle();
            if (result) 
            {
                SetLoggedPanel();
                await Task.Delay(2000);
                ActivateUserPanel();
            }
            else
            {
                ActivateLoginPanel();
            }
        }

        public void OnDirectWalletButtonClick()
        {
            DisableAllPanels();
            _directWalletPanel.gameObject.SetActive(true);
        }

        public void OnGenerateWalletButtonClick()
        {
            _privateKeyInput.text = SuiAccountManager.Instance.GeneratePrivateKey();
        }

        public async void OnImportWalletButtonClick()
        {
            string privateKeyHex = _privateKeyInput.text;
            if(SuiAccountManager.Instance.IsPrivateKeyValid(privateKeyHex))
            {
                SuiAccountManager.Instance.LoginWithLocal(privateKeyHex);
                ActivateLoggingPanel();
                await Task.Delay(500);
                SetLoggedPanel();
                await Task.Delay(2000);
                ActivateUserPanel();
            }
            else
            {
                MessageBox.Instance.Show("Private key is not valid. You need to put in hex format");
            }
        }

        public void OnDirectWalletCancelButtonClick()
        {
            DisableAllPanels();
            _loginPanel.gameObject.SetActive(true);
        }

        public void OnCopyClipboardPrivateKeyClicked()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLCopyAndPaste.WebGLCopyAndPasteAPI.CopyToClipboard(_privateKeyInput.text);
#else
            GUIUtility.systemCopyBuffer = _privateKeyInput.text;
#endif

        }

        public void OnLogoutButtonClick()
        {
            SuiAccountManager.Instance.Logout();
            ActivateLoginPanel();
        }

        public void OnPlayButtonClick()
        {
            _fadeImage.gameObject.SetActive(false);
            DisableAllPanels();
        }

        public void Show()
        {
            _fadeImage.gameObject.SetActive(true);
            ActivateUserPanel();
        }

        void SetupUserInfo()
        {
            string suiAddress = SuiAccountManager.Instance.GetSuiAddress();
            _userAddressFullText.text = suiAddress;
            _userAddressText.text = suiAddress.Substring(0, 5) + ".." + suiAddress.Substring(suiAddress.Length - 4, 3);
            _userNameText.text = SodaNameGenerator.GenerateNickname(suiAddress);
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

