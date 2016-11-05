using UnityEngine;
using System.Collections;

public static class MeshGenerator {

	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail) {
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

		TriangleMeshData meshData = new TriangleMeshData(verticesPerLine, verticesPerLine);
		int vertexIndex = 0;

		for (int y = 0; y < height; y += meshSimplificationIncrement) {
			for (int x = 0; x < width; x += meshSimplificationIncrement) {
				meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
				meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

				if (x < width - 1 && y < height - 1) {
					meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
					meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
				}

				vertexIndex++;
			}
		}

		return meshData;

	}

	public static MeshData GenerateVasarelyLineMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail) {
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

		LineMeshData meshData = new LineMeshData(verticesPerLine, verticesPerLine);
		int vertexIndex = 0;

		for (int y = 0; y < height; y += meshSimplificationIncrement) {
			for (int x = 0; x < width; x += meshSimplificationIncrement) {
				meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[width - x - 1, y]) * heightMultiplier, topLeftZ - y);

				if (x < width - 1 && y < height - 1) {
					meshData.AddLine(vertexIndex, vertexIndex + 1);
				}
				vertexIndex++;
			}
		}

		return meshData;

	}

	public static MeshData GenerateVasarelyBillboardLineMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail, float lineWidthFactor, bool flatShading) {
		int meshSize = heightMap.GetLength(0);
		//int meshSize = heightMap.GetLength(1);
		float topLeftX = (meshSize - 1) / -2f;
		float topLeftZ = (meshSize - 1) / 2f;

		int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		int verticesPerLine = 2 * ((meshSize - 1) / meshSimplificationIncrement + 1);
		float lineWidthHalfMargin = Mathf.Lerp(0, 0.5f, lineWidthFactor);

		BillboardLineMeshData meshData = new BillboardLineMeshData(verticesPerLine, flatShading);
		int vertexIndex = 0;

		for (int y = 0; y < meshSize; y += meshSimplificationIncrement) {
			for (int x = 0; x < meshSize; x += meshSimplificationIncrement) {

				Vector3 vertex = new Vector3(topLeftX + x,
																			heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier,
																			topLeftZ - (y - lineWidthHalfMargin));
				Vector2 percent = new Vector2(x / (float)meshSize, (y - lineWidthHalfMargin) / (float)meshSize);
				meshData.AddVertex(vertex, percent, vertexIndex);

				vertex = new Vector3(topLeftX + x,
																			heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier,
																			topLeftZ - (y + lineWidthHalfMargin));
				percent = new Vector2(x / (float)meshSize, (y + lineWidthHalfMargin) / (float)meshSize);
				meshData.AddVertex(vertex, percent, vertexIndex + 1);

				if (x < meshSize - 1 && y < meshSize - 1) {
					meshData.AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 1);
					meshData.AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 2);

					meshData.AddTriangle(vertexIndex + verticesPerLine + 2, vertexIndex + verticesPerLine, vertexIndex + 1);
					meshData.AddTriangle(vertexIndex + 1, vertexIndex + 3, vertexIndex + verticesPerLine + 2);
				}

				vertexIndex += 2;
			}
		}

		return meshData;
	}
}



public abstract class MeshData {

	public abstract Mesh CreateMesh();
}

public class TriangleMeshData : MeshData {
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;

	int triangleIndex;

	public TriangleMeshData(int meshWidth, int meshHeight) {
		vertices = new Vector3[meshWidth * meshHeight];
		uvs = new Vector2[meshWidth * meshHeight];
		triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
	}

	public void AddTriangle(int a, int b, int c) {
		triangles[triangleIndex] = a;
		triangles[triangleIndex + 1] = b;
		triangles[triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	public override Mesh CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		return mesh;
	}

}

public class LineMeshData : MeshData {
	public Vector3[] vertices;
	public int[] indices;

	int lineIndex;

	public LineMeshData(int meshWidth, int meshHeight) {
		vertices = new Vector3[meshWidth * meshHeight];
		indices = new int[(meshWidth - 1) * 2 * (meshHeight)];
	}

	public void AddLine(int a, int b) {
		indices[lineIndex] = a;
		indices[lineIndex + 1] = b;
		lineIndex += 2;
	}

	public override Mesh CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.SetIndices(indices, MeshTopology.Lines, 0);
		return mesh;
	}

}

public class BillboardLineMeshData : MeshData {
	Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;
	Vector3[] bakedNormals;

	int triangleIndex;

	bool useFlatShading;

	public BillboardLineMeshData(int verticesPerLine, bool useFlatShading) {
		this.useFlatShading = useFlatShading;

		vertices = new Vector3[verticesPerLine * verticesPerLine];
		uvs = new Vector2[verticesPerLine * verticesPerLine];
		triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 4 * 3]; // * 2 triangles per vertex * 3 points
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

	public override Mesh CreateMesh() {
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