using System.Collections.Generic;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.LootDropSystem
{
    /// <summary>
	/// Class serves for assigning and picking loot drop items.
	/// </summary>
	public abstract class GenericLootDropTable<T, TU> where T : GenericLootDropItem<TU>
    {
		[SerializeField] internal List<T> lootDropItems;
		
		private float probabilityTotalWeight;
	
		/// <summary>
		/// Calculates the percentage and assigns the probabilities how many times
		/// the items can be picked. Function used also to validate data when tweaking numbers in editor.
		/// </summary>	
		public void ValidateTable()
		{
			if (lootDropItems is not { Count: > 0 }) return;
			
			float currentProbabilityWeightMaximum = 0f;
	
			// Sets the weight ranges of the selected items.
			foreach(T lootDropItem in lootDropItems)
			{
				if(lootDropItem.probabilityWeight < 0f)
				{
					// Prevent usage of negative weight.
					Debug.Log("You can't have negative weight on an item. Resetting item's weight to 0.");
					lootDropItem.probabilityWeight = 0f;
				}
				else
				{
					//Debug.Log($"Item {lootDropItem.item} has weight {lootDropItem.probabilityWeight}");
					lootDropItem.probabilityRangeFrom = currentProbabilityWeightMaximum;
					currentProbabilityWeightMaximum += lootDropItem.probabilityWeight;	
					lootDropItem.probabilityRangeTo = currentProbabilityWeightMaximum;						
				}
			}
	
			probabilityTotalWeight = currentProbabilityWeightMaximum;
	
			// Calculate percentage of item drop select rate.
			foreach(T lootDropItem in lootDropItems)
			{
				lootDropItem.probabilityPercent = ((lootDropItem.probabilityWeight) / probabilityTotalWeight) * 100;
			}
		}
	
		/// <summary>
		/// Picks and returns the loot drop item based on it's probability.
		/// </summary>
		public T PickLootDropItem()
		{		
			float pickedNumber = Random.Range(0, probabilityTotalWeight);
			//Debug.Log($"Picked number: {pickedNumber}");
			
			foreach (T lootDropItem in lootDropItems)
			{
				//Debug.Log($"{lootDropItem.item} probability range: [{lootDropItem.probabilityRangeFrom}, {lootDropItem.probabilityRangeTo}]");
				// If the picked number matches the item's range, return item
				if(pickedNumber > lootDropItem.probabilityRangeFrom && pickedNumber < lootDropItem.probabilityRangeTo)
				{
					return lootDropItem;
				}
			}	
	
			// If an item wasn't picked... Notify the programmer via console and return the first item from the list
			Debug.LogError("Item couldn't be picked... Be sure that all of your active loot drop tables have assigned at least one item!");
			return lootDropItems[0];
		}
	}
}