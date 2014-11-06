using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(NewSinTest))]
public class NewSinTestEditor : Editor
{
	public NewSinTest Target
	{
		get
		{
			return (target as NewSinTest);
		}
	}

	public override void OnInspectorGUI()
	{
		SceneView.RepaintAll ();

		if (GUILayout.Button ("Draw Line")) 
		{
			DrawLine( target as NewSinTest, (target as NewSinTest).LineSegments );
		}

		base.OnInspectorGUI ();
	}

	public static void DrawLine(NewSinTest eo, int segments)
	{
		float segmentLength = eo.LineLength / (float)segments;

		mPoints = new Vector3[segments + 1];

		Vector3 previousPoint = eo.Origin - (new Vector3 (eo.LineLength * 0.5f, 0f, 0f));
		mPoints [0] = previousPoint;

		for (int i = 1; i < segments; i++) 
		{
			float sin = Mathf.Sin (( Time.realtimeSinceStartup + i) * eo.Frequency + eo.CurrentPhase) * eo.Amplitude;

			Vector3 correctedPoint = previousPoint;
			correctedPoint.y = 0;

			Vector3 point = correctedPoint + (Vector3.right * (( segmentLength)));

			point.y += sin;

			mPoints[i] = point;
			previousPoint = point;
		}

		mPoints [mPoints.Length - 1] = eo.Origin + new Vector3 (eo.LineLength * 0.5f, 0f, 0f);

	}

	public void OnSceneGUI()
	{
		Repaint ();
	}

	private static Vector3[] mPoints;

	[DrawGizmo (GizmoType.NotSelected | GizmoType.Selected | GizmoType.Pickable)]
	static void DrawGizmos(NewSinTest eo, GizmoType gizmoType)
	{
		if (eo.Frequency != eo.PreviousFrequency) 
		{
			CalculateNewFreq(eo);
		}

		eo.PreviousFrequency = eo.Frequency;

		Handles.DrawWireDisc (eo.Origin, Vector3.forward, eo.Radius);

		Vector3 point = Vector3.right * eo.Radius;
		
		float sin = Mathf.Sin (Time.realtimeSinceStartup * eo.Frequency + eo.CurrentPhase) * eo.Amplitude;

		Handles.Label (Vector3.right * (eo.Radius * 1.1f), sin.ToString ());

		point = Quaternion.AngleAxis(sin * eo.MaxDegrees, Vector3.forward) * point;
		
		Handles.DrawLine (eo.Origin, point);

		DrawLine (eo, eo.LineSegments);

		if (mPoints != null && mPoints.Length > 0) 
		{
			for(int i = 1; i < mPoints.Length; i++)
			{
				Handles.color = Color.magenta;
				Handles.DrawLine(mPoints[i - 1], mPoints[i]);
				Handles.color = Color.yellow;
				//Handles.DrawSolidDisc(mPoints[i - 1], Vector3.forward, 0.25f);
				Handles.color = Color.white;
			}

			Handles.color = Color.yellow;
			//Handles.DrawSolidDisc(mPoints[mPoints.Length - 1], Vector3.forward, 0.25f);
			Handles.color = Color.white;

			Handles.color = Color.cyan;
			Handles.DrawLine(mPoints[0], mPoints[mPoints.Length - 1]);
		}

//		float cos = Mathf.Cos (Mathf.PingPong (Time.realtimeSinceStartup * (eo.CosFrequency * Time.deltaTime), eo.Frequency));
//
//		point = Vector3.right * ((eo.Amplitude * (cos + 1)));
//
//		point = Quaternion.AngleAxis(sin * eo.MaxDegrees, Vector3.forward) * point;
//
//		Handles.color = Color.magenta;
//		Handles.DrawLine (eo.Origin, point);
//
//		Handles.Label (Vector3.right * (eo.Amplitude * 2f), cos.ToString ());
//
//		Handles.color = Color.white;
	}

	private static void CalculateNewFreq(NewSinTest eo)
	{
		float curr = (Time.time * eo.PreviousFrequency + eo.CurrentPhase) % (2.0f * Mathf.PI);
		float next = (Time.time * eo.Frequency) % (2.0f * Mathf.PI);
		eo.CurrentPhase = curr - next;
		eo.PreviousFrequency = eo.Frequency;
	}

}
