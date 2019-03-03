using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {

	// Use this for initialization
	public Text debugText;
	public static DebugText DebuggerText;

	public void Awake()
	{
		DebuggerText = this;
	}
	public void setDebugText(string inputString)
	{
		debugText.text = inputString;
	}
}
