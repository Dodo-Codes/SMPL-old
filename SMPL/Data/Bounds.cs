namespace SMPL.Data
{
	public struct Bounds
	{
		public double Lower { get; set; }
		public double Upper { get; set; }

		public Bounds(double lower, double upper)
		{
			Lower = lower;
			Upper = upper;
		}
	}
}
