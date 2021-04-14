using Mirror;
using System.Collections.Generic;
using UnityEngine;

/// <summary>CharacterSelection is a set of playerAdapters which will be used
/// to adapt a player into a certain class upon choice</summary>
public class CharacterSelection : NetworkBehaviour {
    public class ClassPreview : NetworkMessage {
        public string foo;
    }

    private NetworkIdentity identity;

    /// <summary>CmdGenerateClasses generates a series of classes for that
    /// connection.</summary>
    /// <param>conn is the connection that request is coming from</param>
    //
    // TODO: connection ID may change if the user disconnects and reconnects.
    // We should probably figure out a way to save state somehow like using the
    // steam name instead
    [Command(requiresAuthority = false)]
    public void CmdGenerateClasses(NetworkConnectionToClient conn = null) {
        Debug.Log("Generating classes");
        ClassPreview[] previews = new ClassPreview[5];
        for (int i = 0; i < 5; i++) {
            previews[i] = new ClassPreview();
            previews[i].foo = i.ToString();
        }
        TargetSetClasses(conn, previews);
    }

    /// <summary>TargetSetClasses will pass along the class previews back to
    /// the client which will be used for the character selection GUI<summary>
    [TargetRpc]
    public void TargetSetClasses(NetworkConnection conn, ClassPreview[] previews) {
        Debug.Log("Setting classes and invoking strategies");
        Debug.Log(previews[0].foo);
    }
}
