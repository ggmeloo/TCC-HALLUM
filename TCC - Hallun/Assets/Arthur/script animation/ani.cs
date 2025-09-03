using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    private Animator anim;
    public AudioSource jumpScareSound;
    public GameObject jumpScareImage;
    public enum Events
    {
        WalkInRoof,
        JumpScareImage
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayAnimWalkInRoof()
    {
        anim.SetTrigger("walk");
        StartCoroutine("DestroyInSeconds", 4f);
    }

    public void PlayJumpScareImage()
    {
        jumpScareImage.SetActive(true);
        jumpScareSound.Play();
        StartCoroutine("LoadSceneInSeconds", 5f);
    }

    private IEnumerator DestroyInSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private IEnumerator LoadSceneInSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("GameOver");
    }
}