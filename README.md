# Some mods for My Summer Car
### Unflip your ride
A console command to deal with heavy vehicles that went tits-up. Kind of a cheat. Also kind of shit since it simply resets the car's rotation and places it 1m higher. It won't get you out of a ditch or lake.

Usage: `unflip <number> [angle]`  
where `<number>` is one of the following:
- 0: Satsuma
- 1: Gifu (sewage truck)
- 2: Hayosiko (van)
- 3: Ruscko (rusty ventti reward)
- 4: Ferndale (Fleetari's muscle car)
- 5: Kekmet (tractor)

`[angle]` can be:
- an angle from north
- north/east/south/west or n/e/s/w
- empty

If `[angle]` is omitted, the vehicle will keep its original heading.

If the command is called without or with wrong arguments, it'll display the help text.

**Warning: use at your own risk.** Using this command will reset the car's rotation *instantaneously!* It might get damaged or stuck in the ground, especially the Gifu. If the trailer is connected to the Kekmet, it'll flail uncontrollably.  
If there are objects inside the vehicle, physics might freak out when they clip into each other. Cargo will almost certainly be launched into low orbit.

---
### Item cleanup (WIP)
A console command that removes certain items (e.g. empty bottles, food) from the world to improve (?) performance.

---
### Alivieska GPS (WIP)
A tiny web server that hosts a JSON file containing the Satsuma's position and the game's time of day, and a web application that'll hopefully connect to it and display the player's position on a map.

---
#### Usage
1. Make sure the latest version of [MSCModLoader](https://github.com/piotrulos/MSCModLoader) is installed.
2. Download and extract the archive to MSC's `Mods` folder.

The in-game console can be opened using `~`.
