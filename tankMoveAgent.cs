using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class tankMoveAgent : Agent
{
    public Bounds area;
    public GameObject Terrain;
     [SerializeField] private string enemy;
    private float max_distance = 55f;

    Renderer m_TerrainRender;
    Material terrainMaterial;
    public bool useVectorOrbs;
    private Rigidbody tankRb;
    private BoxCollider box;
    public Vector3 StartPosition {set {StartPosition = value;}}
    public Quaternion StartRotation{set {StartRotation = value;}}
    private Rigidbody m_Rigidbody { get; set; }

    EnvironmentParameters m_ResetP;
    public override void Initialize()
    {
        tankRb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        area = Terrain.GetComponent<Collider>().bounds;
        m_TerrainRender = Terrain.GetComponent<Renderer>();
        terrainMaterial = m_TerrainRender.material;
        m_ResetP = Academy.Instance.EnvironmentParameters;

        SetResetParameters();
    }

    public override void OnEpisodeBegin()
    {
        var rotation = Random.Range(0, 4);
        var rotationAngle = rotation * 90f;
        Terrain.transform.Rotate(new Vector3(0f, rotationAngle, 0f));
        
        tankRb.velocity = Vector3.zero;
        tankRb.angularVelocity = Vector3.zero;

        SetResetParameters();
    }

    
    public override void OnActionReceived(ActionBuffers actions)
    {
        moveTank(actions.DiscreteActions);
        if(MaxStep > 0)
            AddReward(-1f/MaxStep);
    }

    public void moveTank(ActionSegment<int> act){
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];

        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        tankRb.AddForce(dirToGo,
            ForceMode.VelocityChange);
    }

      public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;

        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }

        public void ResetBlock()
    {
        tankRb.velocity = Vector3.zero;
        tankRb.angularVelocity = Vector3.zero;
    }

    public void setGroundMaterialFriction()
    {
        var groundCollider = Terrain.GetComponent<Collider>();

        groundCollider.material.dynamicFriction = m_ResetP.GetWithDefault("dynamic_friction", 0);
        groundCollider.material.staticFriction = m_ResetP.GetWithDefault("static_friction", 0);
    }

    public void SetResetParameters()
    {
        setGroundMaterialFriction();
    }

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("Bullet")){
            Debug.Log("Enemy hit");
        }
    }
    
}