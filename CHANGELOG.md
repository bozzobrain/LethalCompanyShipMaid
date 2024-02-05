# Changes
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
