﻿using UnityEngine;
using System.Collections;
using Utils;

namespace Proeve
{
	public class LimitedUsesPickup : MonoBehaviour
	{
		[SerializeField] private Rigidbody[] rigidbodies;
		[SerializeField] private GridNode[] nodes;

		[SerializeField] private int numUses;

		private int remaining;

		protected void Awake()
		{
			remaining = numUses;
		}

		protected void OnEnable()
		{
			GlobalEvents.AddListener<PlayerMovedEvent>(OnPlayerMovedEvent);
		}

		protected void OnDisable()
		{
			GlobalEvents.RemoveListener<PlayerMovedEvent>(OnPlayerMovedEvent);
		}

		private void OnPlayerMovedEvent(PlayerMovedEvent evt)
		{
			if(!IsValid(evt.To) && IsValid(evt.From))
			{
				if(remaining <= 0)
				{
					foreach(GridNode node in nodes)
					{
						node.Active = false;
					}
					
					StartCoroutine(HandleRigidbodies());
				}
			}
			else if(!IsValid(evt.From) && IsValid(evt.To))
			{
				remaining--;
			}
		}

		private IEnumerator HandleRigidbodies()
		{
			SetKinematic(false);

			yield return new WaitForSeconds(1.5f);

			SetKinematic(true);
		}

		private void SetKinematic(bool kinematic)
		{
			foreach(Rigidbody rigidbody in rigidbodies)
			{
				rigidbody.isKinematic = kinematic;
			}
		}

		private bool IsValid(GridNode node)
		{
			foreach(GridNode n in nodes)
			{
				if(node == n)
				{
					return true;
				}
			}

			return false;
		}
	}
}
