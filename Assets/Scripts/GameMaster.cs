using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public Unit selectedunit;

    [SerializeField] Color highlightColor;
    public static Color static_highlightColor;

    public GameObject highlighttile;

    public int playerturn;

    public bool paused=true;
    public GameObject pausescreen,winscreen;
    public TMP_Text text,win_text;

    [SerializeField]
    AudioSource audioSource;

    public Slider slider_value;

    public static GameMaster gm { get; private set; }
    private void Awake()
    {
        if (gm != null)
            Destroy(this);
        else
            gm = this;

        static_highlightColor = highlightColor;
    }

    public void ResetTiles()
    {
        if (selectedunit == null)
            return;
        foreach (Node node in selectedunit.path)
        {
            RaycastHit2D[] hitinfo = Physics2D.RaycastAll(node.Position, Vector3.forward * 1.5f);
            foreach (RaycastHit2D hit in hitinfo)
                if (hit.collider.gameObject.name.Contains("Plain"))
                    hit.collider.GetComponent<Tiles>().reset();
        }
    }

    public void switchturn()
    {
        if (playerturn == 1)
            playerturn = 2;
        else if (playerturn == 2)
            playerturn = 1;

        ResetTiles();
        if (selectedunit != null)
        {
            selectedunit.selected = false;
            selectedunit = null;
        }
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.hasmoved = false;
            unit.Resetenemy();
            unit.hasattacked = false;
            unit.enemylist.Clear();
        }
    }

    public void gamewondisplay(int playerturn)
    {
        win_text.text="Player " + playerturn + " Won";
        winscreen.SetActive(true);
    }

    private void Update()
    {
        if (selectedunit != null)
        {
            highlighttile.SetActive(true);
            highlighttile.transform.position = selectedunit.transform.position;
        }
        else
            highlighttile.SetActive(false);
    }

    public void pauseScreen()
    {
        pausescreen.SetActive(paused);
        if (paused)
            text.text = "Unpause";
        else
            text.text = "Pause";

        paused = !paused;
    }

    private void Start()
    {
        audioSource = FindObjectOfType<AudioSource>();
        slider_value.value = audioSource.volume;
    }

    public void onvaluechanged(float value)
    {
        audioSource.volume = value;
    }

    public void exittomain()
    {
        Destroy(audioSource.gameObject);
        SceneManager.LoadSceneAsync(0);   
    }

}
