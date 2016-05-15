﻿using UnityEngine;
using System.Collections.Generic;

namespace Proeve
{
	public enum GridDirection
	{
		Up,
		Left,
		Down,
		Right
	}

	/// <summary>
	/// The grid of a level.
	/// </summary>
	public class Grid : MonoBehaviour
	{
		/// <summary>
		/// The width in tiles of the grid.
		/// </summary>
		public const int WIDTH = 50;

		/// <summary>
		/// The height in tiles of the grid.
		/// </summary>
		public const int HEIGHT = 50;

		/// <summary>
		/// The tile size in meters of the grid.
		/// </summary>
		public const int SIZE = 3;

		/// <summary>
		/// All <see cref="GridNode"/> in the grid.
		/// </summary>
		public IEnumerable<GridNode> Nodes
		{
			get
			{
				return nodes;
			}
		}

		[HideInInspector, SerializeField] private List<GridNode> nodes;
		private Dictionary<Vector2, GridNode> nodeCache;

		protected void Awake()
		{
			nodeCache = new Dictionary<Vector2, GridNode>();

			foreach(GridNode node in nodes)
			{
				nodeCache.Add(node.GridPosition, node);
			}
		}

		protected void OnDrawGizmosSelected()
		{
			DrawGizmos();
		}

		/// <summary>
		/// Draw gizmos of the grid, called by <see cref="OnDrawGizmosSelected"/> and <see cref="GridNode.OnDrawGizmosSelected"/>.
		/// It will draw the gizmos of the grid and the <see cref="GridNode"/>s.
		/// </summary>
		public void DrawGizmos()
		{
			Gizmos.color = Color.black;
			Gizmos.matrix = transform.localToWorldMatrix;

			for(int x = -WIDTH; x < WIDTH + 1; x++)
			{
				Vector3 start = new Vector3(x, 0, -HEIGHT) * SIZE;
				Vector3 end = new Vector3(x, 0, HEIGHT) * SIZE;

				Gizmos.DrawLine(start, end);
			}

			for(int z = -HEIGHT; z < HEIGHT + 1; z++)
			{
				Vector3 start = new Vector3(-WIDTH, 0, z) * SIZE;
				Vector3 end = new Vector3(WIDTH, 0, z) * SIZE;

				Gizmos.DrawLine(start, end);
			}

			Gizmos.matrix = Matrix4x4.identity;
			foreach(GridNode node in nodes)
			{
				node.DrawGizmos();
			}
		}

		/// <summary>
		/// Add a <see cref="GridNode"/> to the grid.
		/// </summary>
		/// <param name="_node">The node to add.</param>
		/// <returns>Whether or not the node has been added succesfully.</returns>
		public bool AddNode(GridNode _node)
		{
			if(!nodes.Contains(_node))
			{
				nodes.Add(_node);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Remove a <see cref="GridNode"/> from the grid.
		/// </summary>
		/// <param name="_node">The node to remove.</param>
		/// <returns>Whether or not the node has been removed successfully.</returns>
		public bool RemoveNode(GridNode _node)
		{
			return nodes.Remove(_node);
		}

		/// <summary>
		/// Get the <see cref="GridNode"/> at the specified position.
		/// </summary>
		/// <remarks>
		/// <paramref name="_position"/> represents a position on the grid, not transform.position.
		/// </remarks>
		/// <param name="_position">The position of the node.</param>
		/// <returns>The <see cref="GridNode"/> at the specified position, or null if there is none.</returns>
		public GridNode GetNodeAt(Vector2 _position)
		{
			if(nodeCache == null)
			{
				foreach(GridNode node in nodes)
				{
					if(node.GridPosition == _position)
					{
						return node;
					}
				}
			}
			else
			{
				if(nodeCache.ContainsKey(_position))
				{
					return nodeCache[_position];
				}
			}

			return null;
		}
	}
}
