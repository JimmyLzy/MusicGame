﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock {

	public int seconds = 0;
	public int minutes = 0;
	public int hours = 0;
	private const int MAX_VALUE = 60;

	public void increaseTimeBySeconds(int seconds) {
		this.seconds += seconds;
		if (this.seconds >= MAX_VALUE) {
			minutes++;
			this.seconds -= MAX_VALUE;
		} 
		if (minutes >= MAX_VALUE) {
			hours++;
			minutes -= MAX_VALUE;
		}
	}

	public int CalcTotalTime() {
		return hours * MAX_VALUE * MAX_VALUE + minutes * MAX_VALUE + seconds;
	}


	public void Reset() {
		seconds = 0;
		minutes = 0;
		hours = 0;
	}

	public override string ToString () {
		string result = "";
		if (hours < 10) {
			result += "0" + hours + ":";
		} else {
			result += hours + ":";
		}
		if (minutes < 10) {
			result += "0" + minutes + ":";
		} else {
			result += minutes + ":";
		}

		if (seconds < 10) {
			result += "0" + seconds;
		} else {
			result += seconds;
		}
		return result;
	}
}
