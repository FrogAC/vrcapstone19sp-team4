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
##### Gameplay
..... 

##### Daoyi:Network
Finish the Networking base frame for 2 players. By establishing a peer-to-peer connection players can have shared Head+Bat/PitcherGlove+Ball movements with a constant refreshing counts of >90/second. There's no distinguishible latency using Rift and lab Ethernet. Also add some particle effect and plan to contributes more FXs in the future.
For next week(s) I would focus on optimizition connection to ensure "quest ready": for example now the priority of physics simulation is given to the Batter side which is extreamly unrealible and sensitive to network flaws. It is also a great idea to sync only on impact/collision points since ball movements are determinent. This will invlove more collabrates with Terrell's works.

# Asset
.....

To sum up at this point we are still working on seperate parts as decided weeks before ....

