# Changes
- V3.2.7
	- Made a guard for moving items that are inside of the LateGamesUpgrade wheelbarrow due to a reported game crash (Thanks to [GrantisMantis](https://github.com/GrantisMantis)) 
	- Guarded the keybinds when the user is typing in chat (Thanks to [jcsnider](https://github.com/jcsnider))
- V3.2.6
	- Adjusted valid positions to include the whole ship (The rear of the ship was excluded due to assumptions from previous organization methods)
	- Improved the position bounds for edges of ship (front is narrower than the rear of ship)
	- Addressed regex not including items with nubmers in name (Specifically the Magic 7 Ball). 
	- Thanks to [jcsnider](https://github.com/jcsnider) for helping to identify these issues
