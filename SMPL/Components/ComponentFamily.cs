using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace SMPL
{
	public class ComponentFamily : ComponentAccess
	{
		internal readonly ComponentVisual owner;
		internal List<ComponentVisual> children = new();

		private static event Events.ParamsTwo<ComponentFamily, ComponentIdentity<ComponentFamily>> OnIdentityChange;
		private static event Events.ParamsOne<ComponentFamily> OnCreate, OnRemoveAllChildren;
		private static event Events.ParamsTwo<ComponentFamily, ComponentVisual> OnParentChange, OnAddChild, OnRemoveChild;

		public static void CallOnIdentityChange(Action<ComponentFamily, ComponentIdentity<ComponentFamily>> method,
			uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
		public static void CallOnCreate(Action<ComponentFamily> method, uint order = uint.MaxValue) =>
			OnCreate = Events.Add(OnCreate, method, order);
		public static void CallOnParentChange(Action<ComponentFamily, ComponentVisual> method, uint order = uint.MaxValue) =>
			OnParentChange = Events.Add(OnParentChange, method, order);
		public static void CallOnAddChildren(Action<ComponentFamily, ComponentVisual> method, uint order = uint.MaxValue) =>
			OnAddChild = Events.Add(OnAddChild, method, order);
		public static void CallOnRemoveChildren(Action<ComponentFamily, ComponentVisual> method, uint order = uint.MaxValue) =>
			OnRemoveChild = Events.Add(OnRemoveChild, method, order);
		public static void CallOnRemoveAllChildren(Action<ComponentFamily> method, uint order = uint.MaxValue) =>
			OnRemoveAllChildren = Events.Add(OnRemoveAllChildren, method, order);

		private ComponentIdentity<ComponentFamily> identity;
		public ComponentIdentity<ComponentFamily> Identity
		{
			get { return identity; }
			set
			{
				if (identity == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				OnIdentityChange?.Invoke(this, prev);
			}
		}

		private ComponentVisual parent;
		public ComponentVisual Parent
		{
			get { return parent; }
			set
			{
				if (parent == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;

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
				OnParentChange?.Invoke(this, prevPar);
			}
		}
		public ComponentVisual[] Children => children.ToArray();
		public int ChildrenCount => children.Count;

		public void AddChildren(params ComponentVisual[] childrenInstances)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (childrenInstances == null) { Debug.LogError(1, "ComponentVisual children cannot be 'null'."); return; }
			for (int i = 0; i < childrenInstances.Length; i++)
			{
				if (children.Contains(childrenInstances[i])) continue;
				childrenInstances[i].Family.Parent = owner;
				OnAddChild?.Invoke(this, childrenInstances[i]);
			}
		}
		public void RemoveChildren(params ComponentVisual[] childrenInstances)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (childrenInstances == null) { Debug.LogError(1, "ComponentVisual children cannot be 'null'."); return; }
			for (int i = 0; i < childrenInstances.Length; i++)
			{
				if (children.Contains(childrenInstances[i]) == false) continue;
				childrenInstances[i].Family.Parent = null;
				OnRemoveChild?.Invoke(this, childrenInstances[i]);
			}
		}
		public void RemoveAllChildren()
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			for (int i = 0; i < children.Count; i++)
			{
				children[i].Family.Parent = null;
				OnRemoveChild?.Invoke(this, children[i]);
			}
			OnRemoveAllChildren?.Invoke(this);
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
