using System;
using UnityEngine;

public abstract class TutorialBase: MonoBehaviour
{
    public struct TutorialDescriptionData
    {
        public string DescriptionTranslationKey;
        public string[] DescriptionTranslationVariables;

        public TutorialDescriptionData(string descriptionTranslationKey, params string[] descriptionTranslationVariables)
        {
            DescriptionTranslationKey = descriptionTranslationKey; 
            DescriptionTranslationVariables = descriptionTranslationVariables;
        }
    }

    public event Action TutorialStateUpdated;
    public event Action TutorialCompleted;

    public abstract TutorialManager.TutorialType TutorialType { get; }
    public abstract string TutorialTitleTranslationKey { get; }
    public int StepsCount => StepsDescriptionsData.Length;

    public int CurrentStepIndex { get; private set; }
    public TutorialDescriptionData GetStepDescription(int stepIndex) => StepsDescriptionsData[stepIndex];

    protected abstract TutorialDescriptionData[] StepsDescriptionsData { get; }
    protected abstract Action[] StartStepDelegates { get; }

    private WindowUI _currentTutorialWindow;
    private LocationsManager.Location _currentTutorialLocation;

    public void StartTutorial(int startingIndex)
    {
        SavingManager.Instance.OnSceneCleanup += OnSceneCleanup;
        CurrentStepIndex = startingIndex;
        UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        TutorialStateUpdated?.Invoke();
        if (CurrentStepIndex >= StepsCount) {
            SavingManager.Instance.OnSceneCleanup -= OnSceneCleanup;
            TutorialCompleted?.Invoke();
            return;
        }
        StartStepDelegates[CurrentStepIndex].Invoke();
    }

    protected void CompleteCurrentStep()
    {
        CurrentStepIndex++;
        UpdateTutorial();
    }

    protected void StartOpenWindowTutorial(WindowUI window)
    {
        window.Opened += OnWindowOpen;
        _currentTutorialWindow = window;
    }

    private void OnWindowOpen()
    {
        _currentTutorialWindow.Opened -= OnWindowOpen;
        _currentTutorialWindow = null;
        CompleteCurrentStep();
    }

    protected void StartGoToLocationTutorial(LocationsManager.Location location)
    {
        if (LocationsManager.Instance.IsPlayerInLocation(location)) {
            CompleteCurrentStep();
            return;
        }
        _currentTutorialLocation = location;
        LocationsManager.Instance.OnLocationEntered += OnLocationEntered;
    }

    private void OnLocationEntered(LocationsManager.Location location)
    {
        if (_currentTutorialLocation != location)
            return;
        LocationsManager.Instance.OnLocationEntered -= OnLocationEntered;
        CompleteCurrentStep();
    }

    protected virtual void OnSceneCleanup()
    {
        if(_currentTutorialWindow != null)
            _currentTutorialWindow.Opened -= OnWindowOpen;
        LocationsManager.Instance.OnLocationEntered -= OnLocationEntered;
    }
}
