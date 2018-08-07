# Alivieska GPS Server

Simple web server that provides positioning information for the Satsuma

## Features
- Lightweight HTTP server
- XML or JSON output
- Command line interface

## Console command
`gps [-p <port>|--port <port>] COMMAND`  
where `COMMAND` is:
- `start`: attempts to create a web server on the default port, or the port specified by the `-p` switch.
- `stop`: halts the web server.
- `restart`: stops and then starts the web server, for example, to change the port.
- `write`: writes the server's content to file in `<mods>/Config/AlivieskaGpsServer`.
- `help`: display the help text.

## Usage
The server runs on TCP port 8080 by default. Depending on the configuration, the hosted content is either an XML or JSON file.
#### XML:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<GpsData>
	<X>1009.916</X>
	<Y>-0.8313327</Y>
	<Z>-738.0518</Z>
	<Heading>30.412</Heading>
	<Speed>30</Speed>
	<Time>0</Time>
</GpsData>
```
#### JSON:
```json
{
	"x":-1637.947,
	"y":0.09099763,
	"z":-479.6247,
	"heading":106.3766,
	"time":0,
	"speed":0
}
```
The `speed` and `time` fields are currently not used.

Alivieska GPS Client is an application that can parse the data and display it on a map.