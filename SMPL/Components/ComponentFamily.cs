using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace SMPL
{
	public class ComponentFamily : ComponentAccess
	{
		internal ComponentVisual owner;
		internal List<ComponentVisual> children = new();

		private static event Events.ParamsTwo<ComponentFamily, ComponentIdentity<ComponentFamily>> OnIdentityChange;
		private static event Events.ParamsOne<ComponentFamily> OnCreate, OnRemoveAllChildren;
		private static event Events.ParamsTwo<ComponentFamily, ComponentVisual> OnParentChange, OnAddChild, OnRemoveChild;

		public static class CallWhen
		{
			public static void IdentityChange(Action<ComponentFamily, ComponentIdentity<ComponentFamily>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void Create(Action<ComponentFamily> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void ParentChange(Action<ComponentFamily, ComponentVisual> method, uint order = uint.MaxValue) =>
				OnParentChange = Events.Add(OnParentChange, method, order);
			public static void AddChildren(Action<ComponentFamily, ComponentVisual> method, uint order = uint.MaxValue) =>
				OnAddChild = Events.Add(OnAddChild, method, order);
			public static void RemoveChildren(Action<ComponentFamily, ComponentVisual> method, uint order = uint.MaxValue) =>
				OnRemoveChild = Events.Add(OnRemoveChild, method, order);
			public static void RemoveAllChildren(Action<ComponentFamily> method, uint order = uint.MaxValue) =>
				OnRemoveAllChildren = Events.Add(OnRemoveAllChildren, method, order);
		}

		private ComponentIdentity<ComponentFamily> identity;
		public ComponentIdentity<ComponentFamily> Identity
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

		private ComponentVisual parent;
		public ComponentVisual Parent
		{
			get { return parent; }
			set
			{
				if (parent == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;

				var pos = Point.From(owner.transform.LocalPosition);
				var angle = owner.transform.LocalAngle;
				var prevPar = parent;

				parent = value;

				if (value != null) // parent
				{
					var parAng = parent.transform.LocalAngle;
					var newPos = value.transform.sprite.InverseTransform.TransformPoint(pos);

					value.Family.children.Add(owner);
					owner.transform.LocalPosition = Point.To(newPos);
					owner.transform.Size = owner.transform.Size;
					owner.transform.LocalAngle = -(parAng - angle);
				}
				else // unparent
				{
					var newPos = prevPar == null ? pos : prevPar.transform.sprite.Transform.TransformPoint(pos);
					var parAng = prevPar.transform.sprite.Rotation;

					prevPar.Family.children.Remove(owner);
					owner.transform.LocalPosition = Point.To(newPos);
					owner.transform.Size = owner.transform.Size;
					owner.transform.LocalAngle = parAng + angle;
				}
				if (Debug.CalledBySMPL == false) OnParentChange?.Invoke(this, prevPar);
			}
		}
		public ComponentVisual[] Children => children.ToArray();
		public int ChildrenCount => children.Count;

		public void ParentChildren(params ComponentVisual[] childrenInstances)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (childrenInstances == null) { Debug.LogError(1, "ComponentVisual children cannot be 'null'."); return; }
			for (int i = 0; i < childrenInstances.Length; i++)
			{
				if (children.Contains(childrenInstances[i])) continue;
				childrenInstances[i].Family.Parent = owner;
				if (Debug.CalledBySMPL == false) OnAddChild?.Invoke(this, childrenInstances[i]);
			}
		}
		public void UnparentChildren(params ComponentVisual[] childrenInstances)
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
		public bool HasChildren(params ComponentVisual[] childrenInstances)
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

		public ComponentFamily(ComponentVisual owner) : base()
		{
			this.owner = owner;
			OnCreate?.Invoke(this);
		}
	}
}
