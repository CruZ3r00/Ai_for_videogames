using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BBAgent : Agent {
    [Header("Specific to Balance Board")]
    public GameObject ball;                    // a reference to the ball
    public float maxTilt = 0.25f;              // maximum tilt (0.25 π rad = 45˚)

    private Rigidbody ballRbProxy;             // shorthand to the ball Rigidbody
    private EnvironmentParameters envParProxy; // shortgand to the env Parameters

    // Initializing the system
    public void Start() {
        ballRbProxy = ball.GetComponent<Rigidbody>();   
        envParProxy = Academy.Instance.EnvironmentParameters;
        GetTrainerParameters ();
    }

    // This method is run when a sequence begins. i.e., we start trying to balance the ball
    public override void OnEpisodeBegin () {
        // fist, we set the board to a sligtly random inclination 
        gameObject.transform.rotation = new Quaternion (0f, 0f, 0f, 0f);
        gameObject.transform.Rotate (new Vector3 (1, 0, 0), Random.Range (-10f, 10f));
        gameObject.transform.Rotate (new Vector3 (0, 0, 1), Random.Range (-10f, 10f));
        // second, we make sure the ball is still
        ballRbProxy.linearVelocity = new Vector3 (0f, 0f, 0f);
        // third, we place the ball above the plane and off-center by a random value
        ball.transform.position = new Vector3 (Random.Range (-1.5f, 1.5f), 2f, Random.Range (-1.5f, 1.5f))
            + gameObject.transform.position; // the ball is detached from the agent, so we must consider
                                             // the agent position
        // last, we must set new random parameters when the Agent is reset
        GetTrainerParameters ();
    }

    // This method is inspecting the world and fill the observation vector
    // the number of inserted element MUST be the same as the Space Size parameter
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(gameObject.transform.rotation.z); // one float
        sensor.AddObservation(gameObject.transform.rotation.x); // one float
        sensor.AddObservation(ball.transform.position - gameObject.transform.position); // three floats
        sensor.AddObservation(ballRbProxy.linearVelocity); // three floats
    }

    // This method is called when the Neural Network has some directions about what to do
    // The vector used as parameter will have the same size as the Space Size parameter
    public override void OnActionReceived(ActionBuffers actionBuffers) {
        float xAction = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);
        float zAction = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);

        float xTilt = gameObject.transform.rotation.x;
        float zTilt = gameObject.transform.rotation.z;

        if ((xTilt < maxTilt && xAction > 0f) || (xTilt > -maxTilt && xAction < 0f)) {
            gameObject.transform.Rotate (Vector3.right, xAction);
        }

        if ((zTilt < maxTilt && zAction > 0f) || (zTilt > -maxTilt && zAction < 0f)) {
            gameObject.transform.Rotate (Vector3.forward, zAction);
        }

        if ((ball.transform.position - gameObject.transform.position).magnitude < 4f) {
            // it the ball is still on the platform, we are happy, give a reward, and go on
            SetReward (0.1f);
        } else {
            // otherwise, we penalize the current configuration
            SetReward (-1f);
            // and we end the run (a new one will be restarted)
            EndEpisode ();
        }
    }

    // This method will be called when Behavior Type is set to Heuristic.
    // In this case, the agent will learn by observing the user playing
    public /* override */ void Heuristic (float [] actionsOut) {
        actionsOut [0] = Input.GetAxis ("Vertical");
        actionsOut [1] = -Input.GetAxis ("Horizontal");
    }

    // This method (external to the interface) can be useful when you want the training
    // system to generate random parameters.
    // In the .yaml file, you can add a section like the following:
    // 
    // parameter_randomization:
    //   mass:
    //     sampler_type: uniform
    //     sampler_parameters:
    //       min_value: 0.5
    //       max_value: 10
    //   scale:
    //     sampler_type: uniform
    //     sampler_parameters:
    //       min_value: 0.75
    //       max_value: 3
    //
    public void GetTrainerParameters() {
        ballRbProxy.mass = envParProxy.GetWithDefault ("mass", 1.0f);
        float scale = envParProxy.GetWithDefault ("scale", 1.0f);
        ball.transform.localScale = new Vector3 (scale, scale, scale);
    }
}
