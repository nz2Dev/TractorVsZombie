using System.Reflection;

using UnityEngine;

public static class NonNullValidator {

    public static bool ValidateScene() {
        bool valid = true;

        foreach (MonoBehaviour component in Object.FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (!ValidateObject(component))
                valid = false;

        return valid;
    }

    private static bool ValidateObject(Object target) {
        bool valid = true;

        System.Type type = target.GetType();

        foreach (FieldInfo field in type.GetFields(
                     BindingFlags.Instance |
                     BindingFlags.Public |
                     BindingFlags.NonPublic)) {
            if (!field.IsDefined(typeof(NonNullAttribute), true))
                continue;

            if (!typeof(Object).IsAssignableFrom(field.FieldType))
                continue;

            Object value = field.GetValue(target) as Object;

            if (value != null)
                continue;

            Debug.LogError(
                $"[NonNull] Field '{field.Name}' on " +
                $"'{target.name}' ({type.Name}) is null. " +
                $"Assign a value before entering Play Mode.",
                target);

            valid = false;
        }

        return valid;
    }
}