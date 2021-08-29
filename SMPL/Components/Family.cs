using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Family : Access
	{
		internal Visual owner;
		internal List<Visual> children = new();

		private static event Events.ParamsTwo<Family, Identity<Family>> OnIdentityChange;
		private static event Events.ParamsOne<Family> OnCreate, OnRemoveAllChildren;
		private static event Events.ParamsTwo<Family, Visual> OnParentChange, OnAddChild, OnRemoveChild;

		public static class CallWhen
		{
			public static void IdentityChange(Action<Family, Identity<Family>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void Create(Action<Family> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void ParentChange(Action<Family, Visual> method, uint order = uint.MaxValue) =>
				OnParentChange = Events.Add(OnParentChange, method, order);
			public static void AddChildren(Action<Family, Visual> method, uint order = uint.MaxValue) =>
				OnAddChild = Events.Add(OnAddChild, method, order);
			public static void RemoveChildren(Action<Family, Visual> method, uint order = uint.MaxValue) =>
				OnRemoveChild = Events.Add(OnRemoveChild, method, order);
			public static void RemoveAllChildren(Action<Family> method, uint order = uint.MaxValue) =>
				OnRemoveAllChildren = Events.Add(OnRemoveAllChildren, method, order);
		}

		private Identity<Family> identity;
		public Identity<Family> Identity
		{
			get { return identity; }
			set
			{
				if (identity == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				if (Debug.CalledBySMPL == false) OnIdentityChange?.Invoke(this, prev);
			}
		}

		private Visual parent;
		public Visual Parent
		{
			get { return parent; }
			set
			{
				if (parent == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;

				var pos = Point.From(owner.Area.LocalPosition);
				var angle = owner.Area.LocalAngle;
				var prevPar = parent;

				parent = value;

				if (value != null) // parent
				{
					var parAng = parent.Area.LocalAngle;
					var newPos = value.Area.sprite.InverseTransform.TransformPoint(pos);

					value.Family.children.Add(owner);
					owner.Area.LocalPosition = Point.To(newPos);
					owner.Area.LocalSize -= parent.Area.Size;
					owner.Area.LocalAngle = -(parAng - angle);
				}
				else // unparent
				{
					var newPos = prevPar == null ? pos : prevPar.Area.sprite.Transform.TransformPoint(pos);
					var parAng = prevPar.Area.sprite.Rotation;

					prevPar.Family.children.Remove(owner);
					owner.Area.LocalPosition = Point.To(newPos);
					owner.Area.LocalSize += prevPar.Area.Size;
					owner.Area.LocalAngle = parAng + angle;
				}
				if (Debug.CalledBySMPL == false) OnParentChange?.Invoke(this, prevPar);
			}
		}
		public Visual[] Children => children.ToArray();
		public int ChildrenCount => children.Count;

		public void ParentChildren(params Visual[] childrenInstances)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (childrenInstances == null) { Debug.LogError(1, "ComponentVisual children cannot be 'null'."); return; }
			for (int i = 0; i < childrenInstances.Length; i++)
			{
				if (children.Contains(childrenInstances[i])) continue;
				if (childrenInstances[i].Family == null)
				{
					Debug.LogError(1, $"Cannot parent this child instance because it has no Family.\n" +
						$"Both (parent & child) Visual instances need a Family in order to bond.");
					continue;
				}
				childrenInstances[i].Family.Parent = owner;
				if (Debug.CalledBySMPL == false) OnAddChild?.Invoke(this, childrenInstances[i]);
			}
		}
		public void UnparentChildren(params Visual[] childrenInstances)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (childrenInstances == null) { Debug.LogError(1, "ComponentVisual children cannot be 'null'."); return; }
			for (int i = 0; i < childrenInstances.Length; i++)
			{
				if (children.Contains(childrenInstances[i]) == false) continue;
				childrenInstances[i].Family.Parent = null;
				if (Debug.CalledBySMPL == false) OnRemoveChild?.Invoke(this, childrenInstances[i]);
			}
		}
		public void UnparentAllChildren()
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			for (int i = 0; i < children.Count; i++)
			{
				children[i].Family.Parent = null;
				if (Debug.CalledBySMPL == false) OnRemoveChild?.Invoke(this, children[i]);
			}
			if (Debug.CalledBySMPL == false) OnRemoveAllChildren?.Invoke(this);
		}
		public bool HasChildren(params Visual[] childrenInstances)
		{
			if (childrenInstances == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < childrenInstances.Length; i++)
				if (children.Contains(childrenInstances[i]) == false)
					return false;
			return true;
		}

		public Family(string uniqueID)
		{
			if (Identity<Family>.CannotCreate(uniqueID)) return;
			Identity = new(this, uniqueID);

			GrantAccessToFile(Debug.CurrentFilePath(1)); // grant the user access
			DenyAccessToFile(Debug.CurrentFilePath(0)); // abandon ship

			OnCreate?.Invoke(this);
		}
	}
}
