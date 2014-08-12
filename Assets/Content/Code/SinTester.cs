using UnityEngine;
using System.Collections;

public class SinTester : MonoBehaviour 
{

	public Vector3 CentrePoint = Vector3.zero;
	public float Magnitude = 1.0f;
	public float MaxDegrees = 360f;
	public float Speed = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(CentrePoint, Magnitude);

		float currentDegree = Mathf.PingPong(Time.realtimeSinceStartup * Speed, MaxDegrees);

		Vector3 angleDirection = Quaternion.AngleAxis(Mathf.Sin(Mathf.Deg2Rad * currentDegree) * Mathf.Rad2Deg, Vector3.forward) * Vector3.right;

		Gizmos.DrawLine(CentrePoint, angleDirection * Magnitude);
	}

}
