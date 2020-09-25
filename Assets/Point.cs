using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
	public static List<Point> points = new List<Point>();
	public List<Point> connected = new List<Point>();
	public PointManager manager;
	public bool isDeadA = false;
	public bool isDeadB = false;

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
			//allCities.Add(new City(new List<int>() { 1 }, "CityA", 0));
			string str = "allCities.Add(new City(new List<int>() {";
			foreach (Point p in connected)
			{
				str += p.transform.name +",";
			}
			str = str.Remove(str.Length - 1);
			str += "}, \"City_" + name + "\", "+name+"));";
			
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
		Gizmos.color = Color.green;
		if (IsDead()){
			Gizmos.color = isDeadA ? Color.cyan : Color.yellow; 
		}
		
		Gizmos.DrawSphere(transform.position, 0.2f);

		foreach (Point p in connected)
		{
				Debug.DrawLine(transform.position, p.transform.position);
		}

	}

	private bool IsDead(){
		return isDeadA || isDeadB;
	}

	private bool CheckDistance(Point p){
		float d = Vector3.Distance(transform.position, p.transform.position);
		return d <= manager.cableLength && p != this && !p.IsDead() && !IsDead();
	}
}
