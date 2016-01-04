using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MandalaManager : MonoBehaviour {

    private static MandalaManager m_instance;
    public GameObject m_quad;
    public Camera m_cameraGenerator;
    public int increaseOrthographicSize = 2;
    public GameObject m_brush;
    public GameObject m_points;

    public static MandalaManager getInstance() {
        return m_instance;
    }

    public int m_radiusR = 5;
    public int m_radiusr = 3;
    public float m_distance = 5;

    private List<GameObject> m_listGameObjects;

/*
    public float RadiusR
    {
        get { return m_radiusR; }
        set { m_radiusR = (int)value; }
    }
    public float Radiusr
    {
        get { return m_radiusr; }
        set { m_radiusr = (int)value; }
    }
    public float Distance
    {
        get { return m_distance; }
        set { m_distance = value; }
    }
*/
#region Unity methods
    // Use this for initialization
	void Start () {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(this.gameObject);
           
        }
        else
        {
            if (m_instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            generateHipotrocoide(0.5f);
        }
    }
#endregion

#region hipotrocoide
    public void generateHipotrocoide(float diff)
    {
        if (m_listGameObjects != null) 
        { 
            for (int i = 0; i < m_listGameObjects.Count; i++)
            {
                Destroy(m_listGameObjects[i]);
            }
        }
        m_listGameObjects = new List<GameObject>();
        float maxX = float.MinValue; 
        float maxY = float.MinValue; 
        float minX = float.MaxValue; 
        float minY = float.MaxValue;

        float R = m_radiusR;
        float r = m_radiusr;
        float d = m_distance;

        int iterations = totalIterations();

        for (int i = 0; i < iterations; i++)
        {
            for (float angle = 0; angle < 360; angle += diff)
            {
                float radians = Mathf.Deg2Rad * angle * i;
                float x = (R - r) * Mathf.Cos(radians) + d * Mathf.Cos((R - r) / r * radians);
                float y = (R - r) * Mathf.Sin(radians) - d * Mathf.Sin((R - r) / r * radians);
                maxX = Mathf.Max(maxX, x);
                maxY = Mathf.Max(maxY, y);
                minX = Mathf.Min(minX, x);
                minY = Mathf.Min(minY, y);
            }
        }
   
        int ortographicSize = Mathf.RoundToInt(Mathf.Max(
                                               Mathf.Max(maxX, minX * -1) + increaseOrthographicSize, 
                                               Mathf.Max(maxY, minY * -1) + increaseOrthographicSize));
        m_cameraGenerator.orthographicSize = ortographicSize;
        Camera.main.orthographicSize = ortographicSize;
        
        
            for (float angle = 0; angle < 360; angle += diff)
            {
                float radians = Mathf.Deg2Rad * angle * iterations;
                float X = (R - r) * Mathf.Cos(radians) + d * Mathf.Cos((R - r) / r * radians);
                float Y = (R - r) * Mathf.Sin(radians) - d * Mathf.Sin((R - r) / r * radians);
                float Z = m_cameraGenerator.transform.position.z + 1;
                moveToPoint(X, Y, Z);
            }
            
       
        m_quad.transform.localScale = new Vector3(ortographicSize * 2, ortographicSize * 2, 1);
        setScalePoint(1f);
    }

    public int totalIterations()
    {
        // http://temasmatematicos.uniandes.edu.co/Integral_de_Honores/hipotrocoide/hipotrocoide.htm
        return m_radiusr / Mcd(m_radiusR, m_radiusr);
    }
    public int Mcd(int a, int b)
    {
        if (b > a) return Mcd(b, a);
        else if (b == 0) return a;
        else return Mcd(a - b, b);
    }
#endregion

#region paintTexture
    private IEnumerator paintInTexture()
    {
        RenderTexture text = new RenderTexture(1024, 1024, 24);
        Graphics.SetRenderTarget(text);
        GL.Clear(true, true, Color.white);
        m_cameraGenerator.targetTexture = text;
        yield return new WaitForEndOfFrame();
        m_quad.GetComponent<Renderer>().material.mainTexture = text;
    }
    public void setScalePoint(float scale)
    {
        for (int i = 0; i < m_listGameObjects.Count; i++)
        {
            m_listGameObjects[i].transform.localScale = new Vector3(scale, scale, scale);
        }
    }

#endregion

#region move brush
    public void moveToPoint(float X, float Y, float Z)
    {
        GameObject go = Instantiate(m_brush) as GameObject;
        go.transform.position = new Vector3(X, Y, Z);
        go.transform.parent = m_points.transform;
        m_listGameObjects.Add(go);
    }
#endregion


}
