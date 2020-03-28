using System.Diagnostics;
using UnityEngine.Assertions;

public static class AssertHelper
{
    [Conditional("UNITY_ASSERTIONS")]
    public static void NotNegative(int n)
    {
        Assert.IsFalse(n < 0);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void NotNegative(params int[] numbers)
    {
        for (int i = 0; i < numbers.Length; i++) Assert.IsFalse(numbers[i] < 0);
    }
}