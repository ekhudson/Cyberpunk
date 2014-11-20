using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
    public GameObject PlayerCoinPrefab;
    public BoxCollider CoinBounds;
    public LayerMask CoinLayerMask;
    public float CoinForceMin = 5f;
    public float CoinForceMax = 25f;
    public Vector3 PlayerCoinStartingRotation = new Vector3(90f, 0f, 0f);
    public float PlayerCoinDistanceFromCamera = 10f;
    public float ShotDelay = 5f;
    public Vector3 ShotReloadOffset = new Vector3(0.1f, 0.1f, 0f);
    public Vector3 CoinCenterOffset = Vector3.zero;
    public float PanSpeed = 0.25f;
    public float OffsetMag = 2f;
    public float MaxAngularVelocity = 35f;
    public float StrengthBarHeight = 32f;
    public Color StrengthBarColor = Color.yellow;

    private bool mNeedReload = false;
    private Ray mRay;
    private RaycastHit mHit;

    private float mCurrentStrength = 1f;

    private GameObject mPreviewCoin;
    private Renderer mPreviewRenderer;
    private float mCountHeight = 0f;
    private Camera mCamera;
    private GameObject mInventoryCoin;
    private float mCoinRadius = 10f;

    private float mCurrentReloadTime = 0f;
    private Vector3 mCurrentReloadPosition = Vector3.zero;
    private Vector3 mPreviousForceAmount = Vector3.zero;

    private bool mMouseOnCoin = false;

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
        mInventoryCoin.collider.isTrigger = true;
        mInventoryCoin.transform.position = mCamera.ViewportToWorldPoint(ShotReloadOffset);

        mCoinRadius = PlayerCoinPrefab.renderer.bounds.extents.x;

    }

	private void LateUpdate()
    {
        DrawInventory();

        mCurrentStrength = Mathf.Clamp((mCurrentStrength + Input.GetAxis("Mouse ScrollWheel")), 0f, 1f);

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
                //Pan();
            }

            Vector3 mousePos = Input.mousePosition;

            mRay = mCamera.ScreenPointToRay(mousePos);
            mHit = new RaycastHit();
            
            if (Physics.Raycast(mRay, out mHit, 100f, CoinLayerMask))
            {
                if (mHit.collider.gameObject != mPreviewCoin.gameObject)
                {
                    mMouseOnCoin = false;
                    return;
                }
                else
                {
                    CoinCenterOffset = mPreviewCoin.transform.InverseTransformPoint(mHit.point);
                    CoinCenterOffset.y = CoinCenterOffset.z * -1; //for some reason InverseTransformPoint puts y in z?
                    CoinCenterOffset.z = 0f;
                    mMouseOnCoin = true;
                }
            }
            else
            {
                mMouseOnCoin = false;
                return;
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
        DrawStrength();

        if (mNeedReload || (mCurrentReloadTime < ShotDelay) || !mMouseOnCoin)
        {
            return;
        }

        Vector3 pos = mPreviewCoin.transform.position;   

        pos += mCamera.transform.up * CoinCenterOffset.y;
        pos += mCamera.transform.right * CoinCenterOffset.x;
       
        pos = mCamera.WorldToScreenPoint(pos);   
              
        pos.y = mCamera.pixelHeight - pos.y;

        pos.y -= 4f;
        pos.x -= 4f;

        GUI.color = Color.red;
        GUI.Label(new Rect(pos.x, pos.y, 8f, 8f), "X", GUI.skin.box);
        GUI.color = Color.white;

    }

    private void DrawStrength()
    {
        float maxWidth = (mCamera.pixelWidth - (StrengthBarHeight * 2));
        Rect barRect = new Rect(StrengthBarHeight, mCamera.pixelHeight - StrengthBarHeight, maxWidth, StrengthBarHeight);

        GUI.color = Color.gray;
        GUI.Box(barRect, string.Empty);

        float currentWidth = Mathf.Lerp(0f, maxWidth, mCurrentStrength);

        barRect.width = currentWidth;

        GUI.color = StrengthBarColor;
        GUI.Box(barRect, string.Empty);

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

    private void OnDrawGizmos()
    {
        if (mCamera == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(mCamera.transform.position, mCamera.transform.position + (mCamera.transform.forward * 10f));
        Gizmos.color = Color.white;

        Vector3 force = mCamera.transform.forward * CoinForceMax;
        Vector3 forcePos = mPreviewCoin.transform.position;
        forcePos += mPreviewCoin.transform.right * (CoinCenterOffset.x * OffsetMag);
        forcePos += mPreviewCoin.transform.up * (CoinCenterOffset.y * OffsetMag);
        forcePos -= (mCamera.transform.forward *  2);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(forcePos, 0.05f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(forcePos, forcePos + force);
        Gizmos.color = Color.white;
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

        Vector3 force = mCamera.transform.forward * (Mathf.Lerp(CoinForceMin, CoinForceMax, mCurrentStrength));

        mPreviousForceAmount = force;

        playerCoin.rigidbody.AddForceAtPosition(force, playerCoin.transform.position + centerOffset - (mCamera.transform.forward * 2), ForceMode.VelocityChange);

    }
}
