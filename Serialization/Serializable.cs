using System;
using System.Collections.Generic;

namespace IngameScript.Serialization
{
    public abstract class Serializable
    {
        protected Dictionary<string, Field> fields = new Dictionary<string, Field>();
        /// <summary>
        /// Stores all fields that need to be serialized in the fields dictionary
        /// </summary>
        public abstract void SaveToFields();

        /// <summary>
        /// Applies all fields stored in a dictionary
        /// Remember fields that are not saved can not be loaded!!!
        /// </summary>
        /// <param name="fields">Dictionary to take load fields from</param>
        public abstract void LoadFields(Dictionary<string, Field> fields);

        /// <summary>
        /// Serializes this object and its fields to a string
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            SaveToFields();
            return Field.DicToString(fields);
        }

        /// <summary>
        /// Gets the protected fields variable after the SaveToFields function is called
        /// </summary>
        /// <returns>Dictionary of fields</returns>
        public Dictionary<string, Field> GetFields()
        {
            SaveToFields();
            return fields;
        }
    }
}