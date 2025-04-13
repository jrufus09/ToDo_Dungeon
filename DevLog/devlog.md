
### 08-Feb-2025
- solved repo syncing issues.

### 18-Feb-2025
- PC fixed

### 20-Feb-2025
- spent last two days trying to work out why unity doesn't recognise touch input.
- wherever there is a canvas component, there should be a graphic raycaster alongside it.

### 26-Feb-2025
- made a board data type class acts as a baseline container + board data manager class
- popup manager to handle UI; attach all prefabs to this and ensure (the children) are scaled at 1

### 13-Mar-2025
- program checks for number of boards in save data at start up.
- made autosave function.

### 18-Mar-2025
- board name can only confirm when string is not empty

### 21-Mar-2025
- fixes to above

### 23-Mar-2025
- if no save data exists, makes a new one; if save data does exist, loads boards
- new board, new list, new item functions exist
- hash set to quick lookup/check unique board names

### 24-Mar-2025
- new board only allows unique board names. (fixed)
- added board icon container + made board icon prefabs

### 26-Mar-2025
- icons now display in container successfully

### 29-Mar-2025
- persistent UI scene

### 31-Mar-2025
- popup manager is also singleton. it exists on persistent UI:
- SceneLoader script draws persistent UI (nav bar) over current scene.
- on Additive scene loading: keep one "Main" scene with camera and event manager, but remove these from every scene that's added over, else unity gets annoyed

### 01-Apr-2025
- Added further customisation to PopupTextInput prefab so that it can be used for board, list, etc
- New(board/list/item) button -> Popup Manager -> popup prefab

### 04-Apr-2025
- open board in new scene

### 05-Apr-2025
- trying to exit BoardView
- bug: clicking icon opens boardview but it's not visible;
- bug: possible to open multiple at once

### 12-Apr-2025
- moved event system to persistent UI
- board view is currently additive which causes problems when pressing a button whilst there's another button below it
- tried to do the overlay thing that works with popup... it doesn't work
- gave up and made a custom disabler script 
