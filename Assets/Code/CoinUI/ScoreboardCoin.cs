using UnityEngine;
using System.Collections;

public class ScoreboardCoin : MonoBehaviour 
{
    public GameObject ScoreboardCoinPrefab;

    private GameObject mScoreboardCoin = null;
    private Rigidbody mScoreboardCoinRigidbody;

    private static Camera sCoinCam;

    public static Camera CoinCam
    {
        get
        {
            return sCoinCam;
        }
    }

    private void Start()
    {
        if (mScoreboardCoin == null)
        {
            mScoreboardCoin = GameObject.Instantiate(ScoreboardCoinPrefab);
            mScoreboardCoin.transform.parent = transform;
            mScoreboardCoinRigidbody = mScoreboardCoin.GetComponent<Rigidbody>();
            mScoreboardCoinRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            mScoreboardCoin.layer = LayerMask.NameToLayer("UI");
        }

        if (sCoinCam == null)
        {
            sCoinCam = new GameObject("Coin Camera", typeof(Camera)).GetComponent<Camera>();
            sCoinCam.transform.parent = transform;
            sCoinCam.enabled = false;
            sCoinCam.cullingMask = 0;
            sCoinCam.cullingMask = 1 << LayerMask.NameToLayer("UI");
            sCoinCam.targetTexture = new RenderTexture(128, 128, 16);
        }
    }

    private void LateUpdate()
    {

    }
}
