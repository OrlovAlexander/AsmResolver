using System;
using System.Collections;
using System.Collections.Generic;
using AsmResolver.IO;

namespace AsmResolver.PE.DotNet.Metadata.Tables
{
    /// <summary>
    /// Represents a single row in the property map metadata table.
    /// </summary>
    public struct PropertyMapRow : IMetadataRow, IEquatable<PropertyMapRow>
    {
        /// <summary>
        /// Reads a single property map row from an input stream.
        /// </summary>
        /// <param name="reader">The input stream.</param>
        /// <param name="layout">The layout of the property map table.</param>
        /// <returns>The row.</returns>
        public static PropertyMapRow FromReader(ref BinaryStreamReader reader, TableLayout layout)
        {
            return new PropertyMapRow(
                reader.ReadIndex((IndexSize) layout.Columns[0].Size),
                reader.ReadIndex((IndexSize) layout.Columns[1].Size));
        }

        /// <summary>
        /// Creates a new row for the property map metadata table.
        /// </summary>
        /// <param name="parent">The index into the TypeDef table that this mapping is associating to an property list.</param>
        /// <param name="propertyList">The index into the Event table indicating the first property that is defined in
        /// the property list.</param>
        public PropertyMapRow(uint parent, uint propertyList)
        {
            Parent = parent;
            PropertyList = propertyList;
        }

        /// <inheritdoc />
        public TableIndex TableIndex => TableIndex.PropertyMap;

        /// <inheritdoc />
        public int Count => 2;

        /// <inheritdoc />
        public uint this[int index] => index switch
        {
            0 => Parent,
            1 => PropertyList,
            _ => throw new IndexOutOfRangeException()
        };

        /// <summary>
        /// Gets or sets an index into the TypeDef table that this mapping is associating to an property list.
        /// </summary>
        public uint Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an index into the Event table indicating the first property that is defined in the property list.
        /// </summary>
        public uint PropertyList
        {
            get;
            set;
        }

        /// <inheritdoc />
        public void Write(BinaryStreamWriter writer, TableLayout layout)
        {
            writer.WriteIndex(Parent, (IndexSize) layout.Columns[0].Size);
            writer.WriteIndex(PropertyList, (IndexSize) layout.Columns[1].Size);
        }

        /// <inheritdoc />
        public bool Equals(PropertyMapRow other)
        {
            return Parent == other.Parent && PropertyList == other.PropertyList;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is PropertyMapRow other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Parent * 397) ^ (int) PropertyList;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({Parent:X8}, {PropertyList:X8})";
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
        public static bool operator ==(PropertyMapRow left, PropertyMapRow right) => left.Equals(right);

        /// <summary>
        /// Determines whether two rows are not considered equal.
        /// </summary>
        public static bool operator !=(PropertyMapRow left, PropertyMapRow right) => !(left == right);
    }
}
