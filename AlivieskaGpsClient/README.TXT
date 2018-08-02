Alivieska GPS Client
Developed by Wampa842

To be used in conjunction with AlivieskaGpsServer [(see below)](#gps-server)

This is an application that processes and displays data received from AlivieskaGpsServer, as well as a map of the Alivieska region. It does so by periodically requesting data from a specified network address (by default, http://localhost:8080/). Ideally, AlivieskaGpsServer responds by collating relevant data (the Satsuma's position, heading and displayed speed) into an XML file accessible through HTTP on the specified port (8080 by default). The client then updates the map image to display the new position.

The application can be used on the same computer (localhost), or inside the local network. I do not advise using it across the internet since it uses the unencrypted HTTP protocol (and realistically, why would you).

GPS Server
This application was designed to be used exclusively in My Summer Car with AlivieskaGpsServer, which you should've downloaded in the same archive as this program.

Requirements
- .NET Framework 4.5
- AlivieskaGpsServer
    - My Summer Car
	- MSC Mod Loader