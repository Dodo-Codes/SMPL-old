using SMPL.Gear;

namespace RPG1bit
{
	/// <summary>
	/// A <see cref="GameObject"/> that shows the interactable indicator when the <see cref="Player"/> is in range and mouse hovered.
	/// </summary>
	public interface IInteractable { }
	/// <summary>
	/// A <see cref="GameObject"/> that has its type name as a tag. Useful for <see cref="Thing.PickByTag(string)"/> to get all
	/// <see cref="GameObject"/>s of the same type.
	/// </summary>
	public interface ITypeTaggable { }
	/// <summary>
	/// A <see cref="GameObject"/> that the <see cref="Player"/> can interact with is bringed back upon stepping over.
	/// </summary>
	public interface ISolid { }
	/// <summary>
	/// A <see cref="GameObject"/> that the <see cref="Player"/> can ride.
	/// </summary>
	public interface IRidable { }
	/// <summary>
	/// A <see cref="GameObject"/> that is recreated from the current <see cref="Chunk.Data"/> graphics if the <see cref="Player"/> is
	/// close enough. <br></br>If the <see cref="Player"/> is far enough, its graphics are placed in the current
	/// <see cref="Chunk.Data"/> and it is deleted. <br></br>All of this is handled by the <see cref="WorldObjectManager"/>.
	/// </summary>
	public interface IRecreatable { }
	/// <summary>
	/// A <see cref="GameObject"/> that can be saved between game sessions. Starting a new game session will make the
	/// <see cref="WorldObjectManager"/> load it.
	/// </summary>
	public interface ISavable { }
	/// <summary>
	/// A <see cref="GameObject"/> that can be saved to the cache when unloading chunks and loaded later
	/// when needed by the <see cref="ChunkManager"/>.
	/// </summary>
	public interface ICachable { }
}
