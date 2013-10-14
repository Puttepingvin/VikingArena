using System;
using System.Collections.Generic;

using Lidgren.Network;
using System.IO;

using TechnoViking.GlobalData;
 
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Utilities;

using Microsoft.Xna.Framework;
#if !FRB_MDX
using System.Linq;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FlatRedBall.Gui;
using FlatRedBall.Math.Geometry;
#endif
using FlatRedBall.Screens;

namespace TechnoViking
{
    public enum AgentRole
    {
        Client,
        Server
    }
    class NetworkAgent
    {
        private NetPeer mPeer;
        private NetPeerConfiguration mConfig;
        private AgentRole mRole;
        private int port = 6112;
        private NetOutgoingMessage mOutgoingMessage;
        private List<NetIncomingMessage> mIncomingMessages;
        byte nextPlayerID = 1;


        public bool IsPlayerConnected
        {
            get;
            private set;
        }

        public List<NetConnection> Connections
        {
            get
            {
                return mPeer.Connections;
            }
        }

        public NetworkAgent(AgentRole role, string tag)
        {
            mRole = role;
            mConfig = new NetPeerConfiguration(tag);
            Initialize();
        }
        private void Initialize()
        {
            


            if (mRole == AgentRole.Server)
            {
                mConfig.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
                mConfig.Port = port;
                mConfig.EnableUPnP = true;
                //mConfig.SimulatedMinimumLatency = 0.20f;
                //Casts the NetPeer to a NetServer
                mPeer = new NetServer(mConfig);
            }
            if (mRole == AgentRole.Client)
            {
                mConfig.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
                //Casts the NetPeer to a NetClient
                mPeer = new NetClient(mConfig);
            }


            
            mIncomingMessages = new List<NetIncomingMessage>();
            mOutgoingMessage = mPeer.CreateMessage();
            IsPlayerConnected = false;
            mPeer.Start();

        }
        /// <summary>
        /// Connects to a server. Throws an exception if you attempt to call Connect as a Server.
        /// </summary>
        public void Connect(string ip)
        {
            if (mRole == AgentRole.Client)
            {
                mPeer.Connect(ip, port);
            }
            else
            {
                throw new SystemException("Attempted to connect as server. Only clients should connect.");
            }
        }

        public void forwardport()
        {
            mPeer.UPnP.ForwardPort(port, "VikingArcade");
        }


        /// <summary>
        /// Reads every message in the queue and returns a list of data messages.
        /// Other message types just write a Console note.
        /// This should be called every update by the Game Screen
        /// The Game Screen should implement the actual handling of messages.
        /// </summary>
        /// <returns></returns>
        public List<NetIncomingMessage> CheckForMessages()
        {
            mIncomingMessages.Clear();
            NetIncomingMessage incomingMessage;
            string output = "";

            while ((incomingMessage = mPeer.ReadMessage()) != null)
            {
                switch (incomingMessage.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        mPeer.SendDiscoveryResponse(null, incomingMessage.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        if (mRole == AgentRole.Server)
                            output += incomingMessage.ReadString() + "\n";
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)incomingMessage.ReadByte();
                        if (mRole == AgentRole.Server)
                            output += "Status Message: " + incomingMessage.ReadString() + " \n";

                        if (status == NetConnectionStatus.Connected)
                        {
                            //PLAYER CONNECTED
                            //Message to send the player their ID
                            NetOutgoingMessage idMessage = mPeer.CreateMessage();
                            idMessage.Write((byte)MessageType.PlayerID);
                            idMessage.Write(nextPlayerID++);
                            mPeer.SendMessage(idMessage, incomingMessage.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                            IsPlayerConnected = true;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        mIncomingMessages.Add(incomingMessage);
                        break;
                    default:
                        // unknown message type
                        break;
                }
            }
            if (mRole == AgentRole.Server)
            {
                StreamWriter textOut = new StreamWriter(new FileStream("log.txt", FileMode.Append, FileAccess.Write));
                textOut.Write(output);
                textOut.Close();
            }
            return mIncomingMessages;
        }


        /// <summary>
        /// Write bool to message
        /// </summary>
        public void WriteMessage(bool message)
        {
            mOutgoingMessage.Write(message);
        }

        /// <summary>
        /// Write byte to message
        /// </summary>
        public void WriteMessage(byte message)
        {
            mOutgoingMessage.Write(message);
        }

        /// <summary>
        /// Write Int16 to message
        /// </summary>
        public void WriteMessage(Int16 message)
        {
            mOutgoingMessage.Write(message);
        }

        /// <summary>
        /// Write Int32 to message
        /// </summary>
        public void WriteMessage(Int32 message)
        {
            mOutgoingMessage.Write(message);
        }

        /// <summary>
        /// Write Int64 to message
        /// </summary>
        public void WriteMessage(Int64 message)
        {
            mOutgoingMessage.Write(message);
        }

        /// <summary>
        /// Write float to message
        /// </summary>
        public void WriteMessage(float message)
        {
            mOutgoingMessage.Write(message);
        }

        /// <summary>
        /// Write double to message
        /// </summary>
        public void WriteMessage(double message)
        {
            mOutgoingMessage.Write(message);
        }


        /// <summary>
        /// Sends off mOutgoingMessage and then clears it for the next send.
        /// Defaults to UnreliableSequenced for fast transfer which guarantees that older messages
        /// won't be processed after new messages. If IsGuaranteed is true it uses ReliableSequenced
        /// which is safer but much slower.
        /// </summary>
        public void SendMessage(NetConnection recipient)
        {
            SendMessage(recipient, false);
        }
        public void SendMessage(NetConnection recipient, bool IsGuaranteed)
        {
            NetDeliveryMethod method = IsGuaranteed ? NetDeliveryMethod.ReliableOrdered : NetDeliveryMethod.UnreliableSequenced;
            mPeer.SendMessage(mOutgoingMessage, recipient, method);
            mOutgoingMessage = mPeer.CreateMessage();
        }

        /// <summary>
        /// Closes the NetPeer
        /// </summary>
        public void Shutdown()
        {
            mPeer.Shutdown("Closing connection.");
        }
    }
}
