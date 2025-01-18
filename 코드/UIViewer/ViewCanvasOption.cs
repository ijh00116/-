using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewCanvasOption : ViewCanvas
    {
        public Slider bgmValue;
        public Slider effectValue;

        public Toggle damageAppearToggle;
        public GameObject damageAppearEnableObj;

        public Toggle effectAppearToggle;
        public GameObject effectAppearEnableObj;

        public Toggle autoSaveBatteryToggle;
        public GameObject autoSaveBatteryEnableObj;

        public Toggle bossAutoChallengeToggle;
        public GameObject bossautoEnableObj;

        public Toggle camShakeToggle;
        public GameObject camShakeEnableObj;

        public Toggle pushAlarm;
        public GameObject pushAlarmObj;
        public Toggle nightpushAlarm;
        public GameObject nightpushAlarmObj;

        public BTButton termsofServiceBtn;
        public BTButton privatePolicyBtn;
        public BTButton sendASBtn;
        public BTButton infoSaveBtn;
        public BTButton GoogleLoginBtn;
        public BTButton AppleLoginBtn;
        public GameObject alreadyGoogleLoginedCheck;
        public GameObject alreadyAppleLoginedCheck;

        public GameObject SocialConfirmWindow;
        public BTButton SocialConfirmRestartBtn;
        public GameObject SocialExistWindow;
        public BTButton SocialExistConfirmBtn;
        public BTButton[] SocialExistOffBtn;

        public TMP_Text uuidText;
        public BTButton idcopyBtn;

        public BTButton popupDeleteAccount;
        public GameObject deleteAcountWindow;
        public BTButton[] closeDeleteAccount;
        public BTButton confirmDeleteAccount;

        public TMP_Text deleteAccountText_0;
        public TMP_Text deleteAccountText_1;
        public TMP_Text deleteAccountDesc_1;
        public TMP_Text deleteAccountConfirmTxt;
        public TMP_Text deleteAccountCancelTxt;

        public BTButton[] closeButton;

        [Header("localizing")]
        public TMP_Text settingTitle;
        public TMP_Text gameSetting;
        public TMP_Text effectSound;
        public TMP_Text bgmSound;
        public TMP_Text dmgAppear;
        public TMP_Text effectAppear;
        public TMP_Text AutoSaveMode;
        public TMP_Text AutoBoss;
        public TMP_Text cameraEffect;
        public TMP_Text pushAlarmTxt;
        public TMP_Text pushAlarmNightTxt;
        public TMP_Text EtcOptionTxt;
        public TMP_Text TermsTxt;
        public TMP_Text PrivacyTxt;
        public TMP_Text QATxt;
        public TMP_Text GameSaveTxt;
        public TMP_Text GoogleLoginTxt;
        public TMP_Text AppleLoginTxt;
        public TMP_Text IdCopy;

        public BTButton restorePurchase;
        public TMP_Text restoreTxt;

    }

}
