using System;
using System.Collections.Generic;

using NUnit;
using NUnit.Framework;

public class PrimeNumbersTest {

    private List<int> CreateList(params int[] array) {
        var listOfNumber =  new List<int>();
        for (int i = 0; i < array.Length; i++) {
            listOfNumber.Add(array[i]);
        }
        return listOfNumber;
    }

    [Test]
    public void TestOne() {
        Assert.That(CreateList(), Is.EqualTo(PrimeFactors.Generate(1)));
    }

    [Test]
    public void TestTwo() {
        Assert.That(CreateList(2), Is.EqualTo(PrimeFactors.Generate(2)));
    }

    [Test]
    public void TestThree() {
        Assert.That(CreateList(3), Is.EqualTo(PrimeFactors.Generate(3)));
    }

    [Test]
    public void TestFour() {
        Assert.That(CreateList(2, 2), Is.EqualTo(PrimeFactors.Generate(4)));
    }

    [Test]
    public void TestSix() {
        Assert.That(CreateList(2, 3), Is.EqualTo(PrimeFactors.Generate(6)));
    }

    [Test]
    public void TestEight() {
        Assert.That(CreateList(2, 2, 2), Is.EqualTo(PrimeFactors.Generate(8)));
    }

    [Test]
    public void TestNine() {
        Assert.That(CreateList(3, 3), Is.EqualTo(PrimeFactors.Generate(9)));
    }

}
