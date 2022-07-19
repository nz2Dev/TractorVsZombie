public static class Utils {
    
    public static float Map(float value, float startA, float stopA, float startB, float stopB) {
        return startB + (stopB - startB) * ((value - startA) / (stopA - startA));
    }

}