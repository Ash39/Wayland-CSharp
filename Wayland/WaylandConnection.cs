using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wayland
{
    public class WaylandConnection
    {
        private readonly WaylandSocket _socket;
        private readonly Queue<uint> _freeIds;
        private readonly ConcurrentQueue<Action> _events;
        private readonly List<WaylandObject> _objects;
        private readonly List<WaylandObject> _serverObjects;

        private readonly List<WaylandObject> _recycledObjects;


        private const uint ClientRangeBegin = 0x00000001;
        private const uint ClientRangeEnd = 0xFEFFFFFF;
        private const uint ServerRangeBegin = 0xff000000;
        private const uint ServerRangeEnd = 0xffffffff;
        private readonly uint _beginRange;
        private readonly uint _endRange;

        internal int Position => _socket.position;

        public ConcurrentQueue<Action> Events { get => _events;}

        public WaylandConnection(string socket,bool client = true)
        {
            this._socket = new WaylandSocket(socket);
            if (client) 
            {
                _beginRange = ClientRangeBegin;
                _endRange = ClientRangeEnd;
            }
            else
            {
                _beginRange = ServerRangeBegin;
                _endRange = ServerRangeEnd;
            }

            _objects = new List<WaylandObject>();
            _freeIds = new Queue<uint>();
            _events = new ConcurrentQueue<Action>();

            _serverObjects = new List<WaylandObject>();
            _recycledObjects = new List<WaylandObject>();
        }

        private void Create(ref uint id) 
        {
            if(id < ClientRangeEnd)
            {
                if (_freeIds.Count > 0)
                {
                    id = _freeIds.Dequeue();
                }
                else
                {
                    id = ((uint)_objects.Count) + ClientRangeBegin;
                    if (id >= _beginRange && id <= _endRange)
                    {
                        _objects.Add(null);
                    }
                    else
                        throw new IndexOutOfRangeException();
                }
            }else
            {
                _serverObjects.Add(null);
            }
            
            
        }

        public T Create<T>(uint id, uint version) where T : WaylandObject
        {
            Create(ref id);
                
            T @object = (T)_recycledObjects.FirstOrDefault(c => c.GetType() == typeof(T));
            if(@object != null)
            {
                @object.id = id;
                _recycledObjects.Remove(@object);
            }
            else
                @object = (T)Activator.CreateInstance(typeof(T), id, this, version);

            this[id] = @object;
            return @object;
        }

        internal int Flush(int timeout)
        {
            return _socket.Flush(timeout);
        }

        public void Destroy(uint id) 
        {
            if (id >= _beginRange && id <= _endRange)
            {
                _freeIds.Enqueue(id);
                WaylandObject @object = _objects.Single(c => {if(c != null) return c.id == id; return false;});
                this[id] = null;
                _recycledObjects.Add(@object);
            }
            else if (id >= ServerRangeBegin)
            {
                WaylandObject @object = _serverObjects.Single(c => {if(c != null) return c.id == id; return false;});
                _serverObjects.Remove(@object);
                _recycledObjects.Add(@object);
            }
            else
                throw new IndexOutOfRangeException();
        }

        public WaylandObject this[uint id]
        {
            get
            {
                if (id >= _beginRange && id <= _endRange)
                    return _objects[(int)(id - _beginRange)];
                else if(id >= ServerRangeBegin)
                    return _serverObjects[(int)(id - ServerRangeBegin)];
                else
                    return null;
            }
            private set
            {
                if (id >= _beginRange && id <= _endRange)
                    _objects[(int)(id - _beginRange)] = value;
                else if(id >= ServerRangeBegin)
                    _serverObjects[(int)(id - ServerRangeBegin)] = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }


        public void Marshal(uint id, ushort opcode, params object[] arguments)
        {
            ushort size = 8;
            foreach (object argument in arguments)
            {
                switch (argument)
                {
                    case int i:
                    case uint u:
                        size += 4;
                        break;
                    case double d:
                        size += 8;
                        break;
                    case string s:
                        size += 4;
                        size += (ushort)((Encoding.UTF8.GetByteCount(s) + 4) / 4 * 4);
                        break;
                    case byte[] a:
                        size += 4;
                        size += (ushort)((a.Length + 3) / 4 * 4);
                        break;
                    case IntPtr h:
                        break;
                }
            }
            _socket.Write(id);
            _socket.Write(((uint)size << 16) | (uint)opcode);
            foreach (object argument in arguments)
            {
                switch (argument)
                {
                    case int i:
                        _socket.Write(i);
                        break;
                    case uint u:
                        _socket.Write(u);
                        break;
                    case double d:
                        _socket.Write(d);
                        break;
                    case string s:
                        _socket.Write(s);
                        break;
                    case byte[] a:
                        _socket.Write(a);
                        break;
                    case IntPtr h:
                        _socket.Write(h);
                        break;
                }
            }
            //mutex.ReleaseMutex();
        }

        public bool Read() 
        {
            var (id, opCode, _) = _socket.ReadHeader();


            if(_objects.ElementAtOrDefault((int)(id - _beginRange)) == null && _serverObjects.ElementAtOrDefault((int)(id - ServerRangeBegin)) == null)
                return false;

            WaylandType[] types = this[id].WaylandTypes(opCode);

            WlType[] args = new WlType[types.Length];
            int i = 0;

            foreach (WaylandType type in types)
            {
                switch (type)
                {
                    case WaylandType.Fd:
                        args[i].p = _socket.ReadFd();
                        break;
                    case WaylandType.Int:
                        args[i].i = _socket.ReadInt();
                        break;
                    case WaylandType.Uint:
                        args[i].u = _socket.ReadUInt();
                        break;
                    case WaylandType.Fixed:
                        args[i].d = _socket.ReadDouble();
                        break;
                    case WaylandType.Object:
                        args[i].u = _socket.ReadUInt();
                        break;
                    case WaylandType.NewId:
                        args[i].u = _socket.ReadUInt();
                        break;
                    case WaylandType.String:
                        args[i].s = _socket.ReadString();
                        break;
                    case WaylandType.Array:
                        args[i].b = _socket.ReadBytes();
                        break;
                    case WaylandType.Handle:
                        args[i].p = _socket.ReadFd();
                        break;
                }
                i++;
            }

            _events.Enqueue(() => this[id].Event(opCode, args));

            return true;
            //mutex.WaitOne();
        }

       

        public void Disconnect()
        {
            _socket.Dispose();
            _freeIds.Clear();
        }

        public void Wait() => _socket.writeSemaphore.Wait();

        public int Release() => _socket.writeSemaphore.Release();
    }
}
