using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Allow Invoke and InvokeRepeat with Yield instruction.
/// </summary>
public static class InvokeExtension{
	/// <summary>
	/// Invoke with a yield instruction and an Action.
	/// </summary>
	/// <example>
	/// <code>this.Invoke(new WaitForSeconds(1f), () => Debug.Log("Do action here"))</code>
	/// </example>
	/// <returns>
	/// Coroutine played.
	/// </returns>
    public static Coroutine Invoke(this MonoBehaviour monoBehaviour, YieldInstruction yieldInstruction, Action action){      
		return monoBehaviour.StartCoroutine(Coroutine(yieldInstruction, action));
	}

	public static Coroutine Invoke(this MonoBehaviour monoBehaviour, CustomYieldInstruction yieldInstruction, Action action){      
		return monoBehaviour.StartCoroutine(Coroutine(yieldInstruction, action));
	}

	static IEnumerator Coroutine(YieldInstruction yieldInstruction, Action action){
		yield return yieldInstruction;
		action.Invoke();
	}

	static IEnumerator Coroutine(CustomYieldInstruction yieldInstruction, Action action){
		yield return yieldInstruction;
		action.Invoke();
	}

	/// <summary>
	/// Invoke with a yield instruction, an Action and repeating after every new yield instruction.
	/// </summary>
	/// <example>
	/// <code>this.Invoke(new WaitForSeconds(1f), () => Debug.Log("Do action here"), new WaitForSeconds(.2f))</code>
	/// </example>
	/// <returns>
	/// Coroutine played.
	/// </returns>
	public static Coroutine InvokeRepeating(
		this MonoBehaviour monoBehaviour, YieldInstruction yieldInstruction, Action action, YieldInstruction repeatYieldInstruction
	){      
		return monoBehaviour.StartCoroutine(Coroutine(yieldInstruction, action, repeatYieldInstruction));
	}

	public static Coroutine InvokeRepeating(
		this MonoBehaviour monoBehaviour, CustomYieldInstruction yieldInstruction, Action action, CustomYieldInstruction repeatYieldInstruction
	){      
		return monoBehaviour.StartCoroutine(Coroutine(yieldInstruction, action, repeatYieldInstruction));
	}

	static IEnumerator Coroutine(YieldInstruction yieldInstruction, Action action, YieldInstruction repeatYieldInstruction){
        yield return yieldInstruction;
		while(true){
			action.Invoke();
			yield return repeatYieldInstruction;
		}
	}

	static IEnumerator Coroutine(CustomYieldInstruction yieldInstruction, Action action, CustomYieldInstruction repeatYieldInstruction){
		yield return yieldInstruction;
		while(true){
			action.Invoke();
			yield return repeatYieldInstruction;
		}
	}
}