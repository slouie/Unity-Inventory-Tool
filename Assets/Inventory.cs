using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Steven Louie
// Oct 17 2014
//
// Basic Inventory GUI module that supports moving items around
// and swapping places. If an item is right-clicked, a menu will
// show up with Use/Equip/Destroy options. Identical items can
// also be stacked. The inventory size, item sizes, position, and
// styles can all be customized in the Unity editor.
//
// To use, instantiate the Inventory prefab either through the editor
// or by script. You can access the inventory be either attaching the
// object to one of your other scripts, or by finding it by name. Both
// methods are used in ItemExample.cs and InventoryExample.cs.
public class Inventory : MonoBehaviour {

	// Public properties, change through editor
	public bool visible;
	public bool enableTooltips;
	public bool enableDragging;
	public bool enableOptions;
	public Vector2 position;
	public Vector2 itemSize;
	public int numRows;
	public int numCols;
	public int maxItems;
	public bool customInventory;
	public GUIStyle customInventoryStyle;
	public bool customTooltip;
	public GUIStyle customTooltipStyle;

	// Private constants	
	private static float MARGIN_X  = 5.0f;
	private static float MARGIN_Y  = 5.0f;
	private static float PADDING_X = 5.0f;
	private static float PADDING_Y = 5.0f;
	private static float DEFAULT_TOOLTIP_WIDTH = 200.0f;
	private static float DEFAULT_TOOLTIP_HEIGHT = 150.0f;

	// Inventory Rect
	private Rect parentBox;

	// Item options menu
	private int optionsId;
	private bool optionsOpened = false;
	private Vector2 optionsPosition;

	// Item drag
	private bool dragging = false;
	private int draggedId;

	// Key: inventory index (0 to maxItems - 1)
	// Value: IItem at index
	private Dictionary<int, IItem> items;

	// Key: item name
	// Value: stack count
	private Dictionary<string, int> stacks;
	

	public Inventory () {
		items = new Dictionary<int, IItem> ();
		stacks = new Dictionary<string, int> ();
	}

	// Called on every frame
	void OnGUI() {
		if (visible) {
			// Create the box holding the inventory
			float invLength = (numCols * itemSize.x) + (MARGIN_X * (numCols - 1)) + 2 * PADDING_X;
			float invHeight = (numRows * itemSize.y) + (MARGIN_Y * (numRows - 1)) + 2 * PADDING_Y;
			parentBox = new Rect (position.x, position.y, invLength, invHeight);
			if (customInventory) {
				GUI.Box (parentBox, "", customInventoryStyle);
			} else {
				GUI.Box (parentBox, "");
			}

			CreateInventory ();
		}
	}

	// Creates each box in the inventory and overlays any existing items
	// on top of the boxes.
	private void CreateInventory () {
		Event e = Event.current;

		// Hovering item
		Vector2 mouseRel = e.mousePosition - parentBox.position;
		int xIndex = (int) Mathf.Floor(mouseRel.x / (itemSize.x + PADDING_X));
		int yIndex = (int) Mathf.Floor(mouseRel.y / (itemSize.y + PADDING_Y));
		int id = -1;
		if (xIndex < numCols && yIndex < numRows) {
			id = yIndex * numCols + xIndex;
		}

		// Create empty boxes for the empty slots
		for (int row = 0; row < numRows; ++row) {
			for (int col = 0; col < numCols; ++col) {
				float x = MARGIN_X + col * (itemSize.x + PADDING_X);
				float y = MARGIN_Y + row * (itemSize.y + PADDING_Y);
				Rect item = new Rect (x, y, itemSize.x, itemSize.y);
				item.position += parentBox.position;
				GUI.Box (item, "");
			}
		}

		List<int> keys = new List<int>(items.Keys);
		foreach (int key in keys) {
			// Calculate the Rect for the item relative to the parent box.
			float x = MARGIN_X + (key % numCols) * (itemSize.x + PADDING_X);
			float y = MARGIN_Y + (key / numCols) * (itemSize.y + PADDING_Y);
			Rect box = new Rect (x, y, itemSize.x, itemSize.y);
			box.position += parentBox.position;

			if (e.button == 0 && e.isMouse) {
				// Left click
			} else if (e.button == 1 && e.type == EventType.MouseDown) {
				// Right click
				if (box.Contains(e.mousePosition)) {
					optionsOpened = true;
					optionsId = id;
					optionsPosition = e.mousePosition;
					e.Use ();
				}
			}

			// Move or swap a dragged item to a different slot on MouseUp.
			if (e.type == EventType.MouseUp && dragging && key == draggedId && !optionsOpened) {
				dragging = false;
				IItem item = items[key];
				if (parentBox.Contains(e.mousePosition)) {
					if (!items.ContainsKey(id)) {
						items.Remove(key);
						items.Add(id, item);
					} else {
						items[key] = items[id];
						items[id] = item;
					}
				}
			}
			// Keep track of the dragged box on MouseDrag.
			if (box.Contains(e.mousePosition)) {
				if (e.type == EventType.MouseDrag && !dragging && enableDragging) {
					dragging = true;
					draggedId = key;
				}
			}
			// Move the dragged box to the cursor.
			if (dragging && key == draggedId && !optionsOpened) {
				box.position = e.mousePosition -= itemSize / 2;
			}

			// Display the box.
			if (items.ContainsKey (key)) {
				GUI.Box (box, items[key].id);
				Texture text = Resources.Load(items[key].image, typeof(Texture)) as Texture;
				GUI.DrawTexture (box, text);
				if (items[key].stackable) {
					//GUI.Label(box, stacks[items[key].name].ToString());
					GUIStyle stackStyle = new GUIStyle();
					stackStyle.normal.textColor = Color.white;
					stackStyle.alignment = TextAnchor.LowerRight;
					GUI.Box (box, stacks[items[key].name].ToString(), stackStyle);
				}
			}

			// Create the tooltip when hovering over an item.
			if (items.ContainsKey(id) && !dragging && !optionsOpened && enableTooltips) {
				CreateTooltip(e.mousePosition, items[id]);
			}

			// Create item options if user right clicked.
			if (optionsOpened && enableOptions) {
				CreateOptions(optionsPosition, items[optionsId]);
			}
		}
	}

	// When user hovers over an item, display the tooltip for it.
	private void CreateTooltip(Vector2 position, IItem item) {
		Rect rect = new Rect (position.x, position.y, DEFAULT_TOOLTIP_WIDTH, DEFAULT_TOOLTIP_HEIGHT);
		// A custom tooltip can be set in the editor.
		if (customTooltip) {
			GUI.Box (rect, item.tooltip, customTooltipStyle);
		} else {
			GUI.Box (rect, item.tooltip);
		}
	}

	// When user right clicks an item, display the options menu for that item.
	// If the item is consumable, it can be used. Otherwise, it's an equippable
	// item. When an option is clicked, the appropriate method in IItem will be
	// invoked. 
	private void CreateOptions(Vector2 position, IItem item) {
		// Options container
		Rect rect = new Rect (position.x, position.y, 80, 40);
		GUI.Box (rect, "");

		if (item.consumable) {
			if (GUI.Button (new Rect (position.x, position.y, 80, 20), "Use")) {
				item.Use();
				optionsOpened = false;
			}
		} else {
			if (GUI.Button (new Rect (position.x, position.y, 80, 20), "Equip")) {
				item.Equip();
				optionsOpened = false;
			}
		}
		if (GUI.Button (new Rect (position.x, position.y + 20, 80, 20), "Destroy")) {
			item.Destroy();
			optionsOpened = false;
		}

		// Close the options menu on left click.
		if (Event.current.button == 0 && Event.current.isMouse) {
			optionsOpened = false;
			Event.current.Use ();
		}
	}

	// Adds an item to the inventory. If the item is a stackable item,
	// instead of adding the item, the count for that item will be
	// incremented.
	//
	// param item: The item to add to the inventory.
	// return bool: Insert success
	public bool AddItemToInventory (IItem item) {
		if (items.Values.Count < maxItems) {
			if (item.stackable) {
				if (stacks.ContainsKey(item.name)) {
					stacks[item.name]++;
				} else {
					items.Add (items.Count, item);
					stacks.Add (item.name, 1);
				}
			} else {
				items.Add (items.Count, item);
			}
			return true;
		}
		return false;
	}

	// Removes an item from the inventory by item id. If the item is
	// stackable, the stack count is decremented.
	//
	// param id: item id
	// return bool: Remove success
	public bool RemoveItemFromInventory (string id) {
		List<int> keys = new List<int>(items.Keys);
		foreach (int key in keys) {
			if (items[key].id == id) {
				if (items[key].stackable && stacks[items[key].name] > 1) {
					stacks[items[key].name]--;
				} else {
					items.Remove (key);
					return true;
				}
			}
		}
		return false;
	}

	// Checks if the inventory contains an item by id. This may not work
	// for stackable items. For that, it is better to use ContainsName.
	//
	// param id: item id
	// return bool
	public bool ContainsId(string id) {
		foreach (IItem item in items.Values) {
			if (item.id == id) {
				return true;
			}
		}
		return false;
	}

	// Checks if the inventory contains at least one item by name.
	//
	// param name: item name
	// return bool	
	public bool ContainsName(string name) {
		foreach (IItem item in items.Values) {
			if (item.name == name) {
				return true;
			}
		}
		return false;
	}

	// Gets an item by id if it exists.
	// 
	// param id: item id
	// return IItem: the item if it exists, null otherwise
	public IItem GetItemById(string id) {
		foreach (IItem item in items.Values) {
			if (item.id == id) {
				return item;
			}
		}
		return null;
	}

	// Gets the list of items with a given name. If the item is stackable,
	// it will only return the initial item.
	//
	// param name: item name
	// return List<IItem>: list of items
	public List<IItem> GetItemsByName(string name) {
		List<IItem> found = new List<IItem> ();
		foreach (IItem item in items.Values) {
			if (item.name == name) {
				found.Add (item);
			}
		}
		return found;
	}

	// Gets the stack size of a stackable item by name.
	//
	// param name: item name
	// return int: stack size
	public int GetStackSizeByName(string name) {
		int count = 0;
		foreach (IItem item in items.Values) {
			if (item.name == name && item.stackable) {
				count = stacks[item.name];
				break;
			}
		}
		return count;
	}
}
