# NeoScavHelperTool

This tool is intended to use with NeoScavenger game for now as viewer of in-game items.
It will need to have valid XML files on all mods or else an exception will be thrown.

### This is still unfinished and so far:
- It will display barterhexes
- It will display forbiddenhexes
- It will display hextypes
- It will display images
- It will display maps

### Bellow is a list with XML fixes I had to make on some mods:
+ DevKit\BBCAddOn
  - Line 1494: escaped '&' to '&amp;'
  - Line 2574: escaped '&' to '&amp;'
  - Line 2970: escaped '&' to '&amp;'
  - Line 3672: escaped '&' to '&amp;'
  - Line 3870: escaped '&' to '&amp;'

+ DevKit\DevKitAddOn
	- Line 253: escaped '&' to '&amp;'
	- Line 344: escaped '&' to '&amp;'
	- Line 367: escaped '&' to '&amp;'
	- Line 390: escaped '&' to '&amp;'
	- Line 495: removed extra '>' on the closing tag

+ M(m)MoD\M(m)MoD
	- Line 20008: escaped '&' to '&amp;'
	- Line 20047: escaped '&' to '&amp;'

+ Overhaul\AddOn
	- Line 2124: removed extra '24031' after closing tag
	
+ Shouldered\Patch_MmMoD
	- Line 39: escaped '&' to '&amp;'
	- Line 78: escaped '&' to '&amp;'
	- Line 117: escaped '&' to '&amp;'
