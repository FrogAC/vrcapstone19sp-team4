## VRCapstone-19SP-Team4 Blog

This page is hosting on [https://uwrealitylab.github.io/vrcapstone19sp-team4](https://uwrealitylab.github.io/vrcapstone19sp-team4)

### Project Proposal is available here: 
[Project Proposal](https://docs.google.com/document/d/1V69Ls4ID84_nt9mg4wguqv0sTw7dM2adHBnqVyagK6U)

### Product Requirements Document is available here:
[PRD](https://docs.google.com/document/d/1e8s-7eEA75yj_aTk2TJxJ6-0OESeN0v3vhc-tJBpVM4)

### Week 1
We spent most of week 1 exploring the various pieces of hardware and software that we have at our disposal. We were able
narrow in on a VR platform after considering the strengths and weaknesses of each of the different headsets. We also learned
about basic software packages used to create content in VR.


### Week 2
After discussion we aim for a baseball game that focus on physics and multi-player(ideally for quest).
No Updates on code yet.
Next week we'll finish proposal, pitch the idea, divide into groups(maybe) and start working on core mechanics. 
Particularly we need to figure out how to project the player's movement into a set of valid moves in-game.

### Week 3
We have structured our ideas into a visible goal and have gotten to work on prototyping. We were able to create a working prototype of bat and ball interactions, creating different trajectories and forces when the ball comes into contact with different areas of the bat.

We also created a path system to control the trajectory of the ball when pitched. We are currently able to emulate a few different standard baseball pitches, and we are looking to get some of the more bizarre ones working by next week.

We have found a good networking code base that looks to be a promising place to start to achieve multiplayer. We are continuing to explore that and have simple multiplayer tests done next week. The most important thing to emphasize for multiplayer is to make sure that the ball moves the same way across all players' views.

Asset Creation is coming along faster than expected. Models for bases and stadium assets have been completed. A texture for the field has been completed using a full physically based shading network. By next week we hope to have a testing stadium environment setup in engine in order to get the right feel for scale. The environment for now may consist of gray models, while textures for them are being produced. The next step is to also start exploring character solutions, and whether or not it is worthwhile to create our own or use the asset store.

### Week 4
We have assembled our simulation for hitting and pitching into a demo scene. The pitches are coming along well, and most of our work on those will be to make it robust and to allow give the user the right feel. The next step for piching is to allow the user to set the pitch type based on controller input. The batting mechanics are coming along. The user is able to pick up a bat and try to swing it at the balls. Currently we are only able to deflect it, but our goal for next week is to be able to hit it accurately. 

Our networking was set back slightly due to the new Unity release, which did an overhaul of the High Level Networking API. Our goal for next week is to be able to get two players in VR interacting in the same networked space.

In terms of asset creation, we have finished a simple bat and textture design. Now, any new bats that need to be made can simply be done by retexturing the bat model. The field texture was inserted into Unity in order to do scale tests. Initial character design sketches have been produced, and modelling will start this weekend. Our goal for next week is to have our basic mechanics setup in a stadium environment for the user to test. Sound design exploration has also begun.

### Week 5
##### NetWorking Blog
Since unity depreacate UNET in their 2019 pipeline, through the weeks we tried the new unity Multiplayer, Photon, and Oculus Native Platform SDK. The new multiplayer is in develpment, use ECS and lacks of documentations; Photon is a 3rd party networking solution, great in scalability but requires server and requires some extra-cost. Therefore we choose Oculus Platform, which provides convenient and simple Matchmaking and P2P networking api. Hopefully Oculus will update their store interface soon -- for now we are debugging using rift. The SDK performs pretty solid on oculus GO so I'm not expecting too much problems while swtiching platforms in the future.

We were able to get a demo scene working with 2 Oculus Rift headsets using the Oculus P2P framework, which was just as sample demo created by Oculus which allowed players to have a basketball shooting competition over a networked connection. We could not get the networked demo working on the Quest yet due to Oculus account issues which are currently being looked into with the help of the TAs.

![alt text](https://github.com/UWRealityLab/vrcapstone19sp-team4/blob/master/Screenshots/Week5/NetDemo.png "NetDemo")

##### Asset Creation
In terms of asset creation, we have decided to use a Stadium Asset Pack as a base for other potential stadiums. The current sadiums look decent, but they are quite low-poly, as most of the assets are transparent planes. Time permitting we would like to add a little more geometry to to make it look more pronounced, as the planar approach can be visually distracting at times, and as seen below, it causes a lot of hard shadows and lighting artifacts.

We have an ititial character design sketches, which will most likely be very low poly, with textured facial fieatures as opposed to modeled geometry to save time on animations. The focus for this week will be on making the glove for the pitcher, and assets that required for specific game modes, such as vfx shaders for home run derby, targets for target practice, and zombies for our zombie survival mode. A stretch goal we also have is to make a locker room that serves as the start menu scene.

![alt text](/Screenshots/Week5/Bat.PNG)

##### Demo Scene
We have created a demo scene using the stadium pack, the bat and the piching script. Currently the batter controls the pitching mechanic with their left hand controller, using the X and Y buttons to cycle through different pitches. They can also use the left thumbstick to move around and adjust their position in relation to the batters box. They can grab the bat with with the righthand middle finger grip and must keep that button pressed in order to hold onto it. They can trigger the pitch with the left controller and swing the bat with the right handed one.

![alt text](/Screenshots/Week5/CurveballCrop.png)

![alt text](/Screenshots/Week5/FastballCrop.png)

![alt text](/Screenshots/Week5/SpiralballCrop.png)

##### Gameplay Goals
For next week our goals are to turn the demo scene into actual gameplay modes. Our minimum goal is to have a home run derby mode where with a timer, where you have to see how many home runs you can hit. We will have an automatic pitcher and some sort of ui that shows the time you have left and the number that you have hit.

### Week 6
This week the team is focusing on creating the playable demo that meets MVP as well as basis of final goal. Terrell and Stephen worked on SinglePlayer Modes and mechniques. Andrew worked on Asset and Daoyi worked on the networking solution for Rift.

##### Daoyi:Network
Finish the Networking base frame for 2 players. By establishing a peer-to-peer connection players can have shared Head+Bat/PitcherGlove+Ball movements with a constant refreshing counts of >90/second. There's no distinguishible latency using Rift and lab Ethernet. Also add some particle effect and plan to contributes more FXs in the future.
![alt text](/Screenshots/Week6/dao_w6_0.PNG)
![alt text](/Screenshots/Week6/dao_w6_1.gif)

For next week(s) I would focus on optimizition connection to ensure "quest ready": for example now the priority of physics simulation is given to the Batter side which is extreamly unrealible and sensitive to network flaws. It is also a great idea to sync only on impact/collision points since ball movements are determinent. This will invlove more collabrates with Terrell's works.

##### Terrell and Stephen - Batting Mechanic:
Early in the week, I worked with Stephen in order to make improvements to the batting mechanics. Previously, the ball would be launched along one of several fixed paths whenever the ball hit the bat, but now the ball can be hit into any direction. To do this, we moved the logic for hitting the ball from the bat to the ball so whenever the ball hits the bat and breaks out of its animation, it also launches itself depending on the angle that it hit the bat at. The speed of the hit is driven by how hard the player swings the bat, but we have a parameter that we can use to adjust the strength of the hit, so we can tweak that value later in order to make it easier or harder to hit homeruns. We also added haptic feedback for when the bat hits the ball. Now the controllers vibrate depending on how hard the hit was.

##### Terrell - Target Practice:
In preparation for our mid-quarter progress report, I worked on creating the target practice gamemode to show off the improvements that we made to the batting. Since we have already been using a scene with an auto-pitcher and batter for testing batting mechanics, the main task to get this game-mode working was creating targets that disappeared when hit and a way of tracking information about how many targets have been hit. For the targets, I made cylindrical zones outside the field to track where home runs were hit, and used spherical targets for places on and above the field. To visualize the targets, I used cylinders that faded away as they went up away from the ground and a particle system that created a rotating ring around the spherical targets.

##### Terrell - Plans for next week:
We want to work on the zombie game mode next, so I will be helping with that by making it so that balls explode on impact to deal damage to nearby zombies. Also, I want to work on the target practice mode to improve its gameplay elements. Right now everything is fairly basic: none of the targets move, and nothing happens whenever all of them have been hit. To improve this, I can make the targets move across the field, disappear after a set time, and keep track of a score that gets shown at the end of the game. By doing this, we can create a high-score system to give a clear sense of progress and make replaying the gamemode more engaging.

Additionally, I want to reorganize the pitching system so that it is easier to add new pitches and adjust existing ones, as well as create a new type of pitch. Right now, the spiral ball is the only special pitch, so having either a ball that wiggles around or zig-zags would be interesting.

### Stephen - Plans for next week:
Setting up the grey box scene for the zombie mode. Specfically, setting up boxes that represent zombies. Having those zombies move at variable rates of speed torwards the player to try to attack and defeat the player. I will give the zombies an internal life bar that will be depleted by being hit by the bat or getting hit with a grenade. I will be working with Terrell to combine our scenes together into a working game mode. Once the basics of the game mode are complete I will work on making quality of life adjustments.


### Week 7
This week we continue on making new game modes and new assets while improving existing gameplays.

##### Daoyi: Network
Work on porting the network to the Quest after releasing next week. This is done by optimization the messages send between the p2p connections and let both side infer actions. I still need to wait until 21st to see if Oculus update the Quest Api -- for now Rift is still the backup device for network demo.
Next week I will finish the networking part and move to assets -- helping polish the final looks.

###### Terrell - Grenades
This week I worked on the zombies mode in order to get our grenades working properly and started prototyping a simple in-game menu for changing between scenes. The grenade has been implemented as a component added to the baseballs that are thrown to the player, so we have a lot of flexibility with how we tweak the grenades in the future. They work by arming the ball to explode when it hits the bat, then actually exploding once it hits the ground or a zombie. The explosion works by checking for zombies within the explosion radius and currently just destroys the zombie, but in the future we can calculate damage done to zombies based on how far from the center of the explosion they were.

###### Terrell - Simple UI
The in-game menu is essentially a 3d panel that the player can bring up in front of them that has configurable buttons that the player can press. Right now, buttons are considered to be pressed as soon as the player’s hand touches them, so having some sort of delay before triggering or even allowing for the button to actually be depressed by the player’s hand would be the next step in improving the presentation. This UI system became important for our testing because we were finding issues when we tried to build our game and run it outside of Unity; many of our scenes seemed to crash on load and only the simplest ones were able to be built and played. The most unique crash of these was the zombie mode where the build would crash only when a ball had been hit and was about to explode. 

##### Terrell - Plans for next week
Debugging build issues will be something that I work on in this coming week. The problem isn’t crippling in the sense that it doesn’t seem to have an impact on our development right now, but will need to be solved sooner rather than later since we are nearing the end of the quarter and the fix that we find for the crashes may require us to reorganize large parts of the project. I also want to make sure that a new pitch gets added to the game since I didn’t add one this week.

### Week 8
This week our focus is on making the Quest work with the networking setup and improving the P2P information transfer to ensure the networking is running effectively and smoothly. Secondly, we are working bug testing ensuring we minimize the amount errors that occur during gameplay.

### Stephen - Network and Ball physics:
Work with Daoyi to determine the changes that need to be made to the code base to ensure a singular working game across the Rift and the Quest. The major focus of this portion is how we are going to get the ball physics calculations to handled across the machines in a P2P manner while respecting the latency that we will inevitably face with the non-corded Quest. Once these bugs are handled I will work with Terrell to make sure we have smooth transitions between scenes and put the finishing touches on the gameplay mechanics.
