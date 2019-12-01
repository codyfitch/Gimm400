using System;

public class Utilities
{
    //Connecting to a Photon server for online multiplayer
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();  //Connect to Photon server using settings set up in inspector. Uses ID key to connect to correct server
    }

    //When connected to the Photon server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to the Photon master server");
        PhotonNetwork.AutomaticallySyncScene = true; //Syncs the scene for all players
        startButton.SetActive(true); //When connected to server, display start button which will connect to a room
    }

    //When button is clicked after connected to server, join a random room on the server
    public void OnStartButtonClick()
    {
        startButton.SetActive(false); //After the button is clicked, we no longer need it so we set it inactive
        cancelButton.SetActive(true); //Display a cancel button in case the player wants to back out
        PhotonNetwork.JoinRandomRoom(); //Join a random room on the server
    }

    //After connected to server, how to create a room
    void CreateRoom()
    {
        int randomRoomName = Random.Range(0, 10000); //Assigns a random number to use as the room name
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 }; //Set up different room options here
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps); //Create room using the random number as a name and using room options we set up before
    }

    //Photon player setup
    void PlayerSetup()
    {
        PV = GetComponent<PhotonView>(); //Find the PhotonView component on the player prefab
        int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length); //Randomly pick a spawn point

        if (PV.IsMine) //Check if it's the local player (your local machine)
        {
            //Spawn player prefab in the path specified. Spawn them on the spawn location we specified (randomly in this case)
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.GS.spawnPoints[spawnPicker].position, GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);
        }
        //NOTE: AI and objects in the scene that aren't controlled by players just need PhotonView and PhotonTransformView components on them for their transforms to be tracked over the server
    }

    //Player movement over Photon
    private void Update()
    {
        //Check if local player so that they can't control the other players on the server
        if (PV.IsMine)
        {
            //BasicMovement and Rotation are just standard Unity controls (WASD for movement and mouse control for rotation) no special networking commands
            BasicMovement();
            BasicRotation();
        }
    }

    //Sounds play when player collides with AI
    public AudioClip[] impact; //Sets up array of audioclips
    AudioSource audioSource;
    private int num = 0; //Initializes int to randomly choose which sound to play on collision

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); //Find the audiosource component on the gameobject
        audioSource.PlayOneShot(impact[0], 0.7F); //Play the clip once
    }

    void OnCollisionEnter(Collision other)
    {
        //When the player collides with one of the agents (tagged as agents)
        if (other.gameObject.tag == "agent")
        {
            num = Random.Range(0, 3); //Randomly choose a clip to play
            //If statements that will choose which clip to play based on the number selected
            if (num == 1)
            {
                audioSource.PlayOneShot(impact[0], 0.7F);
            }
            else if (num == 1)
            {
                audioSource.PlayOneShot(impact[1], 0.7F);
            }
            else
            {
                audioSource.PlayOneShot(impact[2], 0.7F);
            }
        }
    }

    //Set rewards for agents (based on the food gathering ML-Agents demo in Unity
    void OnCollisionEnter(Collision collision)
    {
        //When agent hits a reward
        if (collision.gameObject.CompareTag("reward"))
        {
            SetReward(2f); //Since we only have a single reward in the middle, we set the reward level pretty high (2)
            print("2 reward!");
            Done(); //Resets agent to do it again
        }

        //Since our map is set up with interior walls, we want the agent to avoid them so we give them a small punishment if they do (-0.2)
        if (collision.gameObject.CompareTag("wall"))
        {
            // medium penalty for colliding with an outer wall tagged 'wall'
            AddReward(-.01f);
            print("-.01 Hit an outer wall");
            if (contribute)
            {
                //myAcademy.totalScore -= 1;
            }
        }
    }
}
