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
    float s = v * v / G;
    float abs_dot = abs(cor * w);
    float k_mag = CalMagnitude(cor);

    float k_square = k_mag * k_mag;
    float k_forth_power = k_square * k_square;

    return a * exp(-1 / (k_square * s * s)) / k_forth_power * abs_dot;
}

//计算频谱的系数
float2 CalFModule(float2 rand_pair, float2 cor, float a, float2 w, float v)
{
    float phillips = CalPhillipsModule(cor, a, w, v);
    float res = 1 / SQRT_2 * phillips;

    return float2(rand_pair.x * res, rand_pair.y * res);
}

//计算最终的频谱高度
float2 CalPhillipsSpectrum(float2 cor, float t, float2 rand_pair, float a, float2 w, float v)
{
    float exp = sqrt(G * CalMagnitude(cor)) * t;
    float2 f0 = CalFModule(rand_pair, cor, a, w, v);
    float2 f1 = CalFModule(rand_pair, -cor, a, w, v);
    float2 f0_eular = TransferEulerIndentityToComplexConjugate(exp);
    float2 f1_eular = TransferEulerIndentityToComplexConjugate(-exp);

    float real = f0.x * f0_eular.x - f0.y * f0_eular.y 
                + f1.x * f1_eular.x - f1.y * f1_eular.y;
    float imag = f0.x * f0_eular.y + f0.y * f0_eular.x 
                + f1.x * f1_eular.y + f1.y * f1_eular.x;
    return float2(real, imag);
}


