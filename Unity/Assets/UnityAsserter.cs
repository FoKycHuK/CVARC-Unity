using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class UnityAsserter : IAsserter
{
	public void IsEqual(bool expected, bool actual)
	{
		
		if (expected != actual)
			Debug.Log("Expected " + expected + ", but was " + actual);
		else
			Debug.Log("OK");
	}

	public void IsEqual(double expected, double actual, double delta)
	{
		var difference = Math.Abs(expected - actual);
		if (difference > delta)
			Debug.Log("Expected " + expected + "+- " + delta + ", but was " + actual);

	}
}