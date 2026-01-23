using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haidon_BE.Application.Services.Realtime
{
    public interface IConnectionManager
    {
        void AddConnection(Guid userId, string connectionId);
        void RemoveConnection(string connectionId);
        List<string> GetConnections(Guid userId);
        Guid? GetUserId(string connectionId);
    }

    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, HashSet<string>> _userConnections = new();
        private readonly ConcurrentDictionary<string, Guid> _connectionUsers = new();
        private readonly object _lock = new();

        public void AddConnection(Guid userId, string connectionId)
        {
            lock (_lock)
            {
                _userConnections.AddOrUpdate(userId,
                    new HashSet<string> { connectionId },
                    (key, connections) =>
                    {
                        connections.Add(connectionId);
                        return connections;
                    });

                _connectionUsers.TryAdd(connectionId, userId);
            }
        }

        public void RemoveConnection(string connectionId)
        {
            lock (_lock)
            {
                if (_connectionUsers.TryRemove(connectionId, out var userId))
                {
                    if (_userConnections.TryGetValue(userId, out var connections))
                    {
                        connections.Remove(connectionId);
                        if (connections.Count == 0)
                            _userConnections.TryRemove(userId, out _);
                    }
                }
            }
        }

        public List<string> GetConnections(Guid userId)
        {
            return _userConnections.TryGetValue(userId, out var connections)
                ? connections.ToList()
                : new List<string>();
        }

        public Guid? GetUserId(string connectionId)
        {
            return _connectionUsers.TryGetValue(connectionId, out var userId)
                ? userId
                : null;
        }
    }
}
