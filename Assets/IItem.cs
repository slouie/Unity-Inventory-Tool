using UnityEngine;
using System;

// All items inserted into the inventory must implement this interface.
// See ItemExample for a sample implementation.
public interface IItem {
	// Unique item id
	string id { get; }

	// Item name
	string name { get; set; }

	// Item hover tooltip, can be in rich text format
	string tooltip { get; set; }

	// File name of item's image icon without the extension
	// The texture must be stored in Assets/Resources
	string image { get; set; }

	// Other properties
	bool consumable { get; set; }
	bool stackable { get; set; }

	// These are the callbacks for the item options events.
	bool Use();
	bool Equip();
	bool Destroy();
}


