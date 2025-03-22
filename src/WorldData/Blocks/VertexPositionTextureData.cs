using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

namespace mc_clone.src.WorldData.Blocks
{
    [DataContract]
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionTextureData : IVertexType
    {
        [DataMember]
        public Vector3 Position;
        [DataMember]
        public Vector2 TextureCoordinate;
        [DataMember]
        public float MaterialId;

        public static readonly VertexDeclaration VertexDeclaration;

        public VertexPositionTextureData(Vector3 position, Vector2 textureCoordinate, float id)
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            MaterialId = id;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }

        public override string ToString()
        {
            return $"{{Position: {Position}}}";
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Position, TextureCoordinate, MaterialId);
        }

        public static bool operator ==(VertexPositionTextureData left, VertexPositionTextureData right)
        {
            return left.Position == right.Position && left.TextureCoordinate == right.TextureCoordinate && left.MaterialId == right.MaterialId;
        }

        public static bool operator !=(VertexPositionTextureData left, VertexPositionTextureData right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return this == (VertexPositionTextureData)obj;
        }

        static VertexPositionTextureData()
        {
            VertexElement[] elements = new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(20, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1), // User defined data, not texture info.
            };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }
    }
}
