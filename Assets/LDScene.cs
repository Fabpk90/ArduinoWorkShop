using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;

[CreateAssetMenu(fileName = "LD", menuName = "Atlas/LD", order = 0)]
public class LDScene : ScriptableObject
{
	public bool[] deadListA;
	public bool[] deadListB;


	[ContextMenu("CopyScene")]
	public void CopyScene()
	{
		deadListA = new bool[Point.points.Count];
		deadListB = new bool[Point.points.Count];
		for (int i = 0; i < Point.points.Count; i++)
		{
			deadListA[i] = Point.points[i].isDeadA;
			deadListB[i] = Point.points[i].isDeadB;
		}
	}


	[ContextMenu("ApplyScene")]
	public void ApplyToScene(){
		if (deadListA.Length < Point.points.Count) return;

		for (int i = 0; i < Point.points.Count; i++)
		{
			Point.points[i].isDeadA = deadListA[i];
			Point.points[i].isDeadB = deadListB[i];

		}
	}
}



