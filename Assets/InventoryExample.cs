using UnityEngine;
using System.Collections;

// Example of Inventory usage
// The items use textures in Assets/Resources originally from FFXIV
public class InventoryExample : MonoBehaviour {

	// Bind the Inventory object to this class in the Unity editor.
	public GameObject inventory;

	void Start () {
		Inventory inv = inventory.GetComponent<Inventory> ();

		// Create objects of an IItem implementation
		ItemExample item1 = new ItemExample ("0", "Evenstar Helmet", "<b>Evenstar Helmet</b>\n\nSample Text", "evenstar_helm", false, false);
		ItemExample item2 = new ItemExample ("1", "Curtana Novus", "<b>Curtana Novus</b>\n\nSample Text", "curtana_novus", false, false);
		ItemExample item3 = new ItemExample ("2", "High Allagan Shield", "<b>High Allagan Shield</b>\n\nSample Text", "ha_shield", false, false);
		ItemExample item4 = new ItemExample ("3", "Elkhorn Robe", "<b>Elkhorn Robe</b>\n\nSample Text", "elkhorn_robe", false, false);

		// Stackable potions
		ItemExample item5 = new ItemExample ("4", "X-Potion", "<b>X-Potion</b>\n\nSample Text", "x_potion", true, true);
		ItemExample item6 = new ItemExample ("5", "X-Potion", "<b>X-Potion</b>\n\nSample Text", "x_potion", true, true);
		ItemExample item7 = new ItemExample ("6", "X-Potion", "<b>X-Potion</b>\n\nSample Text", "x_potion", true, true);
		ItemExample item8 = new ItemExample ("7", "X-Potion", "<b>X-Potion</b>\n\nSample Text", "x_potion", true, true);

		// Insert
		inv.AddItemToInventory (item1);
		inv.AddItemToInventory (item2);
		inv.AddItemToInventory (item3);
		inv.AddItemToInventory (item4);
		inv.AddItemToInventory (item5);
		inv.AddItemToInventory (item6);
		inv.AddItemToInventory (item7);
		inv.AddItemToInventory (item8);

		// Other methods

		// Stack size of X-Potions
		Debug.Log (inv.GetStackSizeByName ("X-Potion") + " X-Potions");

		// Check if Elkhorn Robe exists
		Debug.Log (inv.ContainsId ("3"));

		// Check if High Allagan Shield exists
		Debug.Log (inv.ContainsName ("High Allagan Shield"));

		// Change tooltip of Evenstar Helmet
		ItemExample i = inv.GetItemById ("0") as ItemExample;
		i.tooltip += "\nTest";
	}

	void Update () {
	
	}
}
