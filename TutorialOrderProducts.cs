using DG.Tweening;
using System;
using UnityEngine;

public class TutorialOrderProducts : TutorialBase
{
    public override TutorialManager.TutorialType TutorialType => TutorialManager.TutorialType.OrderProducts;
    public override string TutorialTitleTranslationKey => "tutorial_title_order_products";

    [SerializeField] private GameObject _warehouseCamera;
    private readonly float CAMERA_SHOW_DURATION = 3;

    protected override TutorialDescriptionData[] StepsDescriptionsData => new TutorialDescriptionData[] {
        new("tutorial_desc_open_orders_menu", InputIcons.Instance.GetInputIcon(PlayerInputHandler.Instance.BaseInputActions.OpenOrdersUI)),
        new("tutorial_desc_make_order"),
        new("tutorial_desc_open_map", InputIcons.Instance.GetInputIcon(PlayerInputHandler.Instance.BaseInputActions.OpenMapUI)),
        new("tutorial_desc_drive_to_warehouse"),
        new("tutorial_desc_pack_order_to_car"),
        new("tutorial_desc_go_back_to_your_shop")
    };

    protected override Action[] StartStepDelegates => new Action[]{
        () => StartOpenWindowTutorial(UIManager.ordersUI),
        () => OrdersManager.OrderPlaced += OnPlayerOrder,
        () => StartOpenWindowTutorial(UIManager.mapUI),
        () => StartGoToLocationTutorial(LocationsManager.Location.Warehouse),
        StartPlaceContainerInCarTutorial,
        () => StartGoToLocationTutorial(LocationsManager.Location.YourShop),
    };

    private void OnPlayerOrder(int _)
    {
        OrdersManager.OrderPlaced -= OnPlayerOrder;
        CompleteCurrentStep();
    }

    private void OnContainerPlacedInCar(Container _)
    {
        Car.OnContainerPlacedInCar -= OnContainerPlacedInCar;
        CompleteCurrentStep();
    }

    private void StartPlaceContainerInCarTutorial()
    {
        _warehouseCamera.SetActive(true);
        DOVirtual.DelayedCall(CAMERA_SHOW_DURATION, () => _warehouseCamera.SetActive(false));
        Car.OnContainerPlacedInCar += OnContainerPlacedInCar;
    }

    protected override void OnSceneCleanup()
    {
        base.OnSceneCleanup();
        Car.OnContainerPlacedInCar -= OnContainerPlacedInCar;
        OrdersManager.OrderPlaced -= OnPlayerOrder;
    }
}
