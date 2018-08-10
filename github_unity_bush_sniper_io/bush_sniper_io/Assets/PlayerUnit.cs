using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// A PlayerUnit is a unit controlled by a player
// This could be a character in an FPS, a zerling in a RTS
// Or a scout in a TBS

public class PlayerUnit : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // note: Syncvar with a hook is the sam as an RPC

    Vector3 velocity;

    // The position we think is most correct for this player.
    // NOTE: if we are the authority, then this will be 
    // exactly the same as transform.position;
    Vector3 bestGuessPosition;

    // This is a constantly updated value about our latency to the server
    // i.e. how many seconds it takes for us to receive a one-way message
    // TODO: This should probably be something we get from the player connection Object
    float ourLatency;

    // the higher this value, the faster our localPosition will match the bestGuessPosition
    float latencySmoothingFactor = 10;

    // Update is called once per frame
    void Update()
    {
        // This function runs on ALL PlayerUnits -- not just the ones that I own.

        // Code running right here is running for ALL version of this object, even
        // if its not the authoratitive copy
        // But even if we're Not the owner, we are trying to PREDICT where the object
        // should be right now, based on the last velocity update

        // How do I verify that I am allowed to mess around with this object? 
        if (hasAuthority == false)
        {

            // We aren't the authority for this object, but we still need to update 
            // our local position for this object based on our bestGuess of where
            // it probably is on the owning player's screen

            bestGuessPosition = bestGuessPosition + (velocity * Time.deltaTime);
             
            // instead of TELEPORTING our position to the best guess's position, we can 
            // smoothly lerp to it.

            transform.position = Vector3.Lerp(transform.position, bestGuessPosition, Time.deltaTime * latencySmoothingFactor);

            return;
        }

        // If we get to here we are the authoritative owner of this object
        transform.Translate(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.transform.Translate(0, 1, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            // destroying our object on every client, with hasAuthority we make sure that a client can only tell the other clients to destroy it's own object
            if(hasAuthority) {
                PlayerConnectionObject obj = gameObject.GetComponent<PlayerConnectionObject>();
                string playerName = obj.PlayerName;
                CmdDeleteGameObject(gameObject, playerName);

            }

           // this is only deleting our object on our client, but not on the other clients
            // Destroy(gameObject);
        }

        if(Input.GetKeyDown(KeyCode.RightArrow)) 
        {
            // the player is asking us to change our direction/speed (i.e. velocity)
            velocity = new Vector3(1, 0, 0);

            CmdUpdateVelocity(velocity, transform.position);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // the player is asking us to change our direction/speed (i.e. velocity)
            velocity = new Vector3(-1, 0, 0);

            CmdUpdateVelocity(velocity, transform.position);
        }

        if(Input.GetKeyUp(KeyCode.RightArrow)) {
            // the player is asking us to change our direction/speed (i.e. velocity)
            velocity = new Vector3(0, 0, 0);

            CmdUpdateVelocity(velocity, transform.position);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            // the player is asking us to change our direction/speed (i.e. velocity)
            velocity = new Vector3(0, 0, 0);

            CmdUpdateVelocity(velocity, transform.position);
        }
    }

    [Command]
    void CmdDeleteGameObject(GameObject obj, string playerName) {
        // I am on a Server
        RpcDeleteGameObject(obj, playerName);

    }

    [ClientRpc]
    void RpcDeleteGameObject(GameObject obj, string playerName)
    {
        // I am on a Client
        Debug.Log("Try to delete gameObject: " + playerName + ", on every client");
        // I am on a client
        if (gameObject.GetComponent<PlayerConnectionObject>().PlayerName == playerName)
        {
            Destroy(obj);
        }
    }



    [Command]
    void CmdUpdateVelocity(Vector3 v, Vector3 p) 
    {
        // I am on a Server
        transform.position = p;
        velocity = v;

        // if we know what our current latency is, we could do something like this:
        // transform.position = p + (v * (thisPlayersLatency));

        // Now let the clients know  the correct position of this object 
        RpcUpdateVelocity(velocity, transform.position);

    }


    [ClientRpc]
    void RpcUpdateVelocity(Vector3 v, Vector3 p) 
    {
        // I am on a client

        if (hasAuthority) {
            // this is my object, i "should" already have 
            // to most accurate position/velocity 
            // (possibly more "accurate" than the server)
            // depending on the game, I MIGHT want to change to patch
            // this info from the server, even tho that might 
            // look a little wonky to the user


            // lets assume for now that were just going to ignore the message from the server

        }

        // i am a non-authoratative client. so i definately need to listen to the server

        // if we know what our current latency is, we could do something like this:
        //  transform.position = p + ( v * (ourLatency));

        // transform.position = p;

        velocity = v;
        bestGuessPosition = p + (v * (ourLatency));              

        // Now position of player one is as close as possible on all player's screens

        // IN FACT, we dont want to directly update transform.positon, because then
        // players will keep teleporting/blinking as the updates come in. It looks dumb.


    }
}
