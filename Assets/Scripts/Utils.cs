public static class Utils {
        
    public static float Remap(float value, float startA, float startB, float endA, float endB) {
        var t = (value - startA) / startB;
        return endA + (endB - endA) * t;
    }
    
}