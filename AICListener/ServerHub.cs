
using ApplicationCore;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AICListener
{
    public delegate void ClientConnectionEventHandler(string clientId);
    public delegate void ClientNameChangedEventHandler(string clientId, string newName);

    public delegate void ObjReceivedEventHandler(string senderClientId, string obj);

    public class ServerHub : Hub
    {
        static ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();

        public static event ClientConnectionEventHandler ClientConnected;
        public static event ClientConnectionEventHandler ClientDisconnected;

        public static event ClientNameChangedEventHandler ClientNameChanged;

        public static event ObjReceivedEventHandler ObjReceived;

        public static void ClearState()
        {
            _users.Clear();
        }

        public override Task OnConnected()
        {
            _users.TryAdd(Context.ConnectionId, Context.ConnectionId);

            ClientConnected?.Invoke(Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string username;

            _users.TryRemove(Context.ConnectionId, out username);

            ClientDisconnected?.Invoke(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        #region Client Methods
        public void SetUserName(string userName)
        {
            _users[Context.ConnectionId] = userName;

            ClientNameChanged?.Invoke(Context.ConnectionId, userName);
        }

        public void Send(string message)
        {
            Clients.All.AddMessage(_users[Context.ConnectionId], message);
            ObjReceived?.Invoke(Context.ConnectionId, message);
        }

        public void SendObj(string objJson)
        {
            var objMessage = JsonConvert.DeserializeObject<ObjMessage>(objJson);
            var messageToSend = $"{objMessage.SoGhe} - {objMessage.TrangThai}";

            Clients.All.AddObjMessage(_users[Context.ConnectionId], messageToSend);
            ObjReceived?.Invoke(Context.ConnectionId, messageToSend);
        }
        #endregion
    }
}
