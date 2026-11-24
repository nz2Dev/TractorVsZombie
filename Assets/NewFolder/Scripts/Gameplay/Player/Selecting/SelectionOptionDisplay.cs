using System;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class SelectionOptionDisplay {

    private const string SelectedClass = "selected-border";
    private const string ActiveBorderClass = "active-border";
    private const string ActiveImageClass = "active-image";

    public VisualElement Root { get; }

    private readonly VisualElement border;
    private readonly Image image;
    private readonly Label label;

    public SelectionOptionDisplay(VisualTreeAsset template) {
        if (template == null)
            throw new ArgumentNullException(nameof(template));

        Root = template.Instantiate();
        border = Root.Q<VisualElement>(className: "border");
        image = Root.Q<Image>("image");
        label = Root.Q<Label>("label");

        if (border == null)
            throw new InvalidOperationException("The selection item template needs an element with class 'border'.");
        if (image == null)
            throw new InvalidOperationException("The selection item template needs an Image named 'image'.");

        // These classes are present in the template for its default preview,
        // but represent runtime state for a displayed option.
        SetSelected(false);
        SetActive(false);
    }

    internal void SetAmount(int amount) {
        label.text = $"{amount}";
    }

    public void SetImage(Texture texture) {
        image.image = texture;
    }

    public void SetSelected(bool selected) {
        border.EnableInClassList(SelectedClass, selected);
    }

    public void SetActive(bool active) {
        border.EnableInClassList(ActiveBorderClass, active);
        image.EnableInClassList(ActiveImageClass, active);
    }

}
