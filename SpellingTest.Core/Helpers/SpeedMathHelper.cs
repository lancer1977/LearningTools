using SpellingTest.Core.ViewModels.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellingTest.Core.Helpers;

public static class SpeedMathHelper
{
    public static (int, int) GetMinMax(Difficulty difficulty, Feature feature)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                switch (feature)
                {
                    case Feature.Add: return (0, 9);
                    case Feature.Subtract: return (0, 9);
                    case Feature.Divide: return (0, 9);
                    case Feature.Multiply: return (0, 9);
                    default: throw new ArgumentOutOfRangeException(nameof(feature), feature, null);
                }
            case Difficulty.Medium:
                switch (feature)
                {
                    case Feature.Add: return (0, 10);
                    case Feature.Subtract: return (0, 10);
                    case Feature.Divide: return (0, 10);
                    case Feature.Multiply: return (0, 10);
                    default: throw new ArgumentOutOfRangeException(nameof(feature), feature, null);
                }
            case Difficulty.Hard:
                switch (feature)
                {
                    case Feature.Add: return (-10, 20);
                    case Feature.Subtract: return (-10, 20);
                    case Feature.Divide: return (-10, 20);
                    case Feature.Multiply: return (-10, 20);
                    default: throw new ArgumentOutOfRangeException(nameof(feature), feature, null);
                }
                break;
            case Difficulty.Extreme:
                switch (feature)
                {
                    case Feature.Add: return (-100, 100);
                    case Feature.Subtract: return (-100, 100);
                    case Feature.Divide: return (-100, 100);
                    case Feature.Multiply: return (-100, 100);
                    default: throw new ArgumentOutOfRangeException(nameof(feature), feature, null);
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
        }
    }

    private static int GetAnswer(Feature feature, int n1, int n2)
    {
        switch (feature)
        {
            case Feature.Multiply:
                return n1 * n2;
            case Feature.Divide:
                return n1 / n2;
            case Feature.Add:
                return n1 + n2;
            case Feature.Subtract:
                return n1 - n2;
        }

        return 0;
    }
    public static int GetLower(Difficulty dif)
    {
        switch (dif)
        {
            case Difficulty.Easy:
                return 0;
            case Difficulty.Medium:
                return 5;
            case Difficulty.Hard:
                return 10;
            case Difficulty.Extreme:
                return 15;
            default:
                throw new ArgumentOutOfRangeException(nameof(dif), dif, null);
        }
    }
    public static int GetUpper(Difficulty dif)
    {
        switch (dif)
        {
            case Difficulty.Easy:
                return 10;
            case Difficulty.Medium:
                return 15;
            case Difficulty.Hard:
                return 20;
            case Difficulty.Extreme:
                return 25;
            default:
                throw new ArgumentOutOfRangeException(nameof(dif), dif, null);
        }
    }
    public static List<MathQuestion> PopulateQuestions(Feature feature, int lower, int upper)
    {
        var numberRange = Enumerable.Range(lower, upper).ToArray();
        var questions = new List<MathQuestion>();

        foreach (var first in numberRange)
        {
            foreach (var second in numberRange)
            {
                if (feature == Feature.Divide)
                {
                    if (first == 0) continue;
                    questions.Add(new MathQuestion()
                    {
                        Number1 = GetAnswer(Feature.Multiply, first, second),

                        Number2 = first,
                        Feature = feature,
                        Answer = second
                    });
                }
                else
                {
                    questions.Add(new MathQuestion()
                    {
                        Number1 = first,
                        Number2 = second,
                        Feature = feature,
                        Answer = GetAnswer(feature, first, second)
                    });
                }

            }
        }

        return questions;
    }

}