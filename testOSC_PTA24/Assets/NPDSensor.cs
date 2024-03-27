using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LibPdInstance))]
//[RequireComponent(typeof(SoundController))]
[ExecuteAlways]
public class NPDSensor : MonoBehaviour
{
    // instancia estatica
    public static NPDSensor instance2;

    [Header("Coeficientes")]
    [Range(0, 10)] public int inhale;
    [Range(0, 10)] public int exhale;

    [Header("Adjuntar controllers")]
    //public SoundController soundController;
    public LibPdInstance libPdInstance;

    [Header("Determinar volumen inicial para cada escena")]
    [Range(0, 1)] public float calVolume ;//EMPIEZE EN 0 Y SUBA CUANDO LA SPHRE IS TRUE
    //[Range(0, 1)] public float calVolume; 
    [Range(0, 1)] public float cityVolume;
    [Range(0, 1)] public float shaderVolume;
    [Range(0, 1)] public float forestVolume;

    //public sphereHalo spherehalo;
    //public SphereC sphereSha;


    // Cada vez que iniciemos haremos que la instancia sea el pdSensor de la escena
    private void Awake()
    {
        if (Application.isPlaying)
        {
            if (instance2 == null)
            {
                instance2 = this;
            }
        }
        //if (soundController == null) { soundController = GetComponent<SoundController>(); }
        if (libPdInstance == null) { libPdInstance = GetComponent<LibPdInstance>(); }

        if (Application.isPlaying)
        {
            StartCoroutine(ListenToActivate()); ;
        }
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) // Verifica si la tecla "up" ha sido presionada
        {

            calVolume = 1f; // Establece el valor de calVolume en 1
            ChangeVolume(calVolume, 1f); // Cambia el volumen a 1 en 1 segundo
            //ChangeVolume(1f, 1f); // Cambia el volumen a 1 en 1 segundo

        }
        //    if (RvInputs.chest_Calibrated == 2) EstAct = 1;
        //    if (RvInputs.chest_Calibrated == 1) EstAct = 0;

        //    if (EstAct == 1 && EstAct != EstAnt)
        //    {
        //        //Debug.Log($"RvInputs.chest_Calibrated2 = {RvInputs.chest_Calibrated}");
        //        RvInputs.chest_Calibrated = 2;
        //        SoundUp();
        //    }
        //    if (EstAct == 0 && EstAct != EstAnt)
        //    {
        //        //Debug.Log($"RvInputs.chest_Calibrated1= {RvInputs.chest_Calibrated}");
        //        RvInputs.chest_Calibrated = 1;
        //        SoundDown();
        //    }
        //    EstAnt = EstAct;
    }

    private void OnLevelWasLoaded(int level)
    {
        //switch (SceneDataManager.instance.actualScene)
        //{
        //    case SceneEnum._00_Calibration:
        //        ChangeVolume(calVolume, 1);
        //        break;
        //    case SceneEnum._01_City:
        //        ChangeVolume(cityVolume, 1);
        //        break;
        //    case SceneEnum._02_Shader:
        //        ChangeVolume(shaderVolume, 1);
        //        break;
        //    case SceneEnum._03_Forest:
        //        ChangeVolume(forestVolume, 1);
        //        break;
        //}
    }

    IEnumerator ListenToActivate()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            if (RvInputs.belly_Crude >= 100 )//200 plug in
            {                
                SoundUp();
            }
            if (RvInputs.belly_Crude < 50)//150 plug in
            {
                SoundDown();
            }

            if (RvInputs.chest_Calibrated == 2|| Input.GetKey("c"))
            {
                //spherehalo.IntensityUp();
                //spherehalo.RangeUp();
                //sphereSha.GemaBright();
                //Debug.Log("SPHERE UP");
                SoundUp();
            }
            if (RvInputs.chest_Calibrated == 1 )//|| Input.GetKey("c"))
            {
                //spherehalo.IntensityDown();
                //spherehalo.RangeDown();
                //sphereSha.GemaUnbright();
                //Debug.Log("SPHERE DOWN");
                SoundDown();
            }





            yield return new WaitForFixedUpdate();
        }
    }


    public void ChangeVolume(float destineValue, float secondsOfTransition)
    {
        //soundController.SetVolume(destineValue, secondsOfTransition);
    }

    public void SoundUp()
    {
        //libPdInstance.SendFloat("proximity", 0.9f);
        libPdInstance.SendBang("VolumeUp");
        //Debug.Log("BANG UP");
    }

    public void SoundDown()
    {
        //libPdInstance.SendFloat("proximity", 0.3f);
        libPdInstance.SendBang("VolumeDown");
        //Debug.Log("BANG DOWN");
    }


}
