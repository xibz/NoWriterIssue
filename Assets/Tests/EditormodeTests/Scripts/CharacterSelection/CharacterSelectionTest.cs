using Mirror;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class CharacterSelectionTest : TestUtils.NetworkMockTest {
    [UnityTest]
    public IEnumerator CharacterSelectionGeneration() {
        Random.InitState(0);
        GameObject world = new GameObject();
        world.AddComponent<CharacterSelection>();
        CharacterSelection cs = world.GetComponent<CharacterSelection>();
        NetworkServer.Spawn(world);
        cs.CmdGenerateClasses();
        networkManager.LateUpdate();
        yield return null;
    }
}

