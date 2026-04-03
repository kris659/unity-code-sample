using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private Transform _listParent;
    [SerializeField] private GameObject _listElementPrefab;

    [SerializeField] private BasicLocalizedText _titleLocalizedText;

    [SerializeField] private Color _normalTextColor;
    [SerializeField] private Color _completedTextColor;

    private GameObject _uiGameObject;
    private CanvasGroup _canvasGroup;
    private bool _isOpen = false;

    private readonly int TUTORIAL_STEPS_PER_PAGE = 3;
    private readonly float FADE_DURATION = 1;

    private void Awake()
    {
        _uiGameObject = transform.GetChild(0).gameObject;
        _canvasGroup = _uiGameObject.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _uiGameObject.SetActive(false);
        _listElementPrefab.SetActive(false);
    }

    private void Start()
    {
        // No need to unsubscribe from these events since this object will exist for the entire game session
        TutorialManager.OnTutorialUpdated += UpdateUI;
        UIManager.SettingsUI.Closed += UpdateUI;
        PlayerInputHandler.Instance.OnCurrentDeviceChanged += UpdateUI;
        MainMenu.Instance.OnMainMenuOpen += HideUI;
    }

    private void UpdateUI()
    {
        TutorialBase currentTutorial = TutorialManager.Instance.CurrentTutorial;
        if (currentTutorial == null || currentTutorial.StepsCount == 0) {
            HideUI();
            return;
        }

        OpenUI();

        _listParent.DestroyAllChildren(includeInactive: false); // Extension method
        _titleLocalizedText.SetLocalizationKey(currentTutorial.TutorialTitleTranslationKey);

        int completedPages = currentTutorial.CurrentStepIndex / TUTORIAL_STEPS_PER_PAGE;

        for (int i = 0; i < TUTORIAL_STEPS_PER_PAGE; i++) {
            if (completedPages * TUTORIAL_STEPS_PER_PAGE + i >= currentTutorial.StepsCount)
                break;

            int index = completedPages * TUTORIAL_STEPS_PER_PAGE + i;
            GameObject element = Instantiate(_listElementPrefab, _listParent);
            element.SetActive(true);

            TMP_Text text = element.GetComponentInChildren<TMP_Text>();
            LocalizedTextWithVariables localizedText = text.GetComponent<LocalizedTextWithVariables>();
            Image[] icons = element.GetComponentsInChildren<Image>();

            bool isComplete = currentTutorial.CurrentStepIndex > index;

            icons[0].gameObject.SetActive(true);
            icons[1].gameObject.SetActive(isComplete);

            text.spriteAsset = InputIcons.Instance.CurrentSpriteAsset;
            TutorialBase.TutorialDescriptionData descriptionData = currentTutorial.GetStepDescription(index);
            localizedText.SetLocalizationKey(descriptionData.DescriptionTranslationKey);
            localizedText.SetTextVariables(descriptionData.DescriptionTranslationVariables);
            text.color = isComplete ? _completedTextColor : _normalTextColor;
        }
    }

    private void OpenUI()
    {
        if (_isOpen)
            return;
        _isOpen = true;
        _canvasGroup.DOKill();
        _uiGameObject.SetActive(true);
        _canvasGroup.DOFade(1, FADE_DURATION).SetEase(Ease.OutSine);
    }

    private void HideUI()
    {
        if (!_isOpen)
            return;
        _isOpen = false;
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(0, FADE_DURATION).SetEase(Ease.InSine).onComplete += () => _uiGameObject.SetActive(false);
    }
}
