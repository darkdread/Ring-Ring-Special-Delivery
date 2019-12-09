using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class ReturnControlPlayer : MonoBehaviour
{
    public GameObject virtualCams;
    public GameObject realPlayer;

    public CinemachineBrain mainBrain;
    
    public Animator playerAnimator;

    // public ThirdPersonCamera thirdPersonCamera
    private PlayableDirector playableDirector;

    private float cutsceneTimer;
    private bool controlReturned;

    // Start is called before the first frame update
    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        GameManager.isInCutscene = true;
    }

    // Update is called once per frame
    void Update()
    {
        cutsceneTimer += Time.deltaTime;

        if(cutsceneTimer > 5.0f && !controlReturned)
        {
            ReturnControl();
        }
    }

    private void ReturnControl()
    {
        Destroy(playerAnimator.gameObject);
        Destroy(mainBrain);
        
        // Destroy(playableDirector);
        virtualCams.SetActive(false);
        controlReturned = true;

        // Invoke("EnableAnimator", 2f);
        realPlayer.SetActive(true);

		GameManager.isInCutscene = false;
    }

}
