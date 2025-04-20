using System;

public static class Wrapper {

    public static string Wrap(string content, int column) {
        var charArray = content.ToCharArray();
        int lastLineEndIndex = 0;
        int wordStartIndex = 0;
        int wordEndIndex = -1;
        for (int i = 0; i < charArray.Length; i++) {
            var currentChar = charArray[i];
            if (wordEndIndex < wordStartIndex && (currentChar == ' ' || i == charArray.Length - 1)) {
                wordEndIndex = i;

                var wordEndIndexInLine = wordEndIndex - lastLineEndIndex;
                if (wordEndIndexInLine > column) {
                    charArray[wordStartIndex - 1] = '\n';
                    lastLineEndIndex = wordStartIndex; 
                }
                
                continue;
            }

            if (wordStartIndex < wordEndIndex && currentChar != ' ') {
                wordStartIndex = i;

                var worldStartIndexInLine =  wordStartIndex - lastLineEndIndex;
                if (worldStartIndexInLine > column) {
                    charArray[wordStartIndex - 1] = '\n';
                    lastLineEndIndex = wordStartIndex; 
                }

                continue;
            } 
        }

        return new string(charArray);
    }
}
