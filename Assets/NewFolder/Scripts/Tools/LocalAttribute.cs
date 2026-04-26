using UnityEngine;

/// <summary>
/// Mark a GameObject or Component field as [Local] to ensure it references an object 
/// that is either the same GameObject or a child of the GameObject the script is attached to.
/// </summary>
public class LocalAttribute : PropertyAttribute {
}
