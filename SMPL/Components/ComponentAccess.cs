using System.Collections.Generic;

namespace SMPL
{
	public class ComponentAccess
	{
		internal List<string> accessPaths = new();

		private bool disabled;
		public bool Disabled
		{
			get { return disabled; }
			set
			{
				if (disabled == value || (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				disabled = value;
			}
		}
		public bool IsCurrentlyAccessible(bool displayError = true)
		{
			if (Disabled) return true;
			var filePath = Debug.CurrentFilePath(2);
			if (accessPaths.Contains(filePath)) return true;
			if (displayError == false) return false;
			var filesWithAccess = "";
			for (int i = 0; i < accessPaths.Count; i++)
			{
				filesWithAccess += $"- {accessPaths[i]}";
				if (i < accessPaths.Count - 1) filesWithAccess += "\n";
			}
			Debug.LogError(2, $"Access was denied for '{filePath}'.\n" +
				$"'{Debug.CurrentMethodName(1)}'\ncan be accessed from the following files:\n" +
				filesWithAccess);
			return false;
		}
		public void GrantAccessToFile(string fullFilePath)
		{
			if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (fullFilePath == null)
			{
				Debug.LogError(1, "The file path cannot be 'null'.");
				return;
			}
			if (accessPaths.Contains(fullFilePath))
			{
				Debug.LogError(1, $"The file '{fullFilePath}' already has access.");
				return;
			}
			accessPaths.Add(fullFilePath);
		}
		public void DenyAccessToFile(string fullFilePath)
		{
			if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (accessPaths.Contains(fullFilePath) == false)
			{
				Debug.LogError(1, $"The file '{fullFilePath}' access is already denied.");
				return;
			}
			accessPaths.Add(fullFilePath);
		}
		public bool FileHasAccess(string fullFilePath) => accessPaths.Contains(fullFilePath);

		public ComponentAccess() => accessPaths.Add(Debug.CurrentFilePath(2));
	}
}
