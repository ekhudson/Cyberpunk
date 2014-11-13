using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
    public GameObject PlayerCoinPrefab;
    public BoxCollider CoinBounds;
    public LayerMask CoinBoundsLayerMask;
    public float CoinForce = 25f;

    private Ray mRay;
    private RaycastHit mHit;

    private GameObject mPreviewCoin;
    private Renderer mPreviewRenderer;

    private void Start()
    {
        mPreviewCoin = (GameObject)GameObject.Instantiate(PlayerCoinPrefab);
        mPreviewRenderer = mPreviewCoin.GetComponentInChildren<MeshRenderer>();
        mPreviewCoin.renderer.enabled = false;
        mPreviewRenderer.enabled = false;
        mPreviewCoin.rigidbody.isKinematic = true;
    }

	private void Update()
    {
        mRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        mHit = new RaycastHit();

        if (Physics.Raycast(mRay, out mHit, 100f, CoinBoundsLayerMask))
        {
            Vector3 point = mHit.point;
            point.y = mHit.collider.transform.position.y;
            mPreviewCoin.transform.position = point;
            mPreviewCoin.transform.forward = mHit.normal;
            mPreviewRenderer.enabled = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        Vector3 pos = mPreviewCoin.transform.position;
        Quaternion rot = mPreviewCoin.transform.rotation;

        GameObject playerCoin = (GameObject)GameObject.Instantiate(PlayerCoinPrefab, pos, Quaternion.identity);

        playerCoin.rigidbody.AddRelativeForce(Camera.main.transform.forward * CoinForce, ForceMode.VelocityChange);
    }
}
