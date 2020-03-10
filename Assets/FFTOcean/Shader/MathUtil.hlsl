//gpu端公用的数学函数

#define G 9.8
#define SQRT_2 1.414213562373095

//共轭复数
float2 CalComplexConjugate(float2 plu)
{
    return float2(plu.x, -plu.y);
}

//计算向量的模
float CalMagnitude(float2 vec)
{
    return sqrt(vec.x * vec.x + vec.y + vec.y);
}


//欧拉恒等式转实部虚部
float2 TransferEulerIndentityToComplexConjugate(float exp)
{
    return float2(cos(exp), sin(exp));
}

