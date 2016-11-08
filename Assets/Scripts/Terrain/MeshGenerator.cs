using UnityEngine;
using System.Collections;

public static class MeshGenerator {
	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail, bool useFlatShading, float lineFactor) {
		AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

		int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

		int meshSize = heightMap.GetLength(0) - 2 * meshSimplificationIncrement;
		int meshSizeUnsimplified = heightMap.GetLength(0) - 2;

		float topLeftX = (meshSizeUnsimplified - 1) / -2f;
		float topLeftZ = (meshSizeUnsimplified - 1) / 2f;


		int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

		MeshData meshData = new MeshData(verticesPerLine, useFlatShading);

		int[,] vertexIndicesMap = new int[meshSize, meshSize];
		int meshVertexIndex = 0;

		for (int y = 0; y < meshSize; y += meshSimplificationIncrement) {
			for (int x = 0; x < meshSize; x += meshSimplificationIncrement) {
				vertexIndicesMap[x, y] = meshVertexIndex;
				meshVertexIndex++;
			}
		}

		for (int y = 0; y < meshSize; y += meshSimplificationIncrement) {
			for (int x = 0; x < meshSize; x += meshSimplificationIncrement) {
				int vertexIndex = vertexIndicesMap[x, y];
				
				float height = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;
				Vector2 percent = new Vector2((x - meshSimplificationIncrement) / (float)meshSize, (y - meshSimplificationIncrement) / (float)meshSize);
				Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);
				meshData.AddVertex(vertexPosition, percent, vertexIndex);

				if (x < meshSize - 1 && y < meshSize - 1) {
					int a = vertexIndicesMap[x, y];
					int b = vertexIndicesMap[x + meshSimplificationIncrement, y];
					int c = vertexIndicesMap[x, y + meshSimplificationIncrement];
					int d = vertexIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];
					meshData.AddTriangle(a, d, c);
					meshData.AddTriangle(d, a, b);
				}

				vertexIndex++;
			}
		}

		meshData.ProcessMesh();

		return meshData;

	}
}

public class MeshData {
	Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;
	Vector3[] bakedNormals;

	int triangleIndex;

	bool useFlatShading;

	public MeshData(int verticesPerLine, bool useFlatShading) {
		this.useFlatShading = useFlatShading;

		vertices = new Vector3[verticesPerLine * verticesPerLine];
		uvs = new Vector2[verticesPerLine * verticesPerLine];
		triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

	}

	public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex) {
		vertices[vertexIndex] = vertexPosition;
		uvs[vertexIndex] = uv;
	}

	public void AddTriangle(int a, int b, int c) {
		triangles[triangleIndex] = a;
		triangles[triangleIndex + 1] = b;
		triangles[triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	Vector3[] CalculateNormals() {

		Vector3[] vertexNormals = new Vector3[vertices.Length];
		int triangleCount = triangles.Length / 3;
		for (int i = 0; i < triangleCount; i++) {
			int normalTriangleIndex = i * 3;
			int vertexIndexA = triangles[normalTriangleIndex];
			int vertexIndexB = triangles[normalTriangleIndex + 1];
			int vertexIndexC = triangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			vertexNormals[vertexIndexA] += triangleNormal;
			vertexNormals[vertexIndexB] += triangleNormal;
			vertexNormals[vertexIndexC] += triangleNormal;
		}

		for (int i = 0; i < vertexNormals.Length; i++) {
			vertexNormals[i].Normalize();
		}

		return vertexNormals;

	}

	Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC) {
		Vector3 pointA = vertices[indexA];
		Vector3 pointB = vertices[indexB];
		Vector3 pointC = vertices[indexC];

		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;
		return Vector3.Cross(sideAB, sideAC).normalized;
	}

	public void ProcessMesh() {
		if (useFlatShading) {
			FlatShading();
		} else {
			BakeNormals();
		}
	}

	void BakeNormals() {
		bakedNormals = CalculateNormals();
	}

	void FlatShading() {
		Vector3[] flatShadedVertices = new Vector3[triangles.Length];
		Vector2[] flatShadedUvs = new Vector2[triangles.Length];

		for (int i = 0; i < triangles.Length; i++) {
			flatShadedVertices[i] = vertices[triangles[i]];
			flatShadedUvs[i] = uvs[triangles[i]];
			triangles[i] = i;
		}

		vertices = flatShadedVertices;
		uvs = flatShadedUvs;
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		if (useFlatShading) {
			mesh.RecalculateNormals();
		} else {
			mesh.normals = bakedNormals;
		}
		return mesh;
	}

}