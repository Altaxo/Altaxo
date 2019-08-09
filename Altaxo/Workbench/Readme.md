# Rename later on #

- AbstractViewContent -> AbstractViewController (Non-Gui-Class)




# Important classes in AvalonDock #

- LayoutAnchorableItem: is the Gui class that hosts one tool user control
  - the content of this item is in its Content property
	- the model of type LayoutAnchorable is in its Model property
  - CanHide (bool) property
  -
- LayoutAnchorable is the nonGui class for one tool window
  - it has a content property, too, which is the viewmodel of the tool window
