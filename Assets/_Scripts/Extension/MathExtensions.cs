using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtensions
{
    public static int AwayFromZero(this float f)
    {
		decimal decimals = Math.Truncate((decimal)f);
		Debug.Log("dec: " + decimals);

		// Start is called before the first frame update
		if (f < 0)
		{
			return Mathf.FloorToInt(f);
		}
		else
		{
			return Mathf.CeilToInt(f);
		}
	}
}
