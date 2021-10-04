using UnityEngine;

namespace CoreLib.Serializer
{
    public class SerializeTransform : MonoBehaviour, ISerializedComponent
    {
        public Serialization.SerializationComponentProperty SerializationProperty => Serialization.SerializationComponentProperty.Data;

        private const string k_PositionKey		= "p";
        private const string k_RotationKey		= "r";
        private const string k_ScaleKey			= "s";
	
        // =======================================================================
        public void Save(Serialization.IDataWriter writer)
        {
            writer.Write(k_PositionKey, gameObject.transform.localPosition);
            writer.Write(k_RotationKey, gameObject.transform.localRotation);
            writer.Write(k_ScaleKey, gameObject.transform.localScale);
        }

        public void Load(Serialization.IDataReader reader)
        {
            gameObject.transform.localPosition	= reader.Read<Vector3>(k_PositionKey);
            gameObject.transform.localRotation	= reader.Read<Quaternion>(k_RotationKey);
            gameObject.transform.localScale		= reader.Read<Vector3>(k_ScaleKey);
        }
    }
}