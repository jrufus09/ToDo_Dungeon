
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
- new board, new list, new item functions exist in BoardDataManager
- hash set to quick lookup/check unique board names

### 24-Mar-2025
- new board only allows unique board names. (fixed)
- added board icon container + made board icon prefabs

### 26-Mar-2025
- icons now display in container successfully
- refreshes when opened and also when a new board is made

### 29-Mar-2025
- persistent UI scene created (the nav menu, basically)

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

### 17-Apr-2025
- fixed, button container scaling issue

### 18-Apr-2025
- might be opportune to turn the popup input into a class. that way I can keep the switch statements in popupmanager
- virtual (parent) -> override (child)
- popupmanager switches between cases + there is a popuptextinput parent class where newboard, newlist, etc. inherit from
- on the unity editor end, new board, new list, etc. are prefab variants now
- currentlyOpenBoard exists to keep what's currently open at runtime; list/item adding classes need this info to work out what's unique. all info then goes back to BoardDataManager
- rehauled popup system to account for new list/board; all popups are now variants of PopupTextInput prefab

### 22-Apr-2025
- new list button does not work at all (anymore). fixed
- display list icons in BoardView
- updating icon scripts to use setter methods as opposed to Initialize()
- new errors:
    1. +new list makes a new board popup instead and
    2. +new list doesn't refresh the content pane and
    3. checking for unique list names does not work

### 23-Apr-2025
- rearranged BoardView to be singleton for easier access of its contentpane (also means won't have multiple of one anyway)
- rewrote SceneLoader to make BoardView active when called.
- UI objects in BoardView will not show up during runtime and I have no idea why. they are displaying to screen just fine.
- made a PrintSceneChildren button because WHERE IS MY UI

### 24-Apr-2025
- found the UI (i'm silly, it was in DontDestroyOnLoad)
- I think Instantiate(prefab, parent) is not working how i intended because passing Transform into this directly makes it think parent is null
- List icons now refresh and display as intended; fixed all new errors (make new list, refreshing, unique list per board) 
- Reorganised ListIcon prefabs

### 25-Apr-2025
- Adding new task is programmatically possible
- made task prefab and now new task button has stopped working

### 26-Apr-2025
- UI has broken again because BoardView only goes over and doesn't take inputs. I think we're gonna have to make it active after all
- made a ContentArea class so I can find Viewport>Content easier in scene tree. this way we can "find object of type" then narrow down by enum instead of jumping around passing confusing references
- BoardView exit no longer works :)))) new solution: set BoardView inactive before unloading

### 27-Apr-2025
- BoardView unloading is just broken and I can't work out how to fix it and I'm stressing bad so let's just toggle its invisibility
- rewriting DisableInteraction script so it can be filtered/toggled like what happened with the ContentArea class
- the script would need to also disable cameras at will so basically all canvasgroups need their cameras to be children of whatever disableinteraction script is attached to. so, ideally:
    SceneName [CanvasGroup, DisableInteraction]
        - UI [Canvas, ...]
        - Camera [Camera]
        - Background [Image...]
Current bug that exists (still): stuff inside ListIcon doesn't register clicks.

### 28-Apr-2025
- Still stuck on the buttons within the lists not being reached. 
- due to viewport issues and after many hours I have decided to simply scroll across with the scrollbar instead.
- outer viewport would catch all the gestures and now allow inner viewport gestures. even when inner viewport gestures logged, you couldn't press buttons (or interact within the list at all)
- I got it working but i can't recreate it. what. 

## Features for consideration:
- task text can be changed
- boards, lists and tasks can be deleted
