# Notepad-RT
## Real-time display using Notepad

Inspired by the bloke running Doom in Notepad, I thought I'd take a look to see how it might have been done.

My first approach was to create a ASCII screenbuffer and use the clipboard to copy it to Notepad each time a frame needed updating.  This method has two drawbacks:
* It's slow
* If Notepad loses focus, then whatever application next gets it receives buckets of stuff from the clipboard.

The second attempt uses WM_SETTEXT directly to set the contents of Notepad's edit control.  This runs far faster (~150 fps on my desktop machine) and behaves far better if Notepad ceases to have focus.


