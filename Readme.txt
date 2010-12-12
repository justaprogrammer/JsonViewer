The JSON View package is a set of 3 viewers available in the following flavors:
1) A standalone viewer - JsonView.exe
2) A plugin for Fiddler 2 (http://www.fiddler2.com/) - FiddlerJsonViewer.dll
3) A visualizer for Viusal Studio 2005  - JsonVisualizer.dll

The viewer supports plugins to allow you to customize the way JSON objects are displayed. Sample plugins 
are provided within the source.

Installation
============

The archive contains the following directories:
\JsonView
\Fiddler
\Visualizer

- To use the standalone viewer, run JsonView.exe from \JsonView
- To use the Fiddler2 plugin, copy the files from the \Fiddler directory to fiddler's \Inspectors 
  directory and add the following to the <runtime> section of the fiddler.exe.config:
      <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <probing privatePath="Inspectors" />
    </assemblyBinding>
- To use the Visual Studio Visualizer, copy the JsonVisualizer.dll to the Visual Studio Visualizers 
  directory (usually under \My Documents\Visual Studio 2005\Visualizers) and copy the following files
  to the IDE directory of Visual Studio (Where devenv.exe is located - <Visual Studio>\Common7\IDE):
  - JsonViewer.dll
  - JsonViewer.dll.config
  - Newtonsoft.Json.dll
