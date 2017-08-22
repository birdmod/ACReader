# ACReader
A tool to browse anime by season and year based on Anilist API.

# Motivation
This happened in april 2017.
I wanted to make C# and F# work along. High objective !

I knew this was possible but I wanted to experiment it with 
* a F# "backend"
* leverage on the Type Providers feature to process data from an API 
* display on a WPF client
* **no lag / block on the UI**

The idea of this tool was a bit random but the most important is that the technical needs met the usage !

# Usage
You can run this tool but need your API key from the Anilist website. I did not publish my own key ^^.
For the build, you can open the solution in Visual Studio, link the dependency to FSharp.Data with nuget and this will do the job ! 

# Versions
* F# 4.0 (FSharp.Core 4.4.0.0)
* .NET 4.5.2
* FSharp.Data library

Basically the connection with credentials is done on startup, use the top bar to select a season and 
a year and submit to get the results. This project has an added value because you can add comments 
for each anime such as "Whoah, totally loved it !". These comments are stored in a .csv file

Here is a result : 

![Screenshot](usage.png?raw=true)
