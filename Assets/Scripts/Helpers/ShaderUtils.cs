using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderUtils {

    ///
    /// Will instantiate materil with the shader if it's not already instatiated
    /// While in edit mode, will use sharedMaterial property of renderer
    ///
    public static void ExecutionDependentInstantiateMaterial(this Renderer renderer, Shader shader) {
        var currrentMaterial = ExecutionDependentObtainMaterial(renderer);
        if (currrentMaterial != null && currrentMaterial.shader == shader) {
            return;
        }

        Object.Destroy(currrentMaterial);
        var material = new Material(shader);
        if (Application.isPlaying) {
            renderer.material = material;
        } else {
            renderer.sharedMaterial = material;
        }
    }

    public static Material ExecutionDependentObtainMaterial(this Renderer renderer) {
        if (Application.isPlaying) {
            return renderer.material;
        } else {
            return renderer.sharedMaterial;
        }
    }

}
