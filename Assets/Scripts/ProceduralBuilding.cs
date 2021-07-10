using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RoomInfo
{
    public int isType1;
    public int isType2;
    public int isType3;
    public float emissionAmount;
}

public class ProceduralBuilding : MonoBehaviour
{
    public ComputeShader rTexWriter;
    public Vector2Int nRooms;
    public Material buildingMat;

    private RenderTexture renderTexture;
    private ComputeBuffer buildingInfo;

    private void Awake()
    {
        GenerateBuildingInfo();
        SetupShader();
    }

    private void OnDisable()
    {
        buildingInfo?.Release();
        renderTexture?.Release();
    }

    private void SetBuildingType(ref RoomInfo room, int type)
    {
        if (type == 0)
        {
            room.isType1 = 1;
        }
        else if (type == 1)
        {
            room.isType2 = 1;
        }
        else
        {
            room.isType3 = 1;
        }
    }

    private void GenerateBuildingInfo()
    {
        RoomInfo[] rooms = new RoomInfo[nRooms.x * nRooms.y];
        for(int i = 0; i < nRooms.y; i++)
        {
            for (int j = 0; j < nRooms.x; j++)
            {
                int index = i * nRooms.x + j;
                if (j == nRooms.x-1)
                {
                    rooms[index] = rooms[i * nRooms.x];
                }
                else
                {
                    SetBuildingType(ref rooms[index], Random.Range(0, 3));
                    rooms[index].emissionAmount = Random.Range(0.1f, 1f);
                }
            }
        }

        int stride = sizeof(int) * 3 + sizeof(float);
        buildingInfo = new ComputeBuffer(rooms.Length, stride);
        buildingInfo.SetData(rooms);
    }

    private void SetupShader()
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            renderTexture.wrapMode = TextureWrapMode.Repeat;
            renderTexture.Create();
        }
        buildingMat.SetTexture("_InfoRTex", renderTexture);
        buildingMat.SetVector("_InteriorMapTiling", new Vector2(nRooms.x, nRooms.y));
        
        rTexWriter.SetTexture(0, "Result", renderTexture);
        rTexWriter.SetBuffer(0, "_Rooms", buildingInfo);
        rTexWriter.SetInts("_TextureDimension", new int[] { renderTexture.width - 1, renderTexture.height - 1 });
        rTexWriter.SetInts("_Tiling", new int[] { nRooms.x, nRooms.y });
    }

    private void DispatchCS()
    {
        int threadGroupX = Mathf.CeilToInt(renderTexture.width / 8f);
        int threadGroupY = Mathf.CeilToInt(renderTexture.height / 8f);
        rTexWriter.Dispatch(0, threadGroupX, threadGroupY, 1);
    }

    private void Update()
    {
        SetupShader();
        DispatchCS();
    }
}
