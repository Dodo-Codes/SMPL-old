using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SMPL
{
	public class ComponentFamily
	{
		internal readonly ComponentVisual owner;
		private ComponentVisual parent;
		public ComponentVisual Parent
		{
			get { return parent; }
			set
			{
				if (parent == value) return;

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
			}
		}
		internal List<ComponentVisual> children = new();

		public ComponentFamily(ComponentVisual owner) => this.owner = owner;
	}
}
