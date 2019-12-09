using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleTargeter : MonoBehaviour
{
	public Transform currentTarget;

	public Transform grappleContainer;
	public float grappleCheckRadius = 5f;
	public LayerMask grappleMask;

	private Collider[] _colliders;
	private List<Transform> _grappleTargets = new List<Transform>();

	private void Awake()
	{
		foreach(Transform t in grappleContainer)
		{
			_grappleTargets.Add(t);
		}
	}

	private void Update()
	{
		_colliders = Physics.OverlapSphere(transform.position, grappleCheckRadius, grappleMask);
		if (_colliders.Length > 0)
		{
			currentTarget = _colliders[0].transform;
		} else
		{
			currentTarget = null;
		}
	}

	private void OnDrawGizmos()
	{
		foreach (Transform target in _grappleTargets)
		{
			Gizmos.DrawSphere(target.position, grappleCheckRadius);
		}
	}
}
