using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using Object = System.Object;

namespace UnityEditor.VFX
{
    class InlineTypeProvider : IVariantProvider
    {
        public Dictionary<string, object[]> variants
        {
            get
            {
                return new Dictionary<string, object[]>
                {
                    { "m_Type", VFXLibrary.GetSlotsType().Select(o => new SerializableType(o)).ToArray() }
                };
            }
        }
    }

    [VFXInfo(category = "Inline", variantProvider = typeof(InlineTypeProvider))]
    class VFXInlineOperator : VFXOperator
    {
        [SerializeField, VFXSetting(VFXSettingAttribute.VisibleFlags.None)]
        private SerializableType m_Type;

        public Type type
        {
            get
            {
                return (Type)m_Type;
            }
        }

        public override string name
        {
            get
            {
                var type = (Type)m_Type;
                return type == null ? string.Empty : VFXTypeExtension.UserFriendlyName(type);
            }
        }

        private IEnumerable<VFXPropertyWithValue> property
        {
            get
            {
                var type = (Type)m_Type;
                if (type != null)
                {
                    var property = new VFXProperty(type, string.Empty);
                    yield return new VFXPropertyWithValue(property, VFXTypeExtension.GetDefaultField(type));
                }
            }
        }

        protected override IEnumerable<VFXPropertyWithValue> inputProperties { get { return property; } }
        protected override IEnumerable<VFXPropertyWithValue> outputProperties { get { return property; } }

        protected override VFXExpression[] BuildExpression(VFXExpression[] inputExpression)
        {
            return inputExpression;
        }

        public override void Sanitize()
        {
            if (type == null)
            {
                // First try to force deserialization
                if (m_Type != null)
                {
                    m_Type.OnAfterDeserialize();
                }
                // if it doesn't work set it to int.
                if (type == null)
                    m_Type = new SerializableType(typeof(int));
            }
            base.Sanitize();
        }
    }
}
