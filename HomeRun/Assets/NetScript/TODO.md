
Add hit head effect

Note -> Tracking Ball collision instead assert time for next ball selecton interval

FIX -> destroy in client BallSpawn
Quest -> remove ParticleSystem Controller

Ambigu -> namespace HomeRun should be Homerun.settings

Danger -> CreateBall use implicit selection index
Danger -> PlayerData.activeball grow infi

Interpolate Normal, swing direction

Pitcher Loop:
    Timing  -> throw -> Hit Response
                     -> !Hit Response

DONE:
Remove RB in P2PNetWorkBall
Universal Effect Controller
Net(Reliable):
    ID: pitcher's Instance ID

    NetWorkBallState { Static, Throw, Hit }

    Pitcher SendBallSpawnMsg(Instance ID, type)
    Batter ReceiveBallSpawnMsg : spawn and attached to Glove

    Pitcher SendBallThrowMsg (Instance ID, pos, vel)
    Batter ReciveBallThrowMsg :  Throw (pos, Lin)

//  Ignore collision on pitcher side
    Batter SendBallHitMsg (InstanceID, pos, vel)
    Pitcher ReceiveBallHitMsg
    

