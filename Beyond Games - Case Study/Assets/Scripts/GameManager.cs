using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public int ballCount = 0;

    private const float DELAY = 0.3f;

    private Transform _effects;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        _effects = transform.GetChild(1);
    }

    public IEnumerator ExtractBalls(int amount)
    {
        yield return new WaitForSeconds(DELAY);

        ballCount -= amount;

        if (ballCount <= 0)
        {
            foreach (ParticleSystem _particleSystem in _effects.GetComponentsInChildren<ParticleSystem>())
            {
                _particleSystem.Play();
            }

            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
