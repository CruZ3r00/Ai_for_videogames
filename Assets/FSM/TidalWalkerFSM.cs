using UnityEngine;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using System.IO;

/*
    CLASSE CHE GESTISCE LA FSM DELL'AGENTE UTILIZZANDO LE CLASSI BASE VISTE A LEZIONE COME STRUTTURA DATI
    Utilizza AgentMovement, AgentController, WaterSystem, Graph e Terrain per gestire gli stati 
    e cambiare comportomento dell'agente
*/

public class TidalWalkerFSM : MonoBehaviour
{
    private FSM fsm;

    private AgentMovement movement;
    private AgentController controller;
    private WaterSystem water;
    private Graph graph;
    private Terrain terrain;

    [Header("Tidal Walker Settings")]

    public float safeHeight = 7.5f;
    public float checkInterval = 0.1f;

    //reference agli state
    private RecalculatePath recalculatePathState;
    private CheckSafe checkSafeState;
    private MoveToPath moveToPathState;
    private AvoidFlood avoidFloodState;
    private Stranded strandedState;

    private float previousWaterH = 0f;
    private float lastWaterHeight;

    private float nextTimeCheckSafe = 0f;
    public float checkSafeInterval = 0.3f; 

    public void Initialize()
    {
        movement = GetComponent<AgentMovement>();
        controller = GetComponent<AgentController>();
        water = FindAnyObjectByType<WaterSystem>();
        graph = movement.graph;
        terrain = movement.terrain;


        recalculatePathState = new RecalculatePath(transform, movement, controller, graph, terrain, water);
        checkSafeState = new CheckSafe(transform, movement, controller, graph, terrain, water);
        moveToPathState = new MoveToPath(transform, movement, controller, graph, terrain,water);
        avoidFloodState = new AvoidFlood(transform, movement, controller, graph, terrain,water);
        strandedState = new Stranded(transform, movement, controller, graph, terrain,water, this);

        BuildFSM();

        //start coroutine per avviare il loop dell'FSM
        StartCoroutine(FSMUpdateLoop());
    }

    IEnumerator FSMUpdateLoop()
    {
        while (true)
        {
            //chiamo update della classe base FSM per gestire l'attivazione degli stati
            fsm.Update();
            lastWaterHeight = water.transform.position.y;
            //attesa per rifare l'update
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void BuildFSM()
    {
        /*      inizializzazione stati      */

        //solo enter perche enter gestisce gli altri stati dello stato
        FSMState recalculatePath = new FSMState();
        recalculatePath.enterActions.Add(recalculatePathState.Enter);

        //entra ed esce subito
        FSMState checkSafe = new FSMState();
        checkSafe.enterActions.Add(checkSafeState.Enter);

        //due action
        FSMState moveToPath = new FSMState();
        moveToPath.enterActions.Add(moveToPathState.Enter);
        moveToPath.exitActions.Add(moveToPathState.Exit);

        //due action
        FSMState avoidFlood = new FSMState();
        avoidFlood.enterActions.Add(avoidFloodState.Enter);
        avoidFlood.stayActions.Add(avoidFloodState.Stay);
        avoidFlood.exitActions.Add(avoidFloodState.Exit);


        //
        FSMState stranded = new FSMState();
        stranded.enterActions.Add(strandedState.Enter);
        stranded.stayActions.Add(strandedState.Stay);
        
        /*      inizializzazione transizioni      */

        //recalculatepath -> checksafe
        //recalculatepath -> stranded
        recalculatePath.AddTransition(new FSMTransition(PathExists), checkSafe);
        recalculatePath.AddTransition(new FSMTransition(PathMissing), stranded);

        //checksafe -> movetopath
        // checksafe -> avoidflood
        checkSafe.AddTransition(new FSMTransition(PathSafe), moveToPath);
        checkSafe.AddTransition(new FSMTransition(PathUnSafe), avoidFlood);
        checkSafe.AddTransition(new FSMTransition(PathUnSafeWaterFalling),stranded);
        checkSafe.AddTransition(new FSMTransition(PathNull),recalculatePath);

        //movetopath -> recalculatepath (se il path diventa invalido)
        //movetopath -> avoidflood
        //movetopath -> recalculatepath (se il goal viene trovato)
        //transizione periodica 
        moveToPath.AddTransition( new FSMTransition(ReachedGoal), recalculatePath );
        moveToPath.AddTransition( new FSMTransition(TimeToCheck), recalculatePath );

        //avoidflood -> recalculatePath
        avoidFlood.AddTransition(new FSMTransition(WaitDone), recalculatePath);
        avoidFlood.AddTransition(new FSMTransition(Waiting), stranded);

        //strandend -> recalculatPath
        stranded.AddTransition(new FSMTransition(WaitDone), recalculatePath);

        // Start FSM in recalc
        fsm = new FSM(recalculatePath);

    }

    public bool WaterIsRising()
    {
        return water.transform.position.y > lastWaterHeight;
    }

    //se esiste il path si passa a check safe
    public bool PathExists()
    {
        return movement.path != null && movement.path.Count > 0;
    }

    public bool PathMissing()
    {
       return movement.path == null || movement.path.Count <= 0;
    }

    public bool PathSafe()
    {
        return checkSafeState.isSafe;
    }
    public bool PathUnSafe()
    {
        return !checkSafeState.isSafe && WaterIsRising() && !checkSafeState.recalc;
    }

    public bool PathUnSafeWaterFalling()
    {
        return !checkSafeState.isSafe && !WaterIsRising() && !checkSafeState.recalc;
    } 

    public bool PathNull(){
        return checkSafeState.recalc;
    }
    public bool ReachedGoal()
    {
        Vector3 goalWorld = controller.GetGoal();
        Vector3 flatGoal = new Vector3(goalWorld.x, 0, goalWorld.z);
        Vector3 flatPos  = new Vector3(transform.position.x, 0, transform.position.z);
        if (Vector3.Distance(flatPos, flatGoal) < 0.8f) return true;
        return false;        
    }

    public bool TimeToCheck()
    {
       if (Time.time >= nextTimeCheckSafe)
    {
        nextTimeCheckSafe = Time.time + checkSafeInterval;
        return true;
    }
    return false;
    }
    public bool WaitDone()
    {
        if(previousWaterH == 0f)
        {
            previousWaterH = water.transform.position.y;
            return false;
        }
        else
        {
            if(previousWaterH > water.transform.position.y) 
            {
                previousWaterH = 0f;
                return true;
            }
            else
            {
                previousWaterH = water.transform.position.y;
                return false;
            }
        }
    }
    public bool Waiting()
    {
        return avoidFloodState.reached;
    }
}