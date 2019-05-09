namespace HomeRun.Net
{
    using UnityEngine;
    using System.Collections;

    // This class listens for Input events to shoot a ball, and also notifies the P2PManager when
    // ball or scores needs to be synchronized to remote players.
    public class LocalPlayer : Player
    {
        void Start()
        {
            base.BallSelector = PlatformManager.Instance.LocalBallSelector;
        }
    }
}
