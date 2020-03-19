//gpu端公用的数学函数

#define G 9.8
#define SQRT_2 1.414213562373095
#define PI 3.1415926

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

//bitreverse计算蝶形lut的另一个index
uint CalBitReverse(uint inter)
{
    //最多到1024
    int list[10];
    int i=0;
    for(;; ++i)
    {
        if(inter == 0 )
        {
            break;
        }
        uint cur = inter & 0x01;
        list[i] = cur;
        inter = inter >> 1;
    }
    
    uint res = 0;
    for(int j = 0; j<i; ++j)
    {
        res += list[j];
        res >> 1; 
    }
    return res;
}
