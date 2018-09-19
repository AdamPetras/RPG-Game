using UnityEngine;
using System.Collections;
using System.Security.Policy;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
    private static Image _blackScreenImage;
    private static BlackScreen instance;
    public static bool Visible;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _blackScreenImage = GetComponent<Image>();
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void Print(float sec)
    {
        instance.StartCoroutine("foo", sec);
        Visible = true;
    }
    public static void Print()
    {
        _blackScreenImage.enabled = true;
        Visible = true;
    }
    public static void EndPrint()
    {
        _blackScreenImage.enabled = false;
        Visible = false;
    }

    private static void End()
    {
        _blackScreenImage.enabled = false;
        Visible = false;
        instance.StopCoroutine("foo");
    }

    private IEnumerator foo(float sec)
    {
        _blackScreenImage.enabled = true;
        Debug.Log(sec);
        yield return new WaitForSeconds(sec);
        End();
    }
}
