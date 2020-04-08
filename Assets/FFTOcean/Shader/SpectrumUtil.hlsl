//计算频谱相关的头文件

#include "MathUtil.hlsl"

//计算PhillipsSpectrum
/*
    cor : 采样坐标
    a :
    w : 风向
    v : 风速

    return : PhillipsSpectrum
*/
float CalPhillipsModule(float2 cor, float a, float2 w, float v)
{
    float k_mag = CalMagnitude(cor);
    if (k_mag < 0.001f)
    {
        return 0;
    }
    float s = v * v / G;
    float abs_dot = (cor.x * w.x + cor.y * w.y) / k_mag;
    float k_square = k_mag * k_mag;
    float k_forth_power = k_square * k_square;
    /*float damping = 0.001f;
    float l2 = s * s * damping * damping;*/
    return a * exp(-1 * 1.0f / (k_square * s * s)) / k_forth_power * abs_dot * abs_dot;// * exp(-k_square * l2);
}

//计算频谱的系数
float2 CalFModule(float2 rand_pair, float2 cor, float a, float2 w, float v)
{
    float phillips = CalPhillipsModule(cor, a, w, v);
    float res = 1 / SQRT_2 * sqrt(phillips);

    return float2(rand_pair.x * res, rand_pair.y * res);
}

float2 CalIndex(int2 cor, int size, float length)
{
    float2 res;
    res.x = 2 * PI * ((int)cor.x - size / 2) / length;
    res.y = 2 * PI * ((int)cor.y - size / 2) / length;
    return res;
}

//计算最终的频谱高度
float4 CalPhillipsSpectrum(uint2 cor, float t, float2 rand_pair0, float2 rand_pair1, float a, float2 w, float v, int size, float length)
{
    int2 real_cor = int2(cor.x, cor.y);
    float2 k = CalIndex(real_cor, size, length);
    float2 n_k = CalIndex(-real_cor, size, length);
    float exp = sqrt(G * CalMagnitude(k)) * t;
    float2 f0 = CalFModule(rand_pair0, k, a, w, v);
    float2 f1 = CalFModule(rand_pair1, n_k, a, w, v);
    f1 = CalComplexConjugate(f1);
    float2 f0_eular = TransferEulerIndentityToComplexConjugate(exp);
    float2 f1_eular = TransferEulerIndentityToComplexConjugate(-exp);
    float2 f0_mul = MulComplex(f0, f0_eular);
    float2 f1_mul = MulComplex(f1, f1_eular);
    float real = f0_mul.x + f1_mul.x;
    float imag = f0_mul.y + f1_mul.y;
    return float4(real, imag, w.x, 0);
}


