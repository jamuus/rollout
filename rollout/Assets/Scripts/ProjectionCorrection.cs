using UnityEngine;
using System.Collections;
using System;

public class ProjectionCorrection : MonoBehaviour {
	public float squash = 1f;
	public float width = 1f;
	public float size = 1f;
	public double theta = 0f;

	void Start() {
		Camera cam = Camera.main;
		Matrix4x4 m = PerspectiveOffCenter();
		cam.projectionMatrix = m;
	}

	Matrix4x4 PerspectiveOffCenter() {
		Matrix4x4 m = new Matrix4x4();
		Matrix4x4 rotation = new Matrix4x4();
		m[0, 0] = size * width;
		m[0, 1] = 0f;
		m[0, 2] = 0f;
		m[0, 3] = 0f;
		m[1, 0] = 0f;
		m[1, 1] = size;
		m[1, 2] = 0;
		m[1, 3] = 0;
		m[2, 0] = 0f;
		m[2, 1] = 0;
		m[2, 2] = size;
		m[2, 3] = -1f;
		m[3, 0] = 0;
		m[3, 1] = 1f;
		m[3, 2] = squash;
		m[3, 3] = 1f;
		m = m * Camera.main.projectionMatrix;

		rotation[0, 0] = 1;
		rotation[0, 1] = 0;
		rotation[0, 2] = 0;
		rotation[0, 3] = 0;
		rotation[1, 0] = 0;
		rotation[1, 1] = (float)Math.Cos(theta);
		rotation[1, 2] = (float)-Math.Sin(theta);
		rotation[1, 3] = 0;
		rotation[2, 0] = 0;
		rotation[2, 1] = (float)Math.Sin(theta);
		rotation[2, 2] = (float)Math.Cos(theta);;
		rotation[2, 3] = 0;
		rotation[3, 0] = 0;
		rotation[3, 1] = 0;
		rotation[3, 2] = 0;
		rotation[3, 3] = 1f;


		m = rotation * m;

		return m;
	}
}