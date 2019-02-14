# ScreenSnip

A simple Snipping Tool written in C# which lets you take fast Screenshots without needing to choose a file name or directory.


## Usage
Clone `https://github.com/1polygon/ScreenSnip.git` and compile.

Run the executable to take a screenshot.

Click once to select the first point of the area, move the mouse and click again to take the screenshot. 
You can abort with right click or escape.
The folder where the screenshot was saved will open automatically.

## Change save location

By default screenshots are saved in `C:\Users\<user>\Pictures\Screenshots`
For changing where the screenshots are saved just add the desired path as argument.

Example:
`E:\Software\ScreenSnip\ScreenSnip.exe D:\Pictures\Screenshots\`
