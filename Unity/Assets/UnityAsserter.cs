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
			Debugger.Log(DebuggerMessageType.Unity,"Expected " + expected + ", but was " + actual);
		else
			Debugger.Log(DebuggerMessageType.Unity,"OK");
	}

	public void IsEqual(double expected, double actual, double delta)
	{
		var difference = Math.Abs(expected - actual);
		if (difference > delta)
			Debugger.Log(DebuggerMessageType.Unity,"Expected " + expected + "+- " + delta + ", but was " + actual);

	}
}