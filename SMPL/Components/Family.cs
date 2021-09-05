using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Family : Component
	{
		private Visual parent;

		//============

		internal Visual owner;
		internal List<Visual> children = new();

		//============

		public Visual Parent
		{
			get { return ErrorIfDestroyed() ? default : parent; }
			set
			{
				if (ErrorIfDestroyed()) return;

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
			}
		}
		public Visual[] Children => ErrorIfDestroyed() ? default : children.ToArray();

		public void ParentChildren(params Visual[] childrenInstances)
		{
			if (ErrorIfDestroyed()) return;
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
			}
		}
		public void UnparentChildren(params Visual[] childrenInstances)
		{
			if (ErrorIfDestroyed()) return;
			if (childrenInstances == null) { Debug.LogError(1, "ComponentVisual children cannot be 'null'."); return; }
			for (int i = 0; i < childrenInstances.Length; i++)
			{
				if (children.Contains(childrenInstances[i]) == false) continue;
				childrenInstances[i].Family.Parent = null;
			}
		}
		public void UnparentAllChildren()
		{
			if (ErrorIfDestroyed()) return;
			for (int i = 0; i < children.Count; i++)
				children[i].Family.Parent = null;
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

		public Family(string uniqueID) : base(uniqueID)
		{
			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			UnparentAllChildren();
			Parent = null;
			owner = null;
			base.Destroy();
		}
	}
}
