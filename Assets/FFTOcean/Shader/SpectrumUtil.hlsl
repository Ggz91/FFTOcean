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
    float abs_dot = abs(cor.x * w.x + cor.y * w.y);
    float k_square = k_mag * k_mag;
    float k_forth_power = k_square * k_square;
    float damping = 0.001f;
    float l2 = s * s * damping * damping;
    return a * exp(-1 * 1.0f / (k_square * s * s)) / k_forth_power * abs_dot * abs_dot * exp(-k_square * l2);
}

//计算频谱的系数
float2 CalFModule(float2 rand_pair, float2 cor, float a, float2 w, float v)
{
    float phillips = CalPhillipsModule(cor, a, w, v);
    float res = 1 / SQRT_2 * sqrt(phillips);

    return float2(rand_pair.x * res, rand_pair.y * res);
}

//计算最终的频谱高度
float2 CalPhillipsSpectrum(float2 cor, float t, float2 rand_pair0, float2 rand_pair1, float a, float2 w, float v)
{
    float exp = sqrt(G * CalMagnitude(cor)) * t;
    float2 f0 = CalFModule(rand_pair0, cor, a, w, v);
    float2 f1 = CalFModule(rand_pair1, -cor, a, w, v);
    f1 = CalComplexConjugate(f1);
    float2 f0_eular = TransferEulerIndentityToComplexConjugate(exp);
    float2 f1_eular = TransferEulerIndentityToComplexConjugate(-exp);
    float2 f0_mul = MulComplex(f0, f0_eular);
    float2 f1_mul = MulComplex(f1, f1_eular);
    float real = f0_mul.x + f1_mul.x;
    float imag = f0_mul.y + f1_mul.y;
    return float2(real, imag);
}


