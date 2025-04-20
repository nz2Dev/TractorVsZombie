using System;
using System.Collections.Generic;

public class PrimeFactors {
    public static List<int> Generate(int n) {
        var primeNumbers = new List<int>();
        
        for (int candidate = 2; n > 1; candidate++)
            for (; n % candidate == 0; n /= candidate) 
                primeNumbers.Add(candidate);

        return primeNumbers;
    }
}
