# particle-sim-GRAVITY
This is a particle-sim, 2 versions on this, one that doesn't work but should be better if I got it to work, and the other that works but doesn't have the fancy algorithm :( - first time making a particle sim, learned a lot but a lot to improve.

This particle simulator simulates how particles interact with each other through the force of gravity, and if the particles touch they combine. All the particles are just represented by spheres becasue those were the easiest to play around with, especially when calculating if 2 particles are touching.

The V1 version has a form of continuous collision detection (CCD), which pretty much tried to check if 2 particles would collide within the current frame and the next and act accordingly, however there is some random annoying bug somewhere which means it isn't fully working and particles try to combine when they aren't meant to. Let alone the other bug which occurs when there are loads of particles and it creates some kind of infinite loop that increases their size continuosly and they "explode". There is also a border where particles get "bounced" back in so they don't fly off to who knows where.

In the V2 version I changed it so that it would detect if the particles are touching by simply seeing if they are overlapping, solved all the bugs but in the future if I want to render loads of particles this may not be fully viable, but it works for a smaller amount of particles. Also made it delete particles that went past a certain border, which is a little more realistic then bouncing them back in. I also made the border of where the particles would get deleted follow where the centre of mass is. I also added a feature in this one where you can speed up the simulation by changing something with deltaTime, I don't fully understand how that works but that how chatGPT did it so I am assuming it is fine, and also chatGPT coded a script for the camera so by pressing c, you can attach the camera to where the centre of mass is.

Future (probably won't ever do this): add extra features like if the particles are fast enoguht they split into more smaller particles when crashing into eachother instead of jsut combining; use more of the CPU/GPU; add the Barnes-Hut algorithm so I can simulate more particles.

How to use:
Drop the camera script onto the camera,
Drop the particle simulator script and the particle spawner script onto an empty object (name empty object simulation manager if you want),
Create a sphere (ideally 1x1 scale but I don't think it matters) (you can also name it particle),
Where an object needs to be attached to a script in the Inspector tab attach the sphere object, if a script needs to be attached, attach the particle simulator script,
Play around with the settings and do whatever works.
