using System.Collections.Generic;

public class LoaderModel {
    public List<int> PlatformIds { get; } = new();
    public Queue<LoadoutPrototype> PendingLoadouts { get; } = new();
}
