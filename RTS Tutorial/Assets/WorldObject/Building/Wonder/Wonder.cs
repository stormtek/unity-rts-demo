using UnityEngine;
using System.Collections;

public class Wonder : Building {
	protected override bool ShouldMakeDecision () {
		return false;
	}
}