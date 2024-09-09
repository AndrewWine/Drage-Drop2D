using UnityEngine;

[System.Serializable]
public class BlockData
{
    public float[] position = new float[3]; // X, Y, Z
    public float[] rotation = new float[4]; // X, Y, Z, W
    public float[] colorComponents = new float[4]; // R, G, B, A

    public BlockData(Vector3 position, Quaternion rotation, Color color)
    {
        this.position = new float[] { position.x, position.y, position.z };
        this.rotation = new float[] { rotation.x, rotation.y, rotation.z, rotation.w };
        this.colorComponents = new float[] { color.r, color.g, color.b, color.a };
    }

    public Vector3 GetPosition()
    {
        return new Vector3(position[0], position[1], position[2]);
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
    }

    public Color GetColor()
    {
        return new Color(colorComponents[0], colorComponents[1], colorComponents[2], colorComponents[3]);
    }
}
