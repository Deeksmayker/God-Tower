using UnityEngine;
using NTC.Global.Cache;

public class PlayerBall : MonoCache{
	private bool _activated;

	public bool SetActivated(bool isIt) => _activated = isIt;

	public bool IsActivated() => _activated;
}
