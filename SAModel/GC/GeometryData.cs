﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
	public enum GeometryType
	{
		Opaque,
		Translucent
	}

	public class GeometryData
	{
		public List<Mesh> OpaqueMeshes { get; private set; }
		public List<Mesh> TranslucentMeshes { get; private set; }

		private IndexAttributeParameter cur_attributes;

		public GeometryData()
		{
			OpaqueMeshes = new List<Mesh>();
			TranslucentMeshes = new List<Mesh>();
		}

		public void Load(byte[] file, int address, uint imageBase, int count, GeometryType geometry_type)
		{
			for (int i = 0; i < count; i++)
			{
				Mesh new_mesh = new Mesh(file, address, imageBase, cur_attributes);

				IndexAttributeParameter param = (IndexAttributeParameter)new_mesh.Parameters.Find(x => x.ParameterType == ParameterType.IndexAttributeFlags);
				if (param != null)
				{
					cur_attributes = param;
				}

				if (geometry_type == GeometryType.Translucent)
				{
					TranslucentMeshes.Add(new_mesh);
				}
				else
				{
					OpaqueMeshes.Add(new_mesh);
				}

				address += 16;
			}
		}

		public void WriteGeometryData(BinaryWriter writer, uint imageBase)
		{

		}
	}
}