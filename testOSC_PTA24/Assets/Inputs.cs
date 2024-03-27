using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public enum InputsKeyEnum
{
    none,
    _0, _1, _2, _3, _4, _5, _6, _7, _8, _9,
    a, b, c, d, e, f, g, h, i, j, k, l, m, n, ñ, o, p, q, r, s, t, u, v, w, x, y, z,
    space,

}

public class Inputs : MonoBehaviour////OSC
{
    public OSC osc;
    public int belly = 0;
    public int chest = 0;
    public int calibration = 0;
    public int lastPotValue = 0;

    public static Inputs instance; // Instancia estática

    [SerializeField] AudioSource audioS;
    [SerializeField] AudioClip[] clips;
    [Header("Adjuntar el script que controla la entrada del micrófono")]
    [Header("Indicar el minimo de sonido detectable")]
    [Range(0.00f, 2.00f)] public float minLoudnessDetection = 0.02f;

    // Para los inputs de cada escena escritos en codigo
    private Coroutine activeSceneInputs;

    // Para las coroutinas seleccionadas desde una enumeracion del editor
    private List<CoroutineInput> enumerationsInputsEnabled = new List<CoroutineInput>();

    public struct CoroutineInput
    {
        public Coroutine inputCoroutine;
        public InputsKeyEnum inputEnum;
    }

    ///////////////////////////////////////////////////////////////////////////////////////
    ////////////////////             Metodos arduino            ///////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
//private void Update()
//    {
//        if (chest == 2) { Debug.Log("CHEST,2"); }
//        if (chest == 1) { Debug.Log("CHEST,1"); }

//    }
    ///////////////////////////////////////////////////////////////////////////////////////
    ////////////////////            Fin metodos arduino                        ////////////
    //// ----------------------------------------------------------------------------  ////
    ////////////////////        Inicio de métodos comunes de Inputs            ////////////
    ///////////////////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        osc.SetAddressHandler("/calibration", OnCalibration);
        osc.SetAddressHandler("/belly", OnBelly);
        osc.SetAddressHandler("/chest", OnChest);
        //Debug.Log("OnBelly");

        EnableInputs();
        AudioClip audioClip;
    }

    private void OnCalibration(OscMessage message)
    {
        lastPotValue = message.GetInt(0);
        RvInputs.belly_Crude = lastPotValue;
        Debug.Log("RAW VALUE: " + lastPotValue);
    }

    private void OnChest(OscMessage message)
    {
        int receivedChestValue = message.GetInt(0);
        chest = receivedChestValue;
        RvInputs.chest_Calibrated = receivedChestValue; // Update the calibrated value
        //Debug.Log(chest);
        Debug.Log("RAW CAL: " + receivedChestValue);
    }

    private void OnBelly(OscMessage message)
    {
        //lastPotValue = message.GetInt(0);
        //int receivedBellyValue = message.GetInt(0);
        //belly = receivedBellyValue;
        //RvInputs.belly_Crude = receivedBellyValue; // Update the calibrated value
        ////Debug.Log(belly);
        //Debug.Log("RAW VALUE: " + lastPotValue);
    }

    private void Update()
    {
        //if (lastPotValue > 100)
        if (RvInputs.chest_Calibrated == 2) 
        { 
            //Debug.Log("CHEST, 2");
            //Debug.Log("RvInputs.chest_Calibrated2222: " + RvInputs.chest_Calibrated);
        }

        if (RvInputs.chest_Calibrated == 1) 
        { 
            //Debug.Log("CHEST, 1");
            //
            //Debug.Log("RvInputs.chest_Calibrated1111: " + RvInputs.chest_Calibrated);
        }

        if (RvInputs.belly_Crude > 100)
        {
            //Debug.Log("BELLY, 2");
            //Debug.Log("RvInputs.chest_Calibrated2222: " + RvInputs.chest_Calibrated);
        }

        if (RvInputs.belly_Crude <=50)
        {
            //Debug.Log("BELLY, 1");
            //
            //Debug.Log("RvInputs.chest_Calibrated1111: " + RvInputs.chest_Calibrated);
        }

    }


    // Cada vez que la escena carga
    private void OnLevelWasLoaded(int level)
    {
        DisableInputs();
        EnableInputs();
    }

    private void DisableInputs()
    {
        StopCoroutine(activeSceneInputs);
        //JBMicController.instance.gameObject.SetActive(false);
    }

    private void EnableInputs()
    {
        //switch (SceneDataManager.instance.GetActualScene())
        //{
        //    case SceneEnum._00_Calibration:
        //        activeSceneInputs = (StartCoroutine(CheckSceneInputs(_00_Inputs)));
        //        break;
        //    case SceneEnum._01_City:
        //        activeSceneInputs = (StartCoroutine(CheckSceneInputs(_01_Inputs)));
        //        break;
        //    case SceneEnum._02_Shader:
        //        activeSceneInputs = (StartCoroutine(CheckSceneInputs(_02_Inputs)));
        //        break;
        //    case SceneEnum._03_Forest:
        //        activeSceneInputs = (StartCoroutine(CheckSceneInputs(_03_Inputs)));
        //        break;
        //}
    }

    private void _Shared_Inputs()
    {                                 // Clase | Uso
        if (Input.GetKeyDown("space"))
        {
            AllKeyInputs.space.Invoke();
            Debug.Log("Espacio llamado");
        } // SceneLoadController | cambiar de escena
        if (Input.GetKey("n")) { AllKeyInputs.n.Invoke(); }; // ActivarBody | Prender()
        if (Input.GetKey("m")) { AllKeyInputs.m.Invoke(); }; // ActivarBody | Apagar()
    }

    private void _00_Inputs()
    {//esc cal
        if (Input.GetKey("x")) { AllKeyInputs.m.Invoke(); }; // ManosCintura | EnableMeshRenderer()
        if (Input.GetKey("y")) { AllKeyInputs.m.Invoke(); }; // ManosCintura | DisableMeshRenderer()
        //JBMicController.instance.gameObject.SetActive(true);
    }
    private void _01_Inputs()
    {//esc city
        //JBMicController.instance.gameObject.SetActive(true);
    }
    private void _02_Inputs()
    {//esc shader
        //if (Input.GetKey("5")) ShaderManager.instance.ExecuteInstruction(Sn_02_Instructions_Steps_enum.Instruction_05,false);
        //if (Input.GetKey("6")) ShaderManager.instance.ExecuteInstruction(Sn_02_Instructions_Steps_enum.Instruction_06,false);
        //if (Input.GetKey("7")) ShaderManager.instance.ExecuteInstruction(Sn_02_Instructions_Steps_enum.Instruction_07,false);
        //if (Input.GetKey("8")) ShaderManager.instance.ExecuteInstruction(Sn_02_Instructions_Steps_enum.Instruction_08,false);

    }
    private void _03_Inputs()
    {//esc forest

    }

    // Esta coroutina ejecuta una función dada en cada frame
    IEnumerator CheckSceneInputs(Action input)
    {
        while (true)
        {
            _Shared_Inputs(); // Inputs compartidos en todas las escenas
            input.Invoke(); // Inputs obtenidos por parametro
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Para agregar el checkeo de un input por enumeracion
    /// </summary>
    public void AddInputCheckFromEnum(InputsKeyEnum inputsKeyEnum, UnityAction handler)
    {

        // Obtenemos el evento del input y su string
        ReturnInputEventAndKeyFromEnum(out UnityEvent uEvent, out string key, inputsKeyEnum);

        if (uEvent != null)
        {
            // Ponemos el handler en el evento recibido
            uEvent.AddListener(handler);

            // Por último creamos una referencia a ese llamado del input
            enumerationsInputsEnabled.Add(
                new CoroutineInput
                {
                    inputCoroutine = StartCoroutine(CheckInput(uEvent, key)),
                    inputEnum = inputsKeyEnum
                });
        }
    }

    /// <summary>
    /// Para desactivar el checkeo de un input por enumeración
    /// </summary>
    public void RemoveInputCheckFromEnum(InputsKeyEnum inputsKeyEnum, UnityAction handler)
    {
        // Obtenemos el evento del input y su string
        ReturnInputEventAndKeyFromEnum(out UnityEvent uEvent, out string key, inputsKeyEnum);

        if (uEvent != null)
        {
            // Removemos el handler en el evento recibido
            uEvent.RemoveListener(handler);

            // Buscamos el elemento para quitar su checkeo del input
            int inputIndex = 0;
            foreach (CoroutineInput cInput in enumerationsInputsEnabled)
            {
                if (cInput.inputEnum == inputsKeyEnum)
                {
                    StopCoroutine(cInput.inputCoroutine);
                    // Quitamos el elemento de la lista (puede ser esto lo que falta)
                    enumerationsInputsEnabled.RemoveAt(inputIndex);

                    break;
                }
                inputIndex++;
            }

            //// Y por último lo sacamos de la lista
            //enumerationsInputsEnabled.RemoveAt(inputIndex);
        }
    }

    private void ReturnInputEventAndKeyFromEnum(out UnityEvent uEvent, out string key, InputsKeyEnum input)
    {
        switch (input)
        {

            // Números
            // Otras teclas
            case InputsKeyEnum.none:
                uEvent = null;
                key = null;
                break;
            case InputsKeyEnum._0:
                uEvent = AllKeyInputs._0;
                key = "0";
                break;
            case InputsKeyEnum._1:
                uEvent = AllKeyInputs._1;
                key = "1";
                break;
            case InputsKeyEnum._2:
                uEvent = AllKeyInputs._2;
                key = "2";
                break;
            case InputsKeyEnum._3:
                uEvent = AllKeyInputs._3;
                key = "3";
                break;
            case InputsKeyEnum._4:
                uEvent = AllKeyInputs._4;
                key = "4";
                break;
            case InputsKeyEnum._5:
                uEvent = AllKeyInputs._5;
                key = "5";
                break;
            case InputsKeyEnum._6:
                uEvent = AllKeyInputs._6;
                key = "6";
                break;
            case InputsKeyEnum._7:
                uEvent = AllKeyInputs._7;
                key = "7";
                break;
            case InputsKeyEnum._8:
                uEvent = AllKeyInputs._8;
                key = "8";
                break;
            case InputsKeyEnum._9:
                uEvent = AllKeyInputs._9;
                key = "9";
                break;

            // Letras
            case InputsKeyEnum.a:
                uEvent = AllKeyInputs.a;
                key = "a";
                break;
            case InputsKeyEnum.b:
                uEvent = AllKeyInputs.b;
                key = "b";
                break;
            case InputsKeyEnum.c:
                uEvent = AllKeyInputs.c;
                key = "c";
                break;
            case InputsKeyEnum.d:
                uEvent = AllKeyInputs.d;
                key = "d";
                break;
            case InputsKeyEnum.e:
                uEvent = AllKeyInputs.e;
                key = "e";
                break;
            case InputsKeyEnum.f:
                uEvent = AllKeyInputs.f;
                key = "f";
                break;
            case InputsKeyEnum.g:
                uEvent = AllKeyInputs.g;
                key = "g";
                break;
            case InputsKeyEnum.h:
                uEvent = AllKeyInputs.h;
                key = "h";
                break;
            case InputsKeyEnum.i:
                uEvent = AllKeyInputs.i;
                key = "i";
                break;
            case InputsKeyEnum.j:
                uEvent = AllKeyInputs.j;
                key = "j";
                break;
            case InputsKeyEnum.k:
                uEvent = AllKeyInputs.k;
                key = "k";
                break;
            case InputsKeyEnum.l:
                uEvent = AllKeyInputs.l;
                key = "l";
                break;
            case InputsKeyEnum.m:
                uEvent = AllKeyInputs.m;
                key = "m";
                break;
            case InputsKeyEnum.n:
                uEvent = AllKeyInputs.n;
                key = "n";
                break;
            case InputsKeyEnum.ñ:
                uEvent = AllKeyInputs.ñ;
                key = "ñ";
                break;
            case InputsKeyEnum.o:
                uEvent = AllKeyInputs.o;
                key = "o";
                break;
            case InputsKeyEnum.p:
                uEvent = AllKeyInputs.p;
                key = "p";
                break;
            case InputsKeyEnum.q:
                uEvent = AllKeyInputs.q;
                key = "q";
                break;
            case InputsKeyEnum.r:
                uEvent = AllKeyInputs.r;
                key = "r";
                break;
            case InputsKeyEnum.s:
                uEvent = AllKeyInputs.s;
                key = "s";
                break;
            case InputsKeyEnum.t:
                uEvent = AllKeyInputs.t;
                key = "t";
                break;
            case InputsKeyEnum.u:
                uEvent = AllKeyInputs.u;
                key = "u";
                break;
            case InputsKeyEnum.v:
                uEvent = AllKeyInputs.v;
                key = "v";
                break;
            case InputsKeyEnum.w:
                uEvent = AllKeyInputs.w;
                key = "w";
                break;
            case InputsKeyEnum.x:
                uEvent = AllKeyInputs.x;
                key = "x";
                break;
            case InputsKeyEnum.y:
                uEvent = AllKeyInputs.y;
                key = "y";
                break;
            case InputsKeyEnum.z:
                uEvent = AllKeyInputs.z;
                key = "z";
                break;
            // Otras teclas
            case InputsKeyEnum.space:
                uEvent = AllKeyInputs.space;
                key = "space";
                break;
            default:
                Debug.LogError("La tecla solicitada no está implementada en el Switch Case");
                uEvent = null;
                key = null;
                break;
        }
    }


    /// <summary>
    /// Coroutine para checkear inputs por enumeraciones
    /// </summary>
    public IEnumerator CheckInput(UnityEvent uEvent, string key)
    {
        while (true)
        {
            if (Input.GetKey(key))
            {
                uEvent.Invoke();
            }
            yield return new WaitForFixedUpdate();
        }
    }





} // Fin de la clase Inputs


/// <summary>
/// Inputs de Arduino, se manejan desde la clase Inputs
/// </summary>
public static class RvInputs
{
    public static int chest_Calibrated;
    public static int belly_Calibrated;

    public static float chest_Crude;
    public static float belly_Crude;

    public static bool mic_Input;
}

/// <summary>
/// Estos inputs se controlan de la clase Inputs, agregar los que sean necesarios
/// </summary>
public static class AllKeyInputs
{
    public static UnityEvent[] allInputsEvets = {
    _0, _1, _2, _3, _4, _5, _6, _7, _8, _9,
    a,b,c,d,e,f,g,h,i,j,k,l,m,n,ñ,o,p,q,r,s,t,u,v,w,x,y,z,
    space,
    };

    // Numeros
    public static UnityEvent _0 = new UnityEvent();
    public static UnityEvent _1 = new UnityEvent();
    public static UnityEvent _2 = new UnityEvent();
    public static UnityEvent _3 = new UnityEvent();
    public static UnityEvent _4 = new UnityEvent();
    public static UnityEvent _5 = new UnityEvent();
    public static UnityEvent _6 = new UnityEvent();
    public static UnityEvent _7 = new UnityEvent();
    public static UnityEvent _8 = new UnityEvent();
    public static UnityEvent _9 = new UnityEvent();

    // Letras
    public static UnityEvent a = new UnityEvent();
    public static UnityEvent b = new UnityEvent();
    public static UnityEvent c = new UnityEvent();
    public static UnityEvent d = new UnityEvent();
    public static UnityEvent e = new UnityEvent();
    public static UnityEvent f = new UnityEvent();
    public static UnityEvent g = new UnityEvent();
    public static UnityEvent h = new UnityEvent();
    public static UnityEvent i = new UnityEvent();
    public static UnityEvent j = new UnityEvent();
    public static UnityEvent k = new UnityEvent();
    public static UnityEvent l = new UnityEvent();
    public static UnityEvent m = new UnityEvent();
    public static UnityEvent n = new UnityEvent();
    public static UnityEvent ñ = new UnityEvent();
    public static UnityEvent o = new UnityEvent();
    public static UnityEvent p = new UnityEvent();
    public static UnityEvent q = new UnityEvent();
    public static UnityEvent r = new UnityEvent();
    public static UnityEvent s = new UnityEvent();
    public static UnityEvent t = new UnityEvent();
    public static UnityEvent u = new UnityEvent();
    public static UnityEvent v = new UnityEvent();
    public static UnityEvent w = new UnityEvent();
    public static UnityEvent x = new UnityEvent();
    public static UnityEvent y = new UnityEvent();
    public static UnityEvent z = new UnityEvent();

    // Otras teclas
    public static UnityEvent space = new UnityEvent();
}