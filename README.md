# LethalCompanyShipMaid
A mod where the ship and the storage closet is cleaned up when you ~~scan~~ press the associated keybinding within the ship.

# Features
## Basics
- Keypresses will organize each item type in groups by the name of the item
  - Ship cleanup default keybinding is 'm'
	- Items now are organized by value. The futher from the door, the higher the value.
	- Items are grouped in two locations, one group closest to the door are single handed items. The group futher from the door are two handed objects.
  - Closet cleanup default keybinding is 'n'
	- Items in the closet will be staggered slightly to the right of the 'first' item
## Config File Parameters
- CleanupShipKey
	- Keypress used to cleanup the ship only

- CleanupClosetKey
	- Keypress used to cleanup the closet

- ItemGrouping
	- Whether to group the items in tight clusters or to spread them out by value
	- Options
		- [Value]
			- Spread items up the ship by the value
		- [Stack]
			- Keep items stacked on top of one another to reduce clutter
		- Thanks to [jcsnider](https://github.com/jcsnider) for the suggestion

- TwoHandedItemLocation
	- Where to place the two handed items, and inherrently where to place the single handed objects.
	- Options
		- [Front]
			- Two handed items to the front of the ship 
			- One handed items to the back of the ship
		- [Back]
			- Two handed items to the back of the ship 
			- One handed items to the front of the ship
	- Thanks to [jcsnider](https://github.com/jcsnider) and [Artemis-afk](https://github.com/Artemis-afk) for the suggestion

- OrganizationTechniques
	- Options
		- [Loose]
			- Spread items accross the ship from left to right
		- [Tight]
			- Pack the items to the side of the ship with the suit rack.

- ClosetLocationOverride
	- A List of objects to force into the closet on ship cleanup
		- Enter a list of item names in comma separated form to force these items to be placed in the closet instead of on the floor.
	- Thanks to [jcsnider](https://github.com/jcsnider) for the suggestion

- SortingDisabledList
	- Items on this list will be ignored during the sorting process
		- Enter a list of item names in comma separated form to ignore these items during organization.
	- Thanks to [nontheoretical](https://github.com/nontheoretical) for the suggestion



# Changes
- V3.0.1
	- Fixes
		- Fixed a bug where modified scrap levels would teleport outside of ship when sorted by value
	- Features
		- Added a blacklist to sorting (Thanks to [nontheoretical](https://github.com/nontheoretical) for the suggestion)
		 
- V3.0.0
	- Introduced a configuration system that will allow the customization and tailoring of the mod to your liking
	- Began working on ship storage closet functionality now that the ship functionality is ironing out.
	- Added Oranization configuration for stacking / spacing
	- Added closet override for defaulting storage location to closet for a customized list of items

- V2.1.1
  - Adjusted loot placement to avoid suit rack
- V2.1.0 
  - Updated the networking for better tying to client side functions
- V2.0.0
  - Made the system network compatible (definitely buggy)
- V1.1.0
  - The main control of the mod is now through keybinds.
  - Pressing 'm' will organize ship objects
  - Pressing 'n' will organize the ship's closet

# Installation
1. Install BepInEx
2. Run game once with BepInEx installed to generate folders/files
3. Drop the DLL inside of the BepInEx/plugins folder
4. No further steps needed

# Feedback
- Feel free to leave feedback or requests at [my github](https://github.com/bozzobrain/LethalCompanyShipMaid).

# Buy me a coffee
[Buy me a coffee](https://www.buymeacoffee.com/bozzobrain)
