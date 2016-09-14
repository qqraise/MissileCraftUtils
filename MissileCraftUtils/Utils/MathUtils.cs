using UnityEngine;
using System.Collections;
using System;

public class MathUtils{
    /// <summary>
    /// 2*PI的值
    /// </summary>
    public const float PI_2 = 2.0f * Mathf.PI;
    public const float FloatHalfMin = float.MinValue / 2.0f;
    public const float FloatHalfMax = float.MaxValue / 2.0f;

    /// <summary>
    /// 将较大的整数转化为，用K，M，G表示（1000进制），保留1位小数
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string IntToShortString(int num)
    {
        string sign = "";
        if (num < 0)
            sign = "-";
        num = Mathf.Abs(num);
        if (num < 1000)
            return num + "";
        else if (num >= 1000 && num < 1000000)
            return sign + (num / 1000f).ToString("0.0") + "K";
        else if (num >= 1000000 && num < 1000000000)
            return sign +  (num / 1000000f).ToString("0.0") + "M";
        else
            return sign +  (num / 1000000000f).ToString("0.0") + "G";
    }

    /// <summary>
    /// 无符号取模运算 x%y，x为任意数， y为正整数
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y">y为正整数</param>
    /// <returns>x%y</returns>
    public static int UnsignedMod(int x, int y)
    {
        int num = x % y;
        if (num < 0)
        {
            num += y;
        }
        return num;
    }
}

public class NormalDistribution
{
    /// <summary>
    /// σ,正态分布的标准差 
    /// standard deviation
    /// </summary>
    public float StandardDeviation { get; private set; }

    /// <summary>
    /// σ^2,正态分布的方差 
    /// </summary>
    public float Variance { get { return StandardDeviation * StandardDeviation; } }

    /// <summary>
    /// μ，期望/均值
    /// mean or expectation
    /// </summary>
    public float Mean { get; private set; }

    /// <summary>
    /// 建立期望喂0的标准正态分布
    /// </summary>
    /// <param name="standardDeviation"></param>
    public NormalDistribution(float standardDeviation)
    {
        this.Mean = 0;
        this.StandardDeviation = standardDeviation;
    }

    public NormalDistribution(float mean, float standardDeviation)
    {
        this.Mean = mean;
        this.StandardDeviation = standardDeviation;
    }

    public float GetValue(float input)
    {
        return Mathf.Exp(-Mathf.Pow(input - Mean, 2) / (2 * Variance)) / (Mathf.Sqrt(2 * Mathf.PI) * StandardDeviation);
    }


    static float u, v;
    static bool genrate = true;
    /// <summary>
    /// 采用Box-Muller生成服从N(0,1)的正态分布随机数
    /// </summary>
    /// <returns></returns>
    public static float Random()
    {
        genrate = !genrate;
        if (genrate)
            return Mathf.Sqrt(-2 * Mathf.Log10(u)) * Mathf.Sin(MathUtils.PI_2 * v);
        u = UnityEngine.Random.value;
        v = UnityEngine.Random.value;
        return Mathf.Sqrt(-2 * Mathf.Log10(u)) * Mathf.Cos(MathUtils.PI_2 * v);
    }

    /// <summary>
    /// 采用Box-Muller生成服从N(0,1)的正态分布随机数,并将值归一到（-clampThreshold，clampThreshold）区间中
    /// </summary>
    /// <param name="clampThreshold">归一的阈值范围，由于是N(0,1)，所以为正数</param>
    /// <returns></returns>
    public static float RandomN(float clampThreshold)
    {
        float value = Random() ;
        float norm = value / clampThreshold;
        return Mathf.Clamp(norm, -clampThreshold, clampThreshold);
        
    }
}

[Serializable]
public struct Range
{
    public float min;
    public float max;

    public Range(float min, float max) { this.min = min;this.max = max; }
}



