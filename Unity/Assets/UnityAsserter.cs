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
	}

	public void IsEqual(double expected, double actual, double delta)
	{
		if (Math.Abs(expected - actual) < delta)
			Debug.Log("Expected " + expected + "+- " + delta + ", but was " + actual);
	}
}