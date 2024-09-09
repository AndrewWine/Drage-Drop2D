using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject blockPrefab; // Prefab block cần được chỉ định trong Unity Editor
    public BlockSpawner blockSpawner; // Cần chỉ định BlockSpawner từ Unity Editor
    public InputHandler inputHandler;
    public PlayButton playButton;


    //Check Bool
    public bool DataSaved;

    private void Awake()
    {
        // Kiểm tra xem có file save hay không và cập nhật biến DataSaved
        string path = Application.persistentDataPath + "/savefile.dat";
        DataSaved = File.Exists(path);
    }

    private void Start()
    {
        // Kiểm tra và tải trạng thái trò chơi nếu có file save
        if (DataSaved)
        {
            LoadGameState();
        }
        else
        {
            // Nếu không có file save, bạn có thể thực hiện hành động khởi tạo nào đó nếu cần
            Debug.Log("No save file found. Starting a new game.");
        }

        // Đăng ký sự kiện
        if (inputHandler != null)
        {
            inputHandler.NotifyESCPressed += HandleEscapePressed;
        }
        else
        {
            Debug.LogError("InputHandler is not assigned in GameManager.");
        }

        if (playButton != null)
        {
            playButton.PlayButtonPressed += HandlePlayBtnPressed;
        }
        else
        {
            Debug.LogError("PlayButton is not assigned in GameManager.");
        }
    }

    private void OnDisable()
    {
        // Hủy đăng ký sự kiện khi đối tượng bị hủy
        if (inputHandler != null)
        {
            inputHandler.NotifyESCPressed -= HandleEscapePressed;
        }

        if (playButton != null)
        {
            playButton.PlayButtonPressed -= HandlePlayBtnPressed;
        }
    }

    private void HandlePlayBtnPressed()
    {
        SceneManager.LoadScene("PlayScene");
    }

    private void HandleEscapePressed()
    {
        SaveGameState(); // Lưu trạng thái game khi bấm ESC
        Application.Quit(); // Thoát ứng dụng
        Debug.Log("Application is quitting.");
    }

    public void SaveGameState()
    {
        GameStateData data = new GameStateData();

        // Lưu số lượng block còn lại thông qua blockSpawner
        data.redBlockCount = blockSpawner.redBlockCount;
        data.greenBlockCount = blockSpawner.greenBlockCount;
        data.blueBlockCount = blockSpawner.blueBlockCount;

        // Lưu trạng thái của các block từ các pool
        SavePoolState(blockSpawner.redPool, data.blocks);
        SavePoolState(blockSpawner.greenPool, data.blocks);
        SavePoolState(blockSpawner.bluePool, data.blocks);

        string path = Application.persistentDataPath + "/savefile.dat";
        using (FileStream file = File.Create(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);
        }
    }

    private void SavePoolState(ObjectPool pool, List<BlockData> blocks)
    {
        foreach (var block in pool.GetAllObjects())
        {
            if (block != null && block.activeInHierarchy)
            {
                BlockData blockData = new BlockData(
                    block.transform.position,
                    block.transform.rotation,
                    block.GetComponent<SpriteRenderer>().color
                );
                blocks.Add(blockData);
            }
        }
    }

    public void LoadGameState()
    {
        string path = Application.persistentDataPath + "/savefile.dat";
        if (File.Exists(path))
        {
            GameStateData data;
            using (FileStream file = File.Open(path, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                data = (GameStateData)bf.Deserialize(file);
            }

            // Khôi phục số lượng block còn lại thông qua blockSpawner
            blockSpawner.redBlockCount = data.redBlockCount;
            blockSpawner.greenBlockCount = data.greenBlockCount;
            blockSpawner.blueBlockCount = data.blueBlockCount;

            // Khôi phục trạng thái của các block
            foreach (var blockData in data.blocks)
            {
                GameObject block = Instantiate(blockPrefab);
                block.transform.position = blockData.GetPosition();
                block.transform.rotation = blockData.GetRotation();
                block.GetComponent<SpriteRenderer>().color = blockData.GetColor();
            }

            // Cập nhật hiển thị số lượng block
            blockSpawner.UpdateButtonText();
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }
}
