using System;

namespace StackKata {

    public class Stack {
        
        private int counter = 0;
        private int[] elements = new int[2];

        public bool IsEmpty() {
            return counter == 0;
        }

        public int Pop() {
            if (IsEmpty()) {
                throw new UnderflowException();
            } else {
                return elements[--counter];
            }
        }

        public void Push(int element) {
            elements[counter++] = element;
        }

        public class UnderflowException : Exception {
        }
    }
}
