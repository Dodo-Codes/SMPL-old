using SMPL.Data;

namespace RPG1bit
{
	public class Tick : GameObject
	{
		public bool Value { get; protected set; }

		public Tick(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnLeftClicked()
		{
			Value = !Value;
			OnValueChanged();
		}
		public override void OnDisplay(Point screenPos)
		{
			Screen.EditCell(screenPos, new(Value ? 40 : 39, 14), 1, Position.Color);
		}
		public virtual void OnValueChanged() { }
	}
}
