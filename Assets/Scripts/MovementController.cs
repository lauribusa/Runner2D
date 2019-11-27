using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	BoxCollider2D boxCollider;
	[HideInInspector]
	public Vector2 bottomLeft, bottomRight, topLeft, topRight;

	public int horizontalRayCount;
	public int verticalRayCount;

	public LayerMask layerWall;
	public LayerMask layerOneWayPlatform;
	public Collisions _collisions;

	public float _pixelPerUnit = 16f;
	[HideInInspector]
	public float skinWidth;
	float pitDistance;

	float verticalRaySpacing;
	float horizontalRaySpacing;

	public struct Collisions
	{
		public bool top, bottom, left, right;

		public bool frontPit;
		public void Reset()
		{
			top = bottom = left = right = false;
			frontPit = false;
		}
	}
	// Start is called before the first frame update
	void Start()
	{
		skinWidth = 1 / _pixelPerUnit;
		pitDistance = 0.5f;
		boxCollider = GetComponent<BoxCollider2D>();
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2f);
		verticalRaySpacing = bounds.size.y / (verticalRayCount - 1);
		horizontalRaySpacing = bounds.size.x / (horizontalRayCount - 1);
	}
	public void ReCalculateBounds()
	{
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2f);
		verticalRaySpacing = bounds.size.y / (verticalRayCount - 1);
		horizontalRaySpacing = bounds.size.x / (horizontalRayCount - 1);
	}
	void ComputeBounds()
	{
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2f);
		bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		topLeft = new Vector2(bounds.min.x, bounds.max.y);
		topRight = new Vector2(bounds.max.x, bounds.max.y);
	}
	// Update is called once per frame
	public void Attack(bool flip, int tongueLength, LayerMask enemyLayer)
	{
		float direction = flip ? -1 : 1;
		float distance = tongueLength + skinWidth;
		print(topRight / bottomRight);
		Vector2 baseOrigin = direction == 1 ? topRight : topLeft;
		Vector2 origin = new Vector2(baseOrigin.x, ((topRight.y + bottomRight.y) / 2) - (1f / 5f)); //+ new Vector2(verticalRaySpacing, 0);
		Debug.DrawLine(origin, origin + new Vector2(direction * distance, 0));


		RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(0, direction), distance, enemyLayer);
	}


	public void Move(Vector2 velocity)
	{
		_collisions.Reset();
		ComputeBounds();
		if (velocity.x != 0)
		{
			HorizontalMove(ref velocity);

		}
		if (velocity.y != 0)
		{
			VerticalMove(ref velocity);
		}
		DetectFrontPit(velocity);
		transform.Translate(velocity);
	}

	public void VerticalMove(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.y);
		float distance = Mathf.Abs(velocity.y) + skinWidth;

		Vector2 baseOrigin = direction == 1 ? topLeft : bottomLeft;
		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 origin = baseOrigin + new Vector2(horizontalRaySpacing * i, 0);
			Debug.DrawLine(origin, origin + new Vector2(0, direction * distance));

			RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(0, direction), distance, layerWall);
			if (hit)
			{

				/*print(hit.transform.gameObject.layer);
				print(layerOneWayPlatform);*/
				print("touched: " + hit.transform.gameObject.tag);
				if (!(hit.transform.gameObject.tag == "oneWayPlatform" && direction > 0))
				{
					velocity.y = (hit.distance - skinWidth) * direction;
					distance = hit.distance - skinWidth;

					if (direction < 0)
					{
						_collisions.bottom = true;
					}
					if (direction > 0)
					{
						_collisions.top = true;
					}
				}

			}
		}
	}

	public void HorizontalMove(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.x);
		float distance = Mathf.Abs(velocity.x) + skinWidth;
		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 baseOrigin = direction == 1 ? bottomRight : bottomLeft;
			Vector2 origin = baseOrigin + new Vector2(0, verticalRaySpacing * i);
			Debug.DrawLine(origin, origin + new Vector2(direction * distance, 0));
			RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(direction, 0), distance, layerWall);
			if (hit)
			{
				velocity.x = (hit.distance - skinWidth) * direction;
				if (direction < 0)
				{
					_collisions.left = true;
				}
				else
				if (direction > 0)
				{
					_collisions.right = true;
				}

			}
		}
	}
	void DetectFrontPit(Vector2 velocity)
	{
		Vector2 origin = velocity.x > 0 ? bottomRight : bottomLeft;

		//Debug.DrawLine(origin, origin + Vector2.down * pitDistance);
		RaycastHit2D hit = Physics2D.Raycast(
			origin,
			Vector2.down,
			pitDistance,
			layerWall
			);

		if (!hit)
		{
			_collisions.frontPit = true;
		}
	}
}
