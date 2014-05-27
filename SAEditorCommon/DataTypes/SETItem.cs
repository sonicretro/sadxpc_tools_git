﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.UI;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
    [Serializable]
	public class SETItem : Item, ICustomTypeDescriptor
    {
        public SETItem()
        {
            Position = new Vertex();
            Rotation = new Rotation();
            Scale = new Vertex();
        }

        public SETItem(byte[] file, int address)
        {
            ID = BitConverter.ToUInt16(file, address);
            ushort xrot = BitConverter.ToUInt16(file, address + 2);
            ushort yrot = BitConverter.ToUInt16(file, address + 4);
            ushort zrot = BitConverter.ToUInt16(file, address + 6);
            Rotation = new Rotation(xrot, yrot, zrot);
            Position = new Vertex(file, address + 8);
            Scale = new Vertex(file, address + 0x14);
            isLoaded = true;
        }

        [ParenthesizePropertyName(true)]
        public string Name { get { return LevelData.ObjDefs[id].Name; } }

        protected bool isLoaded = false;
        private ushort id;
        [Editor(typeof(IDEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ushort ID
        {
			get { return id; }
			set { id = (ushort)(value & 0xFFF); }
        }

        public override Vertex Position { get; set; }

        public override Rotation Rotation { get; set; }

        public Vertex Scale { get; set; }

        public override void Paste()
        {
            LevelData.SETItems[LevelData.Character].Add(this);
        }

        public override void Delete()
        {
            LevelData.SETItems[LevelData.Character].Remove(this);
        }

        public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            return LevelData.ObjDefs[ID].CheckHit(this, Near, Far, Viewport, Projection, View, new MatrixStack());
        }

        public override RenderInfo[] Render(Device dev, EditorCamera camera, MatrixStack transform, bool selected)
        {
            float dist = SonicRetro.SAModel.Direct3D.Extensions.Distance(camera.Position, this.Position.ToVector3());
            if (dist > camera.DrawDistance) return Item.EmptyRenderInfo;

            return LevelData.ObjDefs[ID].Render(this, dev, transform, selected);
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(0x20);
            bytes.AddRange(BitConverter.GetBytes(ID));
            unchecked
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.X));
                bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.Y));
                bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.Z));
            }
            bytes.AddRange(Position.GetBytes());
            bytes.AddRange(Scale.GetBytes());
            return bytes.ToArray();
        }

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection result = TypeDescriptor.GetProperties(this, attributes, true);

			ObjectDefinition objdef = LevelData.ObjDefs[ID];
			if (objdef.CustomProperties == null || objdef.CustomProperties.Length == 0) return result;
			List<PropertyDescriptor> props = new List<PropertyDescriptor>(result.Count);
			foreach (PropertyDescriptor item in result)
				props.Add(item);

			foreach (PropertySpec property in objdef.CustomProperties)
			{
				List<Attribute> attrs = new List<Attribute>();

				// Additionally, append the custom attributes associated with the
				// PropertySpec, if any.
				if (property.Attributes != null)
					attrs.AddRange(property.Attributes);

				// Create a new property descriptor for the property item, and add
				// it to the list.
				PropertySpecDescriptor pd = new PropertySpecDescriptor(property,
					property.Name, attrs.ToArray());
				props.Add(pd);
			}

			return new PropertyDescriptorCollection(props.ToArray(), true);
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
	}
}
