# LethalCompanyShipMaid
A mod where the ship and the storage closet is cleaned up when you press the associated keybinding within the ship or use the terminal command.

# Usage
- Activate the cleanup methods by using the terminal or keypresses
	- Terminal Commands 	
		- Type 'cleanup' to cleanup the ship and closet
		- Type 'cleanup storage' to cleanup the storage closet only
	- Keybinds
		- Press 'M' to cleanup the ship and closet
		- Press 'N' to cleanup the storage closet only
- Press 'J' to set an objects placement location (if position overrides enabled)
- Make sure that any players that you play with also have the mod installed for them to see the organized loot change positions.

# Features
## Basics
 - Either Keypresses or Terminal can activate the cleanup routines
	- Key Binds
		- Change these settings in the settings menu (InputUtils) 
		- Ship cleanup default keybinding is 'M'
			- By default all objects not on the 'ClosetLocationOverride' list are organized
			- You can override specific items to not be sorted by adding them to the config setting 'SortingDisabledList'
		- Closet cleanup default keybinding is 'N'
			- Items in the closet will be staggered slightly to the right of the 'first' item
			- Can override which objects get automatically placed in the storage closet using Config setting 'ClosetLocationOverride'
	- Terminal Interface
		- Going to the terminal and entering either 'cleanup' or 'cleanup storage' will organize the ship or the storage closet.
- Setting object locations
	- Set object location default keybinding is 'J'
		- If UseOneHandedPlacementOverrides / UseTwoHandedPlacementOverrides / UseItemTypePlacementOverrides is enabled, pressing this keybind will log the location of the given item for future organization requests
		- Object override locations are stored in the config file and will persist into other game files
- Organization Methods
	- Placement position overrides (3 Config Options)
		- One-Handed Position Override
			- If enabled, all one-handed objects will be placed in a single location
			- Position is set either manually by the config file or by pressing J or whatever keybind you have set by 'SetObjectTypePositionKey'
		- Two-Handed Position Override
			- If enabled, all two-handed objects will be placed in a single location
			- Position is set either manually by the config file or by pressing J or whatever keybind you have set by 'SetObjectTypePositionKey'
		- Item Type Position Override 
			- If enabled, all objects that have override locations will be placed in their designated location
			- Position is set either manually by the config file or by pressing J or whatever keybind you have set by 'SetObjectTypePositionKey'
		- [ItemPlacementOverrideOffsetRotation] and [ItemPlacementOverrideOffsetPosition] allows for items to not be directly placed on each other in PositionOverride modes
	- Randomized placement (Without position overrides)
		- Items now are organized by [Value] by default, the futher from the door, the higher the value. Can also be [Stack]-ed to place all objects of a type directly ontop of each other (See config setting 'ItemGrouping')
		- Items are grouped in two locations, front of ship and back of ship (See config setting 'TwoHandedItemLocation')
		- Items can be strewn accross the ship [Loose]-ly or [Tight]-ly packed on the wall opposite the default location of the Storage Closet (See config setting 'OrganizationTechniques')
- Shotgun Organization
	- Shotguns can be organized into stacks by the amount of shells that are loaded into them using 'OrganizeShotgunByAmmo' config option.
	- Position is set either manually by the config file or by pressing J or whatever keybind you have set by 'SetObjectTypePositionKey'
## Config File Parameters
- Keybinds
	- Now handled in-game by using InputUtils
- Ship Cleanup Configurations
	- UseOnlyTerminal
		- When set to [true] the cleanup keybinding system will not respond to keypresses and only the terminal can be used to activate the mods cleanup functions.
		
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

- Item Type Overrides
	- ClosetLocationOverride
		- A List of objects to force into the closet on ship cleanup
			- Enter a list of item names in comma separated form to force these items to be placed in the closet instead of on the floor.
		- Thanks to [jcsnider](https://github.com/jcsnider) for the suggestion

	- SortingDisabledList
		- Items on this list will be ignored during the sorting process
			- Enter a list of item names in comma separated form to ignore these items during organization.
		- Thanks to [nontheoretical](https://github.com/nontheoretical) for the suggestion

- Position Overrides
	- UseItemTypePlacementOverrides
		- When set to [true] and you have pressed 'J' with an item type in a given location, all future cleanup commands will place this item type in its given location 
		- During bootup all scrap objects that are not on the storage shelve list will be candidates for definiting these location prior to you setting the item locations using 'J'
		- Currently these candidates are not stored into the config file, you must press the keybind to store new locations into the config file to save objects locations if the plugin was not loaded before (tell me to change it if you want this feature)

	- ItemPlacementOverrideLocation
		- The position in the ship where items of a given types will be placed if overrides are enabled for one handed items
		- Configuration will be automatically updated when the keybind is pressed in game (although you could manually enter this information)
		- If someone wants to generate a list of all item names for reference, please send it to me and I'll place it in this readme
		- Format is:
			- "itemname(Clone),posx,posy,posz"
		- For multiple items, continue this format with a comma between item types
			- "itemname(Clone),posx,posy,posz,itemname2(Clone),posx,posy,posz"
		
	- UseTwoHandedPlacementOverrides
		- When set to [true] and you have pressed 'J' with a one handed item in a given location, all future cleanup commands will place all one handed items in its the given location 
		- During bootup all two handed objects that are not on the storage shelve list will be candidates for definiting this location prior to you setting the item locations using 'J'
		- Currently these candidates are not stored into the config file, you must press the keybind to store new locations into the config file to save objects locations if the plugin was not loaded before (tell me to change it if you want this feature)

	- TwoHandedItemPlacementOverrideLocation
		- The position in the ship where Two-Handed Objects will be placed if overrides are enabled for two handed items
		- Configuration will be automatically updated when the keybind is pressed in game
		- Format is:
			- "posx,posy,posz"
	
	- UseOneHandedPlacementOverrides
		- When set to [true] and you have pressed 'J' with a two handed item in a given location, all future cleanup commands will place all two handed items in its the given location 
		- During bootup all one handed objects that are not on the storage shelve list will be candidates for definiting this location prior to you setting the item locations using 'J'
		- Currently these candidates are not stored into the config file, you must press the keybind to store new locations into the config file to save objects locations if the plugin was not loaded before (tell me to change it if you want this feature)
		
	- OneHandedItemPlacementOverrideLocation
		- The position in the ship where One-Handed Objects will be placed if overrides are enabled for one handed items
		- Configuration will be automatically updated when the keybind is pressed in game
		- Format is:
			- "posx,posy,posz"
	- ItemPlacementOverrideOffsetPosition
		- Randomizes the placement locations of items so that they do not stack directly on top of each other
		- Each object has the ability to be offset from the 'Center' location by the some fractional amount in the config setting
		- Format is:
			- "posx,posy,posz"
		
	- ItemPlacementOverrideOffsetRotation
		- Randomizes the placement rotation of items so that they do not stack directly on top of each other
		- Each object has the ability to be rotated from the by the some fractional amount in the config setting
		- Format is:
			- "###.#"
- Shotgun Position Override
	- OrganizeShotgunByAmmo
		- When set to [true] and you have pressed 'J' with a shotgun of a given ammo count in a given location, all future cleanup commands will place this item type in its given location (stored in config file)
		- You can set a location for shotguns with 0 shells loaded, 1 shell and 2 and they will be organized in a pile with other shotguns of the same ammo

	- ShotgunPlacementOverrideLocation
		- The position in the ship where Shotguns of the ammo count will be placed if override is enabled
		- Configuration will be automatically updated when the keybind is pressed in game
		- Format is:
			- "Shotgun(clone),posx,posy,posz,ammo_count"
# Changes
- V4.0.6
	- Added GUI message when object location override is set by keypress
- V4.0.5
	- WARNING: You may want to backup your config prior to launching this version of the app as config settings have changed.
	- Changed configuration settings to use bool instead of string for binary configuration settings [On/Off] 
- V4.0.4
	- Added terminal command option. (Thanks to [UmaLPZ](https://github.com/UmaLPZ) for the suggestion)
	- See [UseOnlyTerminal] configuration setting to disable keybinds and use only the terminal for cleanup
- V4.0.3
	- Added shotgun organization by ammo capacity. (Thanks to [jcsnider](https://github.com/jcsnider) for the suggestion)
	- See new configuration options [OrganizeShotgunByAmmo] and [ShotgunPlacementOverrideLocation]
- V4.0.2
	- Updates to closet location calculation. No longer using door locations for bounds, which should result more consistent organization.  (Thanks to [UmaLPZ](https://github.com/UmaLPZ))
	- Improving handling of a rotated storage closet to position items more intelligently in this case.
- V4.0.1
	- Keybinds are now handled by InputUtils and accessible in the settings menu in-game. (Thanks to [CatsArmy](https://github.com/CatsArmy))
	- Now preventing the organization of items while the ship is moving (!shipHasLanded). Should allow organization while not on a ship (inShipPhase). (Thanks to [Jonteiro](https://github.com/Jonteiro))
- V4.0.0
	- Introduced configuration settings for randomizing both position and rotation on object cleanups (Thanks to [Jonteiro](https://github.com/Jonteiro))
	- See configuration settings [ItemPlacementOverrideOffsetRotation] and [ItemPlacementOverrideOffsetPosition]
	- WARNING: This will break compatibility between previous versions of the plugin. Make sure all players are upto date on the plugin to not have issues.

- V3.2.7
	- Made a guard for moving items that are inside of the LateGamesUpgrade wheelbarrow due to a reported game crash (Thanks to [GrantisMantis](https://github.com/GrantisMantis)) 
	- Guarded the keybinds when the user is typing in chat (Thanks to [jcsnider](https://github.com/jcsnider))
- V3.2.6
	- Adjusted valid positions to include the whole ship (The rear of the ship was excluded due to assumptions from previous organization methods)
	- Improved the position bounds for edges of ship (front is narrower than the rear of ship)
	- Addressed regex not including items with nubmers in name (Specifically the Magic 7 Ball). 
	- Thanks to [jcsnider](https://github.com/jcsnider) for helping to identify these issues
- V3.2.5
	- Overhauled some backend networking things to be compliant around either LC-API or other mods that were breaking things
	- Thanks to [HiiJustin](https://github.com/HiiJustin) for the relaying information
- V3.2.4
	- Introduced the configuration file handling of item positions for all placement overrides
	- Now placements locations for all overrides are stored in the config file and will persist across game resets and different save files.
	- Thanks to [jcsnider](https://github.com/jcsnider) for the pushing to develop this feature
- V3.2.3
	- Patched issue with one handed object location settings being incorrectly stored
	- Thanks to [jcsnider](https://github.com/jcsnider) for the finding this issue
- V3.2.2
	- Update readme for additional information and fixed old descriptions 
- V3.2.1
	- A minor patch fix for possible conflicting configuration issues
- V3.2.0
	- Added custom placement options (set locations with 'J' or whatever key you bind)
		- Place all two handed objects in the same location using UseTwoHandedPlacementOverrides=[Enabled]
		- Place all one handed objects in the same location using UseOneHandedPlacementOverrides=[Enabled]
		- Place all items of a given type in the same location using UseItemTypePlacementOverrides=[Enabled]	
- V3.1.1
	- Fixed an issue where loot was landing either on the storage closet or in the suit rack area 
- V3.1.0
	- Improved the handling of the bounds of the ship and storage closet to prevent items from clipping out of the ship.  
	- Thanks to [nontheoretical](https://github.com/nontheoretical) for finding the issue.
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
4. Download LethalCompanyInputUtils ([github](https://github.com/Rune580/LethalCompanyInputUtils)/[Thunderstore](https://thunderstore.io/c/lethal-company/p/Rune580/LethalCompany_InputUtils/)) and drop the DLL inside of the BepInEx/plugins folder 
5. No further steps needed

# Feedback
- Feel free to leave feedback or requests at [my github](https://github.com/bozzobrain/LethalCompanyShipMaid).

# Buy me a beer
[Buy me a beer](https://www.buymeacoffee.com/bozzobrain)
