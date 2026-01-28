//CircularBuffer.cs
using System;

namespace SpaceZombie.Utilitaires.Tableaux
{
#region Buffer circulaire
    public class CircularBuffer<TValue>
    {
        private TValue[] _buffer;
        private int _head;
        private int _tail;
        private int _size;
        private int _capacity;

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.");

            _capacity = capacity;
            _buffer = new TValue[capacity];
            _head = 0;
            _tail = 0;
            _size = 0;
        }


        public void Enqueue(TValue item)
        {
            if (!CanEnqueue())
                throw new InvalidOperationException("Buffer is full.");

            _buffer[_tail] = item;
            _tail = (_tail + 1) % _capacity;
            _size++;
        }

        public TValue Dequeue()
        {
            if (!CanDequeue())
                throw new InvalidOperationException("Buffer is empty.");

            TValue item = _buffer[_head];
            _head = (_head + 1) % _capacity;
            _size--;
            return item;
        }

        public TValue Peek()
        {
            if (!CanDequeue())
                throw new InvalidOperationException("Buffer is empty.");

            return _buffer[_head];
        }


        public bool CanEnqueue() { return _size != _capacity; }
        public bool CanDequeue() { return _size > 0; }

        public int Count => _size;

        public int Capacity => _capacity;
    }
    public class CircularBufferThreadSafe<TValue>
    {
        private CircularBuffer<TValue> circularBuffer;
        private readonly object _lock = new object();

        public CircularBufferThreadSafe(int capacity)
        {
            circularBuffer = new CircularBuffer<TValue>(capacity);
        }


        public void Enqueue(TValue item)
        {
            lock (_lock)
            {
                circularBuffer.Enqueue(item);
            }
        }

        public TValue Dequeue()
        {
            lock (_lock)
            {
                return circularBuffer.Dequeue();
            }
        }

        public TValue Peek()
        {
            lock (_lock)
            {
                return circularBuffer.Peek();
            }
        }

        public bool CanEnqueue() { return circularBuffer.CanEnqueue(); }

        public bool CanDequeue() { return circularBuffer.CanDequeue(); }

        public int Count => circularBuffer.Count;

        public int Capacity => circularBuffer.Capacity;
    }
#endregion
}