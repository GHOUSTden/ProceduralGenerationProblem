# My Solution On Procedural Dungeon Generation In Unity

So, this is my attempt at creating procedural dungeon generation on Unity using Delaunay Triangulation, MST, Cycles, A* for pathfinding. 

> [!NOTE]
> This project was built primarily for my own game development needs, but is structured to be easily understood, extended, and used by other developers!

---

## What I'm Trying To Solve

Even if this project is especially for my game, i will make here a lot of thing that can be useful for everyone, so what i'm actually trying to solve?
1. **Multi-floor Dungeon Generation** - I actually found some useful dungeon generators that have that, but i wanted to try doing something myself and also make this customizable, so in case if i wanted to have only 2 floors i can just tell generator to do that.
2. **Zones** - I already found one person that did that and i really liked that idea and i wanted to make it myself with my own modifications in it.
3. **High Customizability** - By that i mean that dungeon can generate what i wanted it to generate, like a forest or caves like location using Cellular Automata, also with the ability to customize their behaviour, as a indoors locations generation, etc.

---

## Prerequisites
For this project I was using Unity 6.6 for now, but later i planned to use Unity 7.0

---

## Sources & Inspiration
A huge thank you to the creators and resources that made this project possible:
[1] https://vazgriz.com/119/procedurally-generated-dungeons/

[2] https://github.com/vazgriz/DungeonGenerator

[3] https://github.com/dev-rygy/labyrinth-algorithm

[4] https://en.wikipedia.org/wiki/Delaunay_triangulation

[5] https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
