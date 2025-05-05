
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
- fixed, prefab settings were messing up
- Fixed also some layout issues, Popup issues, and "task not being made" issues
- Made prefab and variants for D-pad buttons

## Features for consideration:
- "there are no lists(/boards) here. why don't you make one?"
- task text can be changed
- boards, lists and tasks can be deleted
- beginning to think every task, list, board needs a unique ID :/ that way it's easier to remove

### 30-April-2025
- whole dungeon generation system complete: simple random room placement with dot leg algorithm to join room centres
- player movement with buttons done
- camera follows player
- tile collisions are on the fence

### 1-May-2025
- fixed tile collisions + weird stickiness on walls
- made player move around to open up fog
- made health system
- fixed alignment issues with tilemap and player

### 2-May-2025
- started on enemy
- made TurnManager
- made DungeonUI / for handling button inputs
- made Enemy array, changed it to hashset, then changed it to a dictionary
- made all this EnemyHandler generation code including the reference sheet and dictionary updating and then just never made a single enemy prefab
- I made a whole scriptable object dictionary and it was cool
- editor is now not allowing me to run play mode without freezing and crashing.
- now need half my scripts to subscribe to "dungeon generation complete"

### 3-May-2025
- trying to work out why my navmeshes don't exist only to find out that unity's system is built for 3D and absolutely won't work with 2D projects and that I need to install another package from an external github.
thanks man.
https://github.com/h8man/NavMeshPlus/wiki/HOW-TO
- nevermind it really doesn't want to work and I don't have all year to fix it. found another package to use
- made a helper class for myself to convert transform.position to cell coordinates, as well as convert pathfinding
- it's called Cell and it's in the Scripts/Game/Helper folder
- fixed so that enemies now spawn successfully
- simple synchronously-generated paths (A*) integrated

### 4-May-2025
- there is a problem with enemy collisions being kinematic and the player doesn't collide with them if that's the case.
- it's time for manual collision detection :(

### 5-May-2025
- updating DungeonUI to remember what button is left, right, etc. instead of simply keeping them in a random array
- updating to disable movement in a direction that is blocked
- new bug! enemy can spawn in player's current location and trap them
- updated spawning system to allow player safe area
- made attack button that enables when enemy is nearby
- resolving turnamanager issues which include: infinitely giving enemy turns