using UnityEngine;
using System;

/// <summary>
/// Score list. Override it child (like ScoreListInt) using a custom class. 
/// Basically you only need to override KeyLabel property in this Custom class. <para>&#160;</para><para>&#160;</para>
///
/// <c>Important Methods:</c> <c>Add(value)</c> to add a new value into a list, <c>Save()</c> to save the list and <c>Load()</c> to load the list.
/// The methods are saved using PlayerPref. So, if you want to write data avoiding changes, use PlayerPrefs.Save() after call.
/// Version 4.0
/// </summary>
public abstract class ScoreList<T> where T : IComparable, new(){
	public T[] values{protected set; get;}

	public virtual int Size{
		get{
			return 10;
		}
	}

	public virtual bool Asc{
		get{
			return false;
		}
	}

	/// <summary>
	/// The score default value. Also, works as limit value.
	/// </summary>
	protected abstract T DefaultValue{get;}

	/// <summary>
	/// The score key. Must be an unique string.
	/// </summary>
	/// <value>The key label.</value>
	protected abstract string KeyLabel{get;}

	public ScoreList(){
		Clear();
	}

	public void Clear(){
		values = new T[Size];
		for(int i = 0;i<Size;i++){
			values[i] = DefaultValue;
		}
	}

	/// <summary>
	/// Return the values[index] formatted as string.
	/// </summary>
	public string GetString(int index){
		return GetStringAsValue(values[index]); 
	}

	/// <summary>
	/// Return the value formatted as string. Can be used to format values that aren't in values array.
	/// </summary>
	public virtual string GetStringAsValue(T value){
		return value.ToString(); 
	}

	/// <summary>
	/// Add a new score and returns if the score is enough to be added.
	/// </summary>
	public virtual bool AddScore(T newScore){
		bool somethingChanged = false;
		T nextScore = Asc ^ DefaultValue.CompareTo(newScore) < 0 ? newScore : DefaultValue;
		for(int i = 0;i<Size;i++){
			if (Asc ^ nextScore.CompareTo(values[i]) > 0){
				somethingChanged = true;
				T aux = values[i];
				values[i] = nextScore;
				nextScore = aux;
			}
		}		
		return somethingChanged;
	}

	/// <summary>
	/// Save the current score list.
	/// </summary>
	public virtual void Save(){
		for(int i = 0;i<Size;i++)
			SaveSingleValue(KeyLabel+"-"+i,values[i]);
		PlayerPrefs.SetInt(KeyLabel,1);
	}

	/// <summary>
	/// Saves a single value.
	/// </summary>
	/// <param name="key">Key used on PlayerPrefs.</param>
	/// <param name="val">Value to be saved.</param>
	protected abstract void SaveSingleValue(string key, T val);

	/// <summary>
	/// Load a ScoreList.
	/// </summary>
	/// Return if the registry exists.
	public virtual bool Load(){
		bool registryFound = PlayerPrefs.GetInt(KeyLabel,0)==1;
		if(!registryFound)
			return false;
		for(int i = 0;i<Size;i++)
			values[i]=LoadSingleValue(KeyLabel+"-"+i);
		return true;
	}

	/// <summary>
	/// Loads a single value.
	/// </summary>
	/// <returns>The value loaded.</returns>
	/// <param name="key">Key used on PlayerPrefs.</param>
	protected abstract T LoadSingleValue(string key);
}