using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class Point : MonoBehaviour
{
	public static List<Point> points = new List<Point>();
	public List<Point> connected = new List<Point>();
	public PointManager manager;
	private void OnEnable()
	{
		points.Add(this);
	}

	private void OnDisable()
	{
		points.Remove(this);

		
	}

	private void Update()
	{
		Connection();
		if(Input.anyKeyDown){
			string str = name+";";
			foreach (Point p in connected)
			{
				str += p.transform.name.Replace('(', ' ').Replace(')', ' ').Trim() + ";";
			}

			str.Remove(str.Length - 1);
			Debug.Log(str);
		}
	}
	private void Connection(){
		connected.Clear();
		foreach (Point p in points)
		{
			if (CheckDistance(p))
			{
				connected.Add(p);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 0.2f);

		foreach (Point p in connected)
		{
				Debug.DrawLine(transform.position, p.transform.position);
		}

	}

	private bool CheckDistance(Point p){
		float d = Vector3.Distance(transform.position, p.transform.position);
		return d <= manager.cableLength && p != this;
	}
}
