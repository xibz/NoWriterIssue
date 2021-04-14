using Mirror;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestUtils {
    public class NetworkMockTest {
        public static ushort port = 7777;
        public static string address = "localhost";

        protected float inactiveTimeout = 1.0f;
        protected GameObject testObject;
        protected NetworkManager networkManager;
        protected GameObject player;

        [UnitySetUp]
        public IEnumerator Setup() {
            testObject = new GameObject();
            player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/TestPlayer.prefab");

            testObject.AddComponent<kcp2k.KcpTransport>();
            // TODO: Add our own test transport
            kcp2k.KcpTransport transport = testObject.GetComponent<kcp2k.KcpTransport>();
            transport.Port = port;
            ((MonoBehaviour)transport).runInEditMode = true;
            Transport.activeTransport = transport;

            testObject.AddComponent<NetworkManager>();
            networkManager = testObject.GetComponent<NetworkManager>();
            SetupNetworkManager(networkManager);

            // we get the network id to lazy load the assetID on the NetworkIdentity
            NetworkIdentity id = player.GetComponent<NetworkIdentity>();
            if (id == null) {
                id = player.AddComponent<NetworkIdentity>();
            }

            networkManager.Awake();
            networkManager.StartHost();
            networkManager.Start();
            networkManager.LateUpdate();
            NetworkClient.Ready();

            yield return null;
        }

        public void SetupNetworkManager(NetworkManager nm) {
            nm.networkAddress = address;
            nm.autoCreatePlayer = true;
            nm.runInEditMode = true;
            nm.runInBackground = true;
            nm.autoStartServerBuild = true;
            nm.maxConnections = 1;
            nm.serverTickRate = 60;
            nm.disconnectInactiveTimeout = inactiveTimeout;
            nm.playerPrefab = player;
        }

        public NetworkConnectionToClient GetPlayerConnection() {
            foreach (KeyValuePair<int, NetworkConnectionToClient> kv in NetworkServer.connections) {
                NetworkConnectionToClient conn = kv.Value;
                // return the only value since we are the only connection on
                // the server
                //
                // TODO: Make this work for multiple connections
                return conn;
            }

            return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown() {
            if (!NetworkServer.active) {
                yield break;
            }

            foreach (NetworkIdentity id in NetworkIdentity.spawned.Values) {
                // set isServer is false. otherwise Destroy instead of
                // DestroyImmediate is called internally, giving an error in Editor
                TestUtils.PrivateUtils.SetValue(id, "isServer", false);
                GameObject.DestroyImmediate(id.gameObject);
            }

            // clean so that null entries are not in dictionary
            NetworkIdentity.spawned.Clear();

            // clear connections first. calling OnDisconnect wouldn't work since
            // we have no real clients.
            NetworkServer.connections.Clear();

            // stop server and client
            NetworkClient.Shutdown();
            NetworkServer.Shutdown();
            GameObject.DestroyImmediate(Transport.activeTransport.gameObject);
            Transport.activeTransport = null;

            yield return null;
        }
    }
}
