using System;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class SelectionOptionDisplay {

    public enum AccentState {
        None,
        Selected,
        Active
    }

    private const string SelectedBorderClass = "selected-border";
    private const string ActiveBorderClass = "active-border";
    private const string DottedBorderClass = "dotted-border";
    private const string ActiveImageClass = "active-image";

    public VisualElement Root { get; }

    private readonly VisualElement border;
    private readonly Image image;
    private readonly Label label;

    public SelectionOptionDisplay(VisualTreeAsset template) {
        if (template == null)
            throw new ArgumentNullException(nameof(template));

        Root = template.Instantiate();
        border = Root.Q<VisualElement>("border");
        image = Root.Q<Image>("image");
        label = Root.Q<Label>("label");

        if (border == null)
            throw new InvalidOperationException("The selection item template needs an element with class 'border'.");
        if (image == null)
            throw new InvalidOperationException("The selection item template needs an Image named 'image'.");

        SetAccentState(AccentState.None);
    }

    internal void SetAmount(int amount) {
        label.text = $"{amount}";
    }

    public void SetImage(Sprite sprite) {
        image.sprite = sprite;
    }

    public void SetAccentState(AccentState state) {
        border.RemoveFromClassList(DottedBorderClass);
        border.RemoveFromClassList(ActiveBorderClass);
        border.RemoveFromClassList(SelectedBorderClass);
        image.RemoveFromClassList(ActiveImageClass);
        switch (state) {
            case AccentState.None:
                border.AddToClassList(DottedBorderClass);
                break;
            case AccentState.Selected:
                border.AddToClassList(SelectedBorderClass);
                break;
            case AccentState.Active:
                border.AddToClassList(SelectedBorderClass);
                border.AddToClassList(ActiveBorderClass);
                image.AddToClassList(ActiveImageClass);
                break;
        }
    }

}
