//cpu端公用的数学公式
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//正态分布的随机数对
class MathUtil
{
    static public Vector2 CalGaussianRandomVariablePair() 
	{
		float x1, x2, w;
		do {
			x1 = 2f * UnityEngine.Random.Range(0f,1f) - 1f;
			x2 = 2f * UnityEngine.Random.Range(0f,1f) - 1f;
			w = x1 * x1 + x2 * x2;
		} while ( w >= 1f );
		w = Mathf.Sqrt((-2f * Mathf.Log(w)) / w);
		return new Vector2(x1 * w, x2 * w);
	}

	public static uint CalBitReverse(uint inter, uint length)
	{
		//最多到1024
		uint[] list = new uint[10];
		uint i=0;
		for(; i < length; ++i)
		{
			uint cur = inter & 0x01;
			list[i] = cur;
			inter = inter >> 1;
		}
		
		uint res = 0;
		for(uint j = 0; j<i; ++j)
		{
			res += list[j];
			if((i-1) == j)
			{
				break;
			}
			res = res << 1; 
		}
		return res;
	}
}

