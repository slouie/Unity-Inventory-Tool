using UnityEngine;
using System;

// Example implementation of IItem
// ItemExample objects can be inserted into Inventory
public class ItemExample : IItem {

	private string _id;

	public string id { 
		get { return _id; }
	}

	public string name { get; set; }
	public string tooltip { get; set; }
	public string image { get; set; }
	public bool consumable { get; set; }
	public bool stackable { get; set; }

	public ItemExample (string _id, string name, string tooltip, string image, bool consumable, bool stackable) {
		this._id = _id;
		this.name = name;
		this.tooltip = tooltip;
		this.image = image;
		this.consumable = consumable;
		this.stackable = stackable;
	}

	// Callbacks for item options events
	// Currently, only Use/Equip/Destroy
	public bool Use() {
		Debug.Log ("Used " + name);
		return GameObject.Find ("Inventory").GetComponent<Inventory> ().RemoveItemFromInventory (id);
	}

	public bool Equip() {
		Debug.Log ("Equipped " + name);
		return true;
	}

	public bool Destroy() {
		Debug.Log ("Destroyed " + name);
		return GameObject.Find ("Inventory").GetComponent<Inventory> ().RemoveItemFromInventory (id);
	}


}


