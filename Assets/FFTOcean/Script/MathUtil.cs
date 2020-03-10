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

	uint CalBitReverse(uint inter, int stage_count)
	{
		List<uint> list = new List<uint>();
		for(int i = 0 ; i < stage_count; ++i)
		{
			if(inter == 0 )
			{
				break;
			}
			uint cur = inter & 0x01;
			list.Add(cur);
			inter = inter >> 1;
		}
		list.Reverse();
		var re_itr = list.GetEnumerator();
		uint res = 0;
		while(re_itr.MoveNext())
		{
			res = res << 1;
			res += re_itr.Current;
		}
		return res;
	}
}

//bitreverse算法
