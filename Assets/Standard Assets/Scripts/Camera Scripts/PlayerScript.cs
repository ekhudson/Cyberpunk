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
    public Vector3 CoinCenterOffset = Vector3.zero;
    public float PanSpeed = 0.25f;
    public float OffsetMag = 2f;
    public float MaxAngularVelocity = 35f;

    private bool mNeedReload = false;
    private Ray mRay;
    private RaycastHit mHit;

    private GameObject mPreviewCoin;
    private Renderer mPreviewRenderer;
    private float mCountHeight = 0f;
    private Camera mCamera;
    private GameObject mInventoryCoin;
    private float mCoinRadius = 10f;

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

        mCoinRadius = PlayerCoinPrefab.renderer.bounds.extents.x;
   
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

        if (mNeedReload)
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
            {
                mNeedReload = false;
                Reload();
            }
        } 
        else
        {
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

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                CoinCenterOffset = Vector3.zero;
            }

            if (Input.GetMouseButton(2))
            {
                Pan();
            }

            if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
            {
                SpawnCoin();
                mNeedReload =  true;
               // Reload();
            }
        }
    }

    private void OnGUI()
    {
        Vector3 pos = mCamera.transform.position + (mCamera.transform.forward * PlayerCoinDistanceFromCamera);      

        pos += mCamera.transform.up * CoinCenterOffset.y;
        pos += mCamera.transform.right * CoinCenterOffset.x;
       

        //pos.y = mCamera.pixelHeight - pos.y;

        pos = mCamera.WorldToScreenPoint(pos);   

        
        pos.y -= 4f;
        pos.x -= 4f;

        GUI.color = Color.red;
        GUI.Label(new Rect(pos.x, pos.y, 8f, 8f), "X", GUI.skin.box);
        GUI.color = Color.white;

    }

    private void Pan()
    {
        CoinCenterOffset.x = Mathf.Clamp(CoinCenterOffset.x + Input.GetAxis("Mouse X") * PanSpeed, -mCoinRadius, mCoinRadius);
        CoinCenterOffset.y = Mathf.Clamp(CoinCenterOffset.y + Input.GetAxis("Mouse Y") * PanSpeed, -mCoinRadius, mCoinRadius);
        CoinCenterOffset = Vector3.ClampMagnitude(CoinCenterOffset, mCoinRadius);
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
        mPreviewRenderer.enabled = true;
        mCoinRadius = PlayerCoinPrefab.renderer.bounds.extents.x;
    }

    private void SpawnCoin()
    {
        Vector3 pos = mPreviewCoin.transform.position;
        Quaternion rot = mPreviewCoin.transform.rotation;

        mPreviewRenderer.enabled = false;

        GameObject playerCoin = (GameObject)GameObject.Instantiate(PlayerCoinPrefab, pos, rot);
        playerCoin.rigidbody.maxAngularVelocity = MaxAngularVelocity;

        Vector3 centerOffset = Vector3.zero;
        centerOffset += playerCoin.transform.right * (CoinCenterOffset.x * OffsetMag);
        centerOffset += playerCoin.transform.up * (CoinCenterOffset.y * OffsetMag);

        playerCoin.rigidbody.AddForceAtPosition(mCamera.transform.forward * CoinForce, playerCoin.transform.position + centerOffset - (mCamera.transform.forward * -1), ForceMode.Force);
    }
}
