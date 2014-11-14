using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
    public GameObject PlayerCoinPrefab;
    public BoxCollider CoinBounds;
    public LayerMask CoinBoundsLayerMask;
    public float CoinForce = 25f;
    public Vector3 PlayerCoinStartingRotation = new Vector3(90f, 0f, 0f);
    public float PlayerCoinDistanceFromCamera = 10f;
    public float ShotDelay = 5f;
    public Vector3 ShotReloadOffset = new Vector3(0.1f, 0.1f, 0f);


    private Ray mRay;
    private RaycastHit mHit;

    private GameObject mPreviewCoin;
    private Renderer mPreviewRenderer;
    private float mCountHeight = 0f;
    private Camera mCamera;
    private GameObject mInventoryCoin;

    private float mCurrentReloadTime = 0f;
    private Vector3 mCurrentReloadPosition = Vector3.zero;

    private void Start()
    {
        mCamera = Camera.main;
        mPreviewCoin = (GameObject)GameObject.Instantiate(PlayerCoinPrefab);
        mPreviewRenderer = mPreviewCoin.GetComponent<MeshRenderer>();

        foreach (Material mat in mPreviewRenderer.materials)
        {
            mat.color = Color.Lerp(mat.color, Color.clear, 0.45f);
        }
     
        mPreviewCoin.rigidbody.isKinematic = true;
        mPreviewCoin.transform.rotation = Quaternion.Euler(PlayerCoinStartingRotation);
        mCountHeight = mPreviewRenderer.bounds.extents.y;
        mCurrentReloadPosition = mCamera.ViewportToWorldPoint(ShotReloadOffset);

        mInventoryCoin = (GameObject)GameObject.Instantiate(PlayerCoinPrefab);
        mInventoryCoin.rigidbody.isKinematic = true;
        mInventoryCoin.collider.enabled = false;
        mInventoryCoin.transform.position = mCamera.ViewportToWorldPoint(ShotReloadOffset);
   
    }

	private void LateUpdate()
    {
       // mRay = new Ray(mCamera.transform.position, mCamera.transform.forward);
       // mHit = new RaycastHit();

//        if (Physics.Raycast(mRay, out mHit, 100f, CoinBoundsLayerMask))
//        {
//            Vector3 point = mHit.point;
//            point.y = mHit.collider.transform.position.y + mCountHeight;
//            mPreviewCoin.transform.position = point;
//            mPreviewCoin.transform.forward = Camera.main.transform.forward;
//            mPreviewCoin.transform.Rotate(PlayerCoinStartingRotation);
//            mPreviewRenderer.enabled = true;
//        }

        DrawInventory();

        if (mCurrentReloadTime < ShotDelay)
        {
            mCurrentReloadTime += Time.deltaTime;

            float time = mCurrentReloadTime / ShotDelay;

            mPreviewCoin.transform.position = Vector3.Slerp(mCurrentReloadPosition, mCamera.transform.position + (mCamera.transform.forward * PlayerCoinDistanceFromCamera), time);
            mPreviewRenderer.transform.forward = Vector3.Slerp(Vector3.down, mCamera.transform.forward, time);
            mPreviewCoin.transform.rotation = Quaternion.Slerp(Quaternion.Euler(-PlayerCoinStartingRotation), Quaternion.Euler(PlayerCoinStartingRotation), time);
            return;
        }

        mPreviewCoin.transform.position = (mCamera.transform.position + (mCamera.transform.forward * PlayerCoinDistanceFromCamera));
        mPreviewCoin.transform.forward = mCamera.transform.forward;
        mPreviewCoin.transform.Rotate(PlayerCoinStartingRotation);

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            SpawnCoin();
            Reload();
        }
    }

    private void DrawInventory()
    {
        mInventoryCoin.transform.position = mCamera.ViewportToWorldPoint(ShotReloadOffset);
        mInventoryCoin.transform.forward = mCamera.transform.forward;
        mInventoryCoin.transform.Rotate(-PlayerCoinStartingRotation);
    }

    private void Reload()
    {
        mCurrentReloadTime = 0;
        mCurrentReloadPosition = mCamera.ViewportToWorldPoint(ShotReloadOffset);
        mPreviewCoin.transform.position = mCurrentReloadPosition;
    }

    private void SpawnCoin()
    {
        Vector3 pos = mPreviewCoin.transform.position;
        Quaternion rot = mPreviewCoin.transform.rotation;

        GameObject playerCoin = (GameObject)GameObject.Instantiate(PlayerCoinPrefab, pos, rot);

        playerCoin.rigidbody.AddForceAtPosition(mCamera.transform.forward * CoinForce, playerCoin.transform.position, ForceMode.VelocityChange);
    }
}
