﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[SelectionBase]
[ExecuteInEditMode]
[RequireComponent(typeof(PolygonMesh))]
public class PolygonMeshSpawner : MonoBehaviour {
    public bool enableSeed = false;
    [ConditionalHide("enableSeed", true)]
    public int seed = 42;
    [Header("Global Settings")]
    public int globalSize = 1;
    [SerializeField] private Vector3 globalScale = Vector3.one;
    private Vector3 oldScale = Vector3.one;
    int oldSize = 1;
    [Header("Settings")]
    public GameObject[] prefabs = new GameObject[1];
    [SerializeField]
    public int[] instanceCount = new int[1];
    [SerializeField]
    public Vector3[] scales = new Vector3[1] { Vector3.one };
    [Header("Optional Settings")]
    [SerializeField]
    public Vector3[] scaleRanges = new Vector3[1];
    [SerializeField]
    public float[] yOffsets = new float[1];
    [SerializeField]
    public float[] minDistance = new float[1];

    /* fields */
    PolygonMesh polygon;
    const int MAX_SPAWN_TRIES = 50;
    float globalMinDistance = 0f;

    void Start() {
        polygon = gameObject.GetComponent<PolygonMesh>();
    }

    void OnValidate() {
        // update all fields
        if (globalSize != oldSize) {
            Array.Resize<GameObject>(ref prefabs, globalSize);
            Array.Resize<int>(ref instanceCount, globalSize);
            Array.Resize<Vector3>(ref scales, globalSize);
            Array.Resize<Vector3>(ref scaleRanges, globalSize);
            Array.Resize<float>(ref yOffsets, globalSize);
            Array.Resize<float>(ref minDistance, globalSize);
            for (int i = oldSize; i < globalSize; i++) {
                scales[i] = globalScale;
                instanceCount[i] = instanceCount[oldSize - 1];
            }
            oldSize = globalSize;
        } else if (globalScale != oldScale) {
            scales = Enumerable.Repeat(globalScale, globalSize).ToArray();
            oldScale = globalScale;
            int ct = transform.childCount;
            for (int i = 0; i < ct; i++) {
                var trans = transform.GetChild(i);
                UpdateInstance(ref trans, Int32.Parse(trans.name));
            }
        } else {
            int ct = transform.childCount;
            for (int i = 0; i < ct; i++) {
                var trans = transform.GetChild(i);
                UpdateInstance(ref trans, Int32.Parse(trans.name));
            }
        }
        // UPDATES
        globalMinDistance = minDistance.Min();
    }

    public void Spawn() {
        if (!polygon.updateMeshPerfromed.Contains(Spawn)) polygon.updateMeshPerfromed.Add(Spawn);
        if (polygon.enableHeight) {
            Debug.LogError("[PolygonMeshSpawner] Cannot Spawn wtih height enabled");
            return;
        }
        int ct = transform.childCount;
        for (int i = 0; i < ct; i++) {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }

        UnityEngine.Random.InitState(enableSeed ? seed : (int)Time.time);
        for (int i = 0; i < globalSize; i++)
            for (int j = 0; j < instanceCount[i]; j++)
                SpawnElement(i);
    }

    void SpawnElement(int elemId) {
        if (prefabs[elemId] == null) return;
        int[] tris = polygon.meshFilter.sharedMesh.triangles;
        Vector3[] verts = polygon.meshFilter.sharedMesh.vertices;
        // atleast 1 triangle
        if (tris.Length < 3) return;
        int spawnTry = 0;
        Vector3 spawnPos;
        do {
            if (++spawnTry > MAX_SPAWN_TRIES) {
                Debug.LogError("[PolygonMeshSpawner] Cannot Find proper Spawn position, too many instance");
                return;
            }
            // calc offset
            spawnPos = getRandomPointFromTriangleXZ(ref tris, ref verts) + transform.position;
        } while (!isValidSpawnPos(ref spawnPos, elemId));

        GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[elemId]);
        newInstance.transform.SetParent(transform);
        newInstance.transform.position = spawnPos;
        newInstance.name = elemId.ToString();
        var trans = newInstance.transform;
        UpdateInstance(ref trans, elemId);
    }

    void UpdateInstance(ref Transform trans, int elemId) {
        if (elemId >= globalSize) return;
        // SCALE
        Vector3 newScale = scales[elemId];
        newScale.x += UnityEngine.Random.Range(-scaleRanges[elemId].x, scaleRanges[elemId].x);
        newScale.y += UnityEngine.Random.Range(-scaleRanges[elemId].y, scaleRanges[elemId].y);
        newScale.z += UnityEngine.Random.Range(-scaleRanges[elemId].z, scaleRanges[elemId].z);
        trans.localScale = newScale;

        // Y OFFSET
        trans.position = new Vector3(trans.position.x, polygon.localY + transform.position.y + yOffsets[elemId], trans.position.z);
    }

    bool isValidSpawnPos(ref Vector3 pos, int elemId) {
        int ct = transform.childCount;
        for (int i = 0; i < ct; i++) {
            var trans = transform.GetChild(i);
            if ((elemId == Int32.Parse(trans.name) &&
                Vector3.Distance(pos, trans.position) < minDistance[elemId]) ||
                Vector3.Distance(pos, trans.position) < globalMinDistance)
                return false;
        }
        return true;
    }

    static Vector3 getRandomPointFromTriangleXZ(ref int[] tris, ref Vector3[] verts) {
        // GET RAND TRI INDEX
        float areaSum = 0.0f;
        for (int i = 0; i < tris.Length; i += 3) {
            Vector3 corner = verts[tris[i]];
            Vector3 a = verts[tris[i + 1]] - corner;
            Vector3 b = verts[tris[i + 2]] - corner;
            areaSum += Vector3.Cross(a, b).magnitude;
        }
        float randArea = UnityEngine.Random.Range(0f, areaSum);
        areaSum = 0.0f;
        int triId = 0;
        for (int i = 0; i < tris.Length; i += 3) {
            Vector3 corner = verts[tris[i]];
            Vector3 a = verts[tris[i + 1]] - corner;
            Vector3 b = verts[tris[i + 2]] - corner;
            areaSum += Vector3.Cross(a, b).magnitude;
            if (areaSum > randArea) {
                triId = i / 3;
                break;
            }
        }
        // GET RAND POINT in TRI
        triId *= 3;
        float r1 = UnityEngine.Random.Range(0f, 1f);
        float r2 = UnityEngine.Random.Range(0f, 1f);
        float px = (1 - Mathf.Sqrt(r1)) * verts[tris[triId]].x +
                   Mathf.Sqrt(r1) * (1 - r2) * verts[tris[triId + 1]].x +
                   (Mathf.Sqrt(r1) * r2) * verts[tris[triId + 2]].x;
        float pz = (1 - Mathf.Sqrt(r1)) * verts[tris[triId]].z +
                   Mathf.Sqrt(r1) * (1 - r2) * verts[tris[triId + 1]].z +
                   (Mathf.Sqrt(r1) * r2) * verts[tris[triId + 2]].z;
        return new Vector3(px, verts[tris[triId]].y, pz);
    }
}

[CustomEditor(typeof(PolygonMeshSpawner))]
public class PolygonMeshSpawnerEditor : Editor {
    PolygonMeshSpawner spawner;
    public override void OnInspectorGUI() {
        EditorGUILayout.HelpBox("All Childern will be destroyed upon spawn\n\nPlease change Number of Prefabs only via Global Size", MessageType.Warning);
        if (GUILayout.Button("Spawn")) spawner.Spawn();
        DrawDefaultInspector();
    }

    void OnEnable() {
        spawner = target as PolygonMeshSpawner;
    }
}
