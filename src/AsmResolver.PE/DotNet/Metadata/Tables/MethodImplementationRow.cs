using System;
using System.Collections;
using System.Collections.Generic;
using AsmResolver.IO;

namespace AsmResolver.PE.DotNet.Metadata.Tables
{
    /// <summary>
    /// Represents a single row in the method implementation metadata table.
    /// </summary>
    public struct MethodImplementationRow : IMetadataRow, IEquatable<MethodImplementationRow>
    {
        /// <summary>
        /// Reads a single method implementation row from an input stream.
        /// </summary>
        /// <param name="reader">The input stream.</param>
        /// <param name="layout">The layout of the method implementation table.</param>
        /// <returns>The row.</returns>
        public static MethodImplementationRow FromReader(ref BinaryStreamReader reader, TableLayout layout)
        {
            return new MethodImplementationRow(
                reader.ReadIndex((IndexSize) layout.Columns[0].Size),
                reader.ReadIndex((IndexSize) layout.Columns[1].Size),
                reader.ReadIndex((IndexSize) layout.Columns[2].Size));
        }

        /// <summary>
        /// Creates a new row for the method implementation metadata table.
        /// </summary>
        /// <param name="class">The index into the TypeDef table indicating the class that inherited methods from an interface.</param>
        /// <param name="methodBody">The MethodDefOrRef index indicating the method which provides the implementation
        /// for the interface method.</param>
        /// <param name="methodDeclaration">The MethodDefOrRef index indicating the interface method which is implemented.</param>
        public MethodImplementationRow(uint @class, uint methodBody, uint methodDeclaration)
        {
            Class = @class;
            MethodBody = methodBody;
            MethodDeclaration = methodDeclaration;
        }

        /// <inheritdoc />
        public TableIndex TableIndex => TableIndex.MethodImpl;

        /// <inheritdoc />
        public int Count => 3;

        /// <inheritdoc />
        public uint this[int index] => index switch
        {
            0 => Class,
            1 => MethodBody,
            2 => MethodDeclaration,
            _ => throw new IndexOutOfRangeException()
        };

        /// <summary>
        /// Gets or sets an index into the TypeDef table indicating the class that inherited methods from an interface.
        /// </summary>
        public uint Class
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a MethodDefOrRef index (an index into either the Method or MemberRef table) indicating the method
        /// which provides the implementation for the interface method.
        /// </summary>
        public uint MethodBody
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a MethodDefOrRef index (an index into either the Method or MemberRef table) indicating the interface
        /// method which is implemented.
        /// </summary>
        public uint MethodDeclaration
        {
            get;
            set;
        }

        /// <inheritdoc />
        public void Write(BinaryStreamWriter writer, TableLayout layout)
        {
            writer.WriteIndex(Class, (IndexSize) layout.Columns[0].Size);
            writer.WriteIndex(MethodBody, (IndexSize) layout.Columns[1].Size);
            writer.WriteIndex(MethodDeclaration, (IndexSize) layout.Columns[2].Size);
        }

        /// <inheritdoc />
        public bool Equals(MethodImplementationRow other)
        {
            return Class == other.Class
                   && MethodBody == other.MethodBody
                   && MethodDeclaration == other.MethodDeclaration;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is MethodImplementationRow other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int) Class;
                hashCode = (hashCode * 397) ^ (int) MethodBody;
                hashCode = (hashCode * 397) ^ (int) MethodDeclaration;
                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({Class:X8}, {MethodBody:X8}, {MethodDeclaration:X8})";
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
        public static bool operator ==(MethodImplementationRow left, MethodImplementationRow right) => left.Equals(right);

        /// <summary>
        /// Determines whether two rows are not considered equal.
        /// </summary>
        public static bool operator !=(MethodImplementationRow left, MethodImplementationRow right) => !(left == right);
    }
}
