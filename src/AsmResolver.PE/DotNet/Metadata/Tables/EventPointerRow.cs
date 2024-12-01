﻿using System;
using System.Collections;
using System.Collections.Generic;
using AsmResolver.IO;

namespace AsmResolver.PE.DotNet.Metadata.Tables
{
    /// <summary>
    /// Represents a single row in the event pointer metadata table.
    /// </summary>
    public struct EventPointerRow : IMetadataRow, IEquatable<EventPointerRow>
    {
        /// <summary>
        /// Reads a single event pointer row from an input stream.
        /// </summary>
        /// <param name="reader">The input stream.</param>
        /// <param name="layout">The layout of the event pointer table.</param>
        /// <returns>The row.</returns>
        public static EventPointerRow FromReader(ref BinaryStreamReader reader, TableLayout layout)
        {
            return new EventPointerRow(reader.ReadIndex((IndexSize) layout.Columns[0].Size));
        }

        /// <summary>
        /// Creates a new row for the event pointer metadata table.
        /// </summary>
        /// <param name="event">The index into the Event table that this pointer references.</param>
        public EventPointerRow(uint @event)
        {
            Event = @event;
        }

        /// <inheritdoc />
        public TableIndex TableIndex => TableIndex.EventPtr;

        /// <inheritdoc />
        public int Count => 1;

        /// <inheritdoc />
        public uint this[int index] => index switch
        {
            0 => Event,
            _ => throw new IndexOutOfRangeException()
        };

        /// <summary>
        /// Gets or sets an index into the Event table that this pointer references.
        /// </summary>
        public uint Event
        {
            get;
            set;
        }

        /// <inheritdoc />
        public void Write(BinaryStreamWriter writer, TableLayout layout)
        {
            writer.WriteIndex(Event,(IndexSize) layout.Columns[0].Size);
        }

        /// <inheritdoc />
        public bool Equals(EventPointerRow other)
        {
            return Event == other.Event;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is EventPointerRow other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int) Event;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({Event:X8})";
        }

        /// <inheritdoc />
        public IEnumerator<uint> GetEnumerator()
        {
            return new MetadataRowColumnEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines whether two rows are considered equal.
        /// </summary>
        public static bool operator ==(EventPointerRow left, EventPointerRow right) => left.Equals(right);

        /// <summary>
        /// Determines whether two rows are not considered equal.
        /// </summary>
        public static bool operator !=(EventPointerRow left, EventPointerRow right) => !(left == right);
    }
}
