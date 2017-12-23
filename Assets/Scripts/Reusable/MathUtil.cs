using UnityEngine;
using System.Collections;

/// <summary>
/// Math Util class
/// Version 1.4
/// </summary>
public static class MathUtil{
	/// <summary>
	/// Summation operation.
	/// </summary>
	public static int Summation(int number){
		return (number*(number+1))/2;
	}

	/// <summary>
	/// Fatorial operation.
	/// </summary>
	public static int Factorial(int number){
		return number<=1 ? number : Factorial(number-1)*number;
	}

	public static Vector2 Abs(Vector2 vector){
		return new Vector2(Mathf.Abs(vector.x),Mathf.Abs(vector.y));
	}

	public static Vector3 Abs(Vector3 vector){
		return new Vector3(Mathf.Abs(vector.x),Mathf.Abs(vector.y),Mathf.Abs(vector.z));
	}
	
	#region Repeat
    /// <summary>
	/// Repeat for int where t is always subtracted/sum of length until it is 0 <= t < length.
	/// </summary>
	/// <example>
	/// Repeat(15,6) = 3, Repeat(12,3) = 0, Repeat(-2,3) = 1.
	/// </example>
    public static int Repeat(int t, int length) {
        while (t < 0)
            t += length;
        while (t >= length)
            t -= length;
        return t;
    }

    /// <summary>
    /// Repeat who accept range.
    /// </summary>
    public static int Repeat(int t, int min, int max) {
        return Repeat(t-min, max-min+1)+min;
    }

    /// <summary>
    /// Repeat who accept range.
    /// </summary>
    public static float Repeat(float t, float min, float max) {
        return Mathf.Repeat(t - min, max - min + 1) + min;
    }
	#endregion

	/// <summary>
	/// Like floor, but instead of clamping into integer (1), you can choose the number. Not 100% accurate.
	/// </summary>
	/// <example>
	/// SuperFloor(3,1.2) = 2.4, SuperFloor(4.6,1.5) = 4.5.
	/// </example>
	public static float Floor(float number, float unitLimit){
		return Mathf.Floor(number/unitLimit)*unitLimit;
	}

	/// <summary>
	/// Like Floor, but with Round.
	/// </summary>
	/// <example>
	/// SuperEound(3,1.1) = 3.3.
	/// </example>
	public static float Round(float number, float unitLimit){
		return Mathf.Round(number/unitLimit)*unitLimit;
	}

	/// <summary>
	/// Convert Unity Angle to Math angle and vice-versa.
	/// 0 is right on Math Angle. 0 is up on unity angle.
	/// 90 is up on Math Angle. 90 is right on unity angle.
	/// </summary>
	/// <returns>The angle to math angle.</returns>
	public static float ConvertUnityMathAngle(float unityAngle){
		float mathAngle = -unityAngle+90;
		return WrapAngle(mathAngle);
	}

	/// <summary>
	/// WrapAngle between 0 and 360.
	/// </summary>
	/// <returns>The angle.</returns>
	public static float WrapAngle (float angle){
		while (angle > 360f) angle -= 360f;
		while (angle < 0f) angle += 360f;
		return angle;
	}

	/// <summary>
	/// WrapAngle between -180 and 180.
	/// </summary>
	/// <returns>The angle.</returns>
	public static float WrapAngle180 (float angle){
		while (angle > 180f) angle -= 360f;
		while (angle < -180f) angle += 360f;
		return angle;
	}

	#region GetAngle
	/// <summary>
	///	Returns the angle in Unity degrees based in two positions. 0-360
	/// </summary>
	/// <returns>The angle.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public static float GetAngle(float x, float y){
		return WrapAngle(-Mathf.Atan2(y,x) * Mathf.Rad2Deg+90);
	}

	public static float GetAngle(float x, float y, float targetX, float targetY){
		return GetAngle(targetX-x,targetY-y);
	}

	public static float GetAngle(Vector2 positions){
		return GetAngle(positions.x,positions.y);
	}

	public static float GetAngle(Vector2 positions, Vector2 target){
		return GetAngle(positions.x,positions.y,target.x,target.y);
	}
	#endregion

	/// <summary>
	/// Cast Unity angle into a normalized Vector2
	/// </summary>
	/// <returns>The angle to normal.</returns>
	public static Vector2 UnityAngleToNormal(float unityAngle){
		float radians = ConvertUnityMathAngle(unityAngle)* Mathf.Deg2Rad;
		Vector2 ret = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
		return ret;
	}

	/// <summary>
	/// Cast X value of a Normalized Vector into Unity angle.
	/// </summary>
	public static float XnormalToUnityAngle(float xNormalized){
		return ConvertUnityMathAngle(Mathf.Acos(xNormalized)*Mathf.Rad2Deg);
	}

	/// <summary>
	/// Cast Y value of a Normalized Vector into Unity angle.
	/// </summary>
	public static float YnormalToUnityAngle(float yNormalized){
		return ConvertUnityMathAngle(Mathf.Asin(yNormalized)*Mathf.Rad2Deg);
	}

	#region VectorChangedByUnityAngle
	/// <summary>
	/// Based on a local position and a Unity angle, returns the equivalent position if the position is rotated by this angle.
	/// </summary>
	/// <returns>Rotated position.</returns>
	public static Vector2 VectorChangedByUnityAngle(Vector2 position, float unityAngle){
		return VectorChangedByUnityAngle(position.magnitude,unityAngle);
	}

	public static Vector2 VectorChangedByUnityAngle(float positionX, float positionY, float unityAngle){
		return VectorChangedByUnityAngle(new Vector2(positionX,positionY),unityAngle);
	}

	/// <summary>
	/// Based on a foward Z position and a Unity angle, returns the equivalent position if the position is rotated by this angle.
	/// </summary>
	/// <returns>Rotated position.</returns>
	public static Vector2 VectorChangedByUnityAngle(float positionZ, float unityAngle){
		Vector2 normal = UnityAngleToNormal(unityAngle);
		return new Vector2(normal.x,normal.y)*positionZ;
	}
	#endregion

	public static Vector3 QuadraticBezier(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, float time){
		time = Mathf.Clamp01(time);
		float curveX = (((1-time)*(1-time)) * startPoint.x) + (2 * time * (1 - time) * controlPoint.x) + ((time * time) * endPoint.x);
		float curveY = (((1-time)*(1-time)) * startPoint.y) + (2 * time * (1 - time) * controlPoint.y) + ((time * time) * endPoint.y);
		return new Vector3(curveX, curveY, 0);
	}
}