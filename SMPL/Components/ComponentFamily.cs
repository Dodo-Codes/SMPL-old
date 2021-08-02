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
				var size = owner.transform.LocalSize;
				var angle = owner.transform.LocalAngle;
				var parSz = parent == null ? owner.transform.LocalSize : parent.transform.LocalSize;
				var futureParSz = value == null ? owner.transform.LocalSize : value.transform.LocalSize;
				var prevPar = parent;

				parent = value;

				if (value != null) // parent
				{
					var parAng = parent.transform.LocalAngle;
					var newPos = value.transform.sprite.InverseTransform.TransformPoint(pos);
					var ssc = new Size(futureParSz.W / size.W, futureParSz.H / size.H);

					value.Family.children.Add(owner);
					owner.transform.LocalPosition = Point.To(newPos);
					if (owner is ComponentSprite) owner.transform.LocalSize = size * ssc;
					owner.transform.LocalAngle = -(parAng - angle);
				}
				else // unparent
				{
					var newPos = prevPar == null ? pos : prevPar.transform.sprite.Transform.TransformPoint(pos);
					var parAng = prevPar.transform.sprite.Rotation;
					var ssc = new Size(parSz.W / size.W, parSz.H / size.H);
					prevPar.Family.children.Remove(owner);
					owner.transform.LocalPosition = Point.To(newPos);
					if (owner is ComponentSprite) owner.transform.LocalSize = size / ssc;
					owner.transform.LocalAngle = parAng + angle;
				}
			}
		}
		internal List<ComponentVisual> children = new();

		public ComponentFamily(ComponentVisual owner) => this.owner = owner;
	}
}
