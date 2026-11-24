using System.Collections.Generic;

public class SelectingModel {
    public List<SelectionOption> Options { get; } = new();
    public List<SelectionOption> SelectedOptions { get; } = new();
    public List<WeaponCategory> Categories { get; } = new ();
    public List<WeaponCategory> SelectedCategories { get;} = new ();
}
