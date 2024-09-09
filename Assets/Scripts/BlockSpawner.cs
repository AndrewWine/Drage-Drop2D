using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance;

    public GameObject blockPrefab;

    // Các button
    public Button redButton;
    public Button greenButton;
    public Button blueButton;

    // Các TMP_Text để hiển thị số lượng block còn lại
    public TMP_Text redButtonText;
    public TMP_Text greenButtonText;
    public TMP_Text blueButtonText;

    // Các màu block tương ứng
    private Color redColor;
    private Color greenColor;
    private Color blueColor;

    // Số lượng block cho mỗi loại
    public int redBlockCount = 10;
    public int greenBlockCount = 10;
    public int blueBlockCount = 10;

    public ObjectPool redPool;
    public ObjectPool greenPool;
    public ObjectPool bluePool;

    private GameObject currentBlock;
    private Color currentBlockColor;

    private InputHandler inputHandler;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateButtonText();
        inputHandler = FindObjectOfType<InputHandler>();
        if (inputHandler != null)
        {
            inputHandler.OnLeftClick += HandleLeftClick;
            inputHandler.OnRightClick += HandleRightClick;
        }

        // Gọi LoadGameState để khôi phục trạng thái khi trò chơi bắt đầu

        // Lấy màu từ các Image trong Button
        redColor = GetImageColorFromButton(redButton);
        greenColor = GetImageColorFromButton(greenButton);
        blueColor = GetImageColorFromButton(blueButton);

        Debug.Log("Red Color: " + redColor);
        Debug.Log("Green Color: " + greenColor);
        Debug.Log("Blue Color: " + blueColor);

        // Khởi tạo pool cho các loại block
        redPool = CreatePool();
        greenPool = CreatePool();
        bluePool = CreatePool();

        // Gán màu sắc cho các pool
        redPool.SetColor(redColor);
        greenPool.SetColor(greenColor);
        bluePool.SetColor(blueColor);

        // Gán sự kiện click cho button
        redButton.onClick.AddListener(() => PrepareBlock(redPool, ref redBlockCount, redButtonText, redColor));
        greenButton.onClick.AddListener(() => PrepareBlock(greenPool, ref greenBlockCount, greenButtonText, greenColor));
        blueButton.onClick.AddListener(() => PrepareBlock(bluePool, ref blueBlockCount, blueButtonText, blueColor));

        // Cập nhật hiển thị số lượng khối ban đầu
        UpdateButtonText();
    }

    void HandleLeftClick()
    {
        if (currentBlock != null)
        {
            PlaceBlock();
        }
    }

    void HandleRightClick()
    {
        CheckAndReturnBlock();
    }

    void Update()
    {
        if (currentBlock != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            currentBlock.transform.position = mousePos;
        }
    }

    // Tạo pool cho block
    ObjectPool CreatePool()
    {
        GameObject poolContainer = new GameObject("Pool");
        ObjectPool pool = poolContainer.AddComponent<ObjectPool>();
        pool.prefab = blockPrefab;
        pool.poolSize = 10;
        return pool;
    }

    // Lấy màu từ Image của Button
    private Color GetImageColorFromButton(Button button)
    {
        Image image = button.GetComponentInChildren<Image>();
        if (image != null)
        {
            return image.color;
        }
        else
        {
            Debug.LogWarning("No Image component found in button: " + button.name);
            return Color.white; // Trả về màu trắng nếu không tìm thấy Image
        }
    }

    // Hàm chuẩn bị block nhưng chưa đặt ra, chỉ di chuyển theo chuột
    void PrepareBlock(ObjectPool pool, ref int blockCount, TMP_Text buttonText, Color blockColor)
    {
        if (currentBlock != null || blockCount <= 0) return; // Không tạo block nếu block đang tồn tại hoặc hết block

        // Lấy block từ pool
        currentBlock = pool.GetObject();
        SpriteRenderer sr = currentBlock.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = blockColor;
        }

        // Tắt collider khi block đang di chuyển theo chuột
        Collider2D col = currentBlock.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Giảm số lượng block và cập nhật Text hiển thị
        blockCount--;
        buttonText.text = blockCount.ToString();
        currentBlockColor = blockColor;
    }

    // Đặt block và dừng di chuyển theo chuột
    void PlaceBlock()
    {
        // Bật lại collider khi block được đặt xuống
        Collider2D col = currentBlock.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        currentBlock = null; // Reset currentBlock để chuẩn bị cho lần tạo tiếp theo
    }

    // Cập nhật hiển thị số lượng block trên các button
    public void UpdateButtonText()
    {
        redButtonText.text = redBlockCount.ToString();
        greenButtonText.text = greenBlockCount.ToString();
        blueButtonText.text = blueBlockCount.ToString();
    }

    // Kiểm tra và trả block về pool khi chuột phải click vào block
    void CheckAndReturnBlock()
    {
        // Tạo raycast từ vị trí chuột
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Block")) // Kiểm tra tag của block
            {
                // Xác định màu block để cập nhật số lượng
                SpriteRenderer sr = hitObject.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color blockColor = sr.color;
                    if (blockColor == redColor)
                    {
                        redBlockCount++;
                        redButtonText.text = redBlockCount.ToString();
                    }
                    else if (blockColor == greenColor)
                    {
                        greenBlockCount++;
                        greenButtonText.text = greenBlockCount.ToString();
                    }
                    else if (blockColor == blueColor)
                    {
                        blueBlockCount++;
                        blueButtonText.text = blueBlockCount.ToString();
                    }
                }

                // Trả block về pool
                ObjectPool pool = GetPoolFromColor(sr.color);
                pool.ReturnObject(hitObject);
            }
        }
    }

    // Lấy pool từ màu
    ObjectPool GetPoolFromColor(Color color)
    {
        if (color == redColor) return redPool;
        if (color == greenColor) return greenPool;
        if (color == blueColor) return bluePool;
        return null;
    }
    
    

}
