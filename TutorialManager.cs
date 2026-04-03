using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviourSingleton<TutorialManager>
{
    public enum TutorialType
    {
        BuyShelf,
        OrderProducts,
        PlaceProductOnShelf,
        OpenShop,
        None
    }

    public static event Action OnTutorialUpdated;

    public TutorialBase CurrentTutorial { get; private set; }

    [SerializeField] private TutorialType[] _startingTutorialTypes;

    private readonly Dictionary<TutorialType, string> _tutorialTypeStringKeys = new Dictionary<TutorialType, string>() {
        {TutorialType.BuyShelf, "tutorial_complete_buy_shelf" },
        {TutorialType.OrderProducts, "tutorial_complete_order_products" },
        {TutorialType.PlaceProductOnShelf, "tutorial_complete_place_products_on_shelf" },
        {TutorialType.OpenShop, "tutorial_complete_open_shop" },
    };

    private Dictionary<TutorialType, bool> _isTutorialFinished;
    private TutorialBase[] _tutorials;

    protected override void Awake()
    {
        base.Awake();
        _tutorials = GetComponentsInChildren<TutorialBase>();
    }

    private void Start()
    {
        SavingManager.Instance.LoadingCompleted += LoadSavedData;
        SavingManager.Instance.SavingStarted += SaveTutorialData;
        SavingManager.Instance.OnSceneCleanup += OnSceneCleanup;
    }

    private void SaveTutorialData()
    {
        foreach (var tutorialType in _tutorialTypeStringKeys.Keys) {
            SavingManager.Instance.SetBool(_tutorialTypeStringKeys[tutorialType], _isTutorialFinished[tutorialType]);
        }
        if(CurrentTutorial != null) {
            SavingManager.Instance.SetInt(_tutorialTypeStringKeys[CurrentTutorial.TutorialType], CurrentTutorial.CurrentStepIndex);
        }
    }

    private void LoadSavedData()
    {
        _isTutorialFinished = new();
        foreach (var tutorialType in _tutorialTypeStringKeys.Keys) {
            _isTutorialFinished[tutorialType] = SavingManager.Instance.GetBool(_tutorialTypeStringKeys[tutorialType], false);
        }
        StartTutorial();
    }

    private void StartTutorial()
    {
        CurrentTutorial = null;
        foreach (var tutorialType in _startingTutorialTypes) {
            if (!_isTutorialFinished[tutorialType]) {
                StartTutorialByType(tutorialType);
                return;
            }
        }
        OnTutorialUpdated?.Invoke();
    }

    private void StartTutorialByType(TutorialType tutorialType)
    {
        foreach (var tutorial in _tutorials) {
            if(tutorial.TutorialType == tutorialType) {
                CurrentTutorial = tutorial;
                CurrentTutorial.TutorialCompleted += OnCurrentTutorialCompleted;
                CurrentTutorial.TutorialStateUpdated += OnCurrentTutorialUpdated;
                tutorial.StartTutorial(SavingManager.Instance.GetInt(_tutorialTypeStringKeys[tutorialType], 0));
                return;
            }
        }
        Debug.LogError("Tutorial: " + tutorialType + " - NOT FOUND!");
    }

    private void OnCurrentTutorialUpdated()
    {
        OnTutorialUpdated?.Invoke();
    }

    private void OnCurrentTutorialCompleted()
    {
        _isTutorialFinished[CurrentTutorial.TutorialType] = true;
        CurrentTutorial.TutorialCompleted -= OnCurrentTutorialCompleted;
        CurrentTutorial.TutorialStateUpdated -= OnCurrentTutorialUpdated;
        StartTutorial();
    }

    private void OnSceneCleanup()
    {
        if(CurrentTutorial != null) {
            CurrentTutorial.TutorialCompleted -= OnCurrentTutorialCompleted;
            CurrentTutorial.TutorialStateUpdated -= OnCurrentTutorialUpdated;
        }
    }
}
