using System.Collections;
using static Plugin.Maui.Calendar.Models.EventCollection;

namespace Plugin.Maui.Calendar.Shared.Models;

/// <summary>
/// Heavily based on EventCollection.cs
/// Literally just copied all of the code with changed names
/// </summary>
public class DayDetails : Dictionary<DateTime, IList>
{
	#region ctor

	public DayDetails() : base()
	{ }
	public DayDetails(int capacity) : base(capacity)
	{ }

	#endregion

	/// <summary>
	/// Removes a collection of values for specific date
	/// </summary>
	/// <param name="key">Event DateTime</param>
	/// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the System.Collections.Generic.Dictionary`2.</returns>
	public new bool Remove(DateTime key)
	{
		var removed = base.Remove(key.Date);

		if (removed)
		{
			CollectionChanged?.Invoke(this, new DayDetailsChangedArgs { Item = key.Date, Type = DayDetailsChangedType.Remove });
		}

		return removed;
	}

	/// <summary>
	/// Add collection of values for specific date
	/// </summary>
	/// <param name="key">Event DateTime</param>
	/// <param name="value">Collection of events for date</param>
	public new void Add(DateTime key, IList value)
	{
		base.Add(key.Date, value);
		CollectionChanged?.Invoke(this, new DayDetailsChangedArgs { Item = key.Date, Type = DayDetailsChangedType.Add });
	}

	/// <summary>
	/// Gets/sets collection of values for specific date
	/// </summary>
	/// <param name="key">Event DateTime</param>
	/// <returns>Collection of events for date</returns>
	public new IList this[DateTime key]
	{
		get => base[key.Date];
		set
		{
			base[key.Date] = value;
			CollectionChanged?.Invoke(this, new DayDetailsChangedArgs { Item = key.Date, Type = DayDetailsChangedType.Set });
		}
	}

	/// <summary>
	/// Checks if dictionary already has collection for specific date
	/// </summary>
	/// <param name="key">Key DateTime</param>
	/// <returns>true if dictionary already has the date as key; otherwise, false</returns>
	public new bool ContainsKey(DateTime key)
	{
		return base.ContainsKey(key.Date);
	}

	/// <summary>
	/// Gets the value associated with the specific date
	/// </summary>
	/// <param name="key">The date for the value to get</param>
	/// <param name="value">If the date exists then this is the associated collection; otherwise, it will be the default value of ICollection</param>
	/// <returns>true if dictionary contains an element with the specified date; otherwise false</returns>
	public new bool TryGetValue(DateTime key, out IList value)
	{
		return base.TryGetValue(key.Date, out value);
	}

	/// <summary>
	/// Gets the values associated with the specific date range
	/// </summary>
	/// <param name="keys"></param>
	/// <param name="values"></param>
	/// <returns></returns>
	public bool TryGetValues(IList<DateTime> keys, out IList values)
	{
		var listToReturn = new List<object>();

		foreach (var currentDate in keys)
		{
			if (base.TryGetValue(currentDate, out var dayEvents))
			{
				foreach (var singleEvent in dayEvents)
				{
					listToReturn.Add(singleEvent);
				}
			}
		}

		if (listToReturn.Count > 0)
		{
			values = listToReturn;
			return true;
		}
		else
		{
			values = null;
			return false;
		}
	}

	/// <summary>
	/// Removes all dates and collections
	/// </summary>
	public new void Clear()
	{
		if (base.Count == 0)
		{
			return;
		}

		base.Clear();
		CollectionChanged?.Invoke(this, new DayDetailsChangedArgs { Item = default, Type = DayDetailsChangedType.Clear });
	}

	internal event EventHandler<DayDetailsChangedArgs> CollectionChanged;

	internal sealed class DayDetailsChangedArgs
	{
		public DateTime Item { get; set; }
		public DayDetailsChangedType Type { get; set; }
	}

	internal enum DayDetailsChangedType
	{
		Add,
		Set,
		Remove,
		Clear
	}
}
