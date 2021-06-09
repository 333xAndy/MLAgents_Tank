using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class agentGun : Agent
{
  private Rigidbody m_Turrent;
  private bool gunCoolDown = true;

  [SerializeField] private string enemy;
  [SerializeField] private GameObject bullet;
  [SerializeField] private Transform originPoint;
  private float scrollSpeed = 100f;
  private float timeBetweenShots = 1f;
  EnvironmentParameters m_Reset;

    public override void Initialize()
    {
        m_Turrent = GetComponent<Rigidbody>();
        m_Reset = Academy.Instance.EnvironmentParameters;
        GetComponent<RayPerceptionSensorComponent3D>().DetectableTags[0] = enemy;
    }

    public override void OnEpisodeBegin()
    {
       Vector3 rTurrent = new Vector3(0,0,0);
       transform.rotation = Quaternion.Euler(rTurrent);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        moveTop(actions.DiscreteActions);
    }

    private void moveTop(ActionSegment<int> act){
        float turnValue = act[0];
        float turn = turnValue * scrollSpeed * Time.deltaTime;
        Quaternion turnRotate = Quaternion.Euler(0f, turn, 0f);
        m_Turrent.MoveRotation(m_Turrent.rotation * turnRotate);

        if(act[1] >= 0.5f)
            Shoot();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;
        var shootActions = actionsOut.DiscreteActions;
        shootActions[1] = 0;
        int turn = 0;
        int shoot = 0;

        if(Input.GetKey(KeyCode.Q)){
            turn = 1;
        }
        else if(Input.GetKey(KeyCode.E)){
            turn = -1;
        }
        else if(Input.GetKey(KeyCode.X))
            shoot = 1;

        discreteActionsOut[0] = turn;
        shootActions[1] = shoot;
    }

    private void Shoot(){
        if(!gunCoolDown)
            return;
        Instantiate(bullet, originPoint);
        
        int layerMask = 1 << LayerMask.NameToLayer(enemy);
        Vector3 direction = transform.forward;
        RaycastHit hit;
    
        if(Physics.Raycast(transform.position, transform.forward, out hit, 15)){
            if(hit.transform.tag == enemy){
                 Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                 AddReward(1f);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, direction*50f, Color.red, 1f);
            AddReward(-0.03f);
        }
      
        gunCoolDown  = false;
        StartCoroutine(Reloading(timeBetweenShots));
    }
    private IEnumerator Reloading(float t){
        yield return new WaitForSeconds(t);
        gunCoolDown = true;
        yield return null;
    }
}
