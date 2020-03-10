//cpu端公用的数学公式
using System.Collections;
using UnityEngine;

//正态分布的随机数对
class MathUtil
{
    static public Vector2 CalGaussianRandomVariablePair() 
	{
		float x1, x2, w;
		do {
			x1 = 2f * Random.Range(0f,1f) - 1f;
			x2 = 2f * Random.Range(0f,1f) - 1f;
			w = x1 * x1 + x2 * x2;
		} while ( w >= 1f );
		w = Mathf.Sqrt((-2f * Mathf.Log(w)) / w);
		return new Vector2(x1 * w, x2 * w);
	}
}
