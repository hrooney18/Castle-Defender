using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int wave;
    public int money;
    private float reloadMultiplier;
    public GameObject enemies;
    public bool canShoot;
    public bool hasSniper;
    public bool startOfGame;

    public bool gamePaused;
    public bool gameOver;
    private bool reloading;
    private int numGunmen;
    private int numCraftsmen;
    private int walltier;
    private int castleTier;
    [SerializeField] private int payroll;
    [SerializeField] private int maxAmmo;
    private int ammo;
    public int maxCastleHealth;
    public int castleHealth;

    [SerializeField] private Slider castleHealthSlider;
    [SerializeField] private Slider ammoSlider;
    [SerializeField] private TextMeshProUGUI castleHealthText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI gunmanText;
    [SerializeField] private TextMeshProUGUI craftsmanText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI upgradeWallCostText;
    [SerializeField] private TextMeshProUGUI fortifyCostText;
    [SerializeField] private TextMeshProUGUI payrollText;
    [SerializeField] private TextMeshProUGUI waveUIText;
    [SerializeField] private TextMeshProUGUI gameOverWaveText;
    [SerializeField] private GameObject openingUI;

    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject craftsmanPrefab;
    [SerializeField] private GameObject gunmanPrefab1;
    [SerializeField] private GameObject gunmanPrefab2;
    [SerializeField] private GameObject gunmanPrefab3;
    [SerializeField] private GameObject gunmen;
    [SerializeField] private GameObject craftsmen;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject waveUI;

    [SerializeField] private GameObject repairWallButton;
    [SerializeField] private GameObject repairWallDarkButton;
    [SerializeField] private GameObject sniperButton;
    [SerializeField] private GameObject sniperDarkButton;
    [SerializeField] private GameObject fortifyButton;
    [SerializeField] private GameObject fortifyDarkButton;

    private WaveSpawner waveSpawner;
    // Start is called before the first frame update
    void Start()
    {
        waveSpawner = GameObject.FindObjectOfType<WaveSpawner>();
        wave = 1;
        reloadMultiplier = 8f;
        startOfGame = true;
        gamePaused = true;
        hasSniper = false;
        gameOver = false;
        reloading = false;
        canShoot = true;
        payroll = 0;
        money = 0;
        walltier = 1;
        castleTier = 1;
        moneyText.text = "" + money;
        castleHealthSlider.maxValue = maxCastleHealth;
        castleHealthSlider.value = maxCastleHealth;
        castleHealth = maxCastleHealth;
        castleHealthText.text = "" + castleHealth + " / " + maxCastleHealth;
        payrollText.text = "$" + payroll;
        ammo = maxAmmo;
        ammoSlider.value = maxAmmo;
        ammoText.text = "" + ammo + " / " + maxAmmo;
        shopUI.SetActive(false);
        gameOverUI.SetActive(false);
        waveUI.SetActive(false);

        StartCoroutine("StartWave");
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamePaused && !gameOver)
        {
            if (Input.GetMouseButtonDown(0))
                ShootGun();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                canShoot = false;
                reloading = true;
            }

            if (reloading)
            {
                ammoSlider.value = Mathf.MoveTowards(ammoSlider.value, ammoSlider.maxValue, reloadMultiplier * Time.deltaTime);
                if (ammoSlider.value == ammoSlider.maxValue)
                {
                    ReloadGun();
                    reloading = false;
                }
            }
        }
    }

    public void DamageCastle(int damage)
    {
        castleHealth -= damage;
        castleHealth = Mathf.Clamp(castleHealth, 0, maxCastleHealth);
        castleHealthSlider.value = castleHealth;
        castleHealthText.text = "" + castleHealth + " / " + maxCastleHealth;
        if (castleHealth <= 0)
            EndGame();
    }

    public void ShootGun()
    {
        if (canShoot)
        {
            ammo--;
            ammoSlider.value = ammo;
            ammoText.text = "" + ammo + " / " + maxAmmo;
            if (ammo <= 0)
                canShoot = false;
        }
    }

    private void ReloadGun()
    {
        ammo = maxAmmo;
        ammoSlider.value = ammo;
        ammoText.text = "" + ammo + " / " + maxAmmo;
        canShoot = true;
    }

    public void AddMoney(int moneyAdded)
    {
        money += moneyAdded;
        moneyText.text = "" + money;
    }

    public void RepairCastle(int repairAmount)
    {
        castleHealth += repairAmount;
        castleHealth = Mathf.Clamp(castleHealth, 0, maxCastleHealth);
        castleHealthSlider.value = castleHealth;
        castleHealthText.text = "" + castleHealth + " / " + maxCastleHealth;
    }

    public void BuyUpgrade(string upgrade)
    {
        switch (upgrade)
        {
            case "clipsize":
                if (CanBuy(1000))
                {
                    money -= 1000;
                    maxAmmo++;
                    ammoSlider.maxValue = maxAmmo;
                    ammo = maxAmmo;
                    ammoSlider.value = ammo;
                    ammoText.text = "" + ammo + " / " + maxAmmo;
                }
                break;
            case "repair":
                if (CanBuy(800))
                {
                    if (castleHealth < maxCastleHealth)
                    {
                        RepairCastle(20);
                        money -= 800;
                    }
                }
                break;
            case "upgradewall":
                switch (walltier)
                {
                    case 1:
                        if (CanBuy(3000))
                        {
                            walltier++;
                            maxCastleHealth += 50;
                            castleHealthSlider.maxValue = maxCastleHealth;
                            RepairCastle(50);
                            upgradeWallCostText.text = "8000";
                            money -= 3000;
                        }
                        break;
                    case 2:
                        if (CanBuy(8000))
                        {
                            walltier++;
                            maxCastleHealth += 50;
                            castleHealthSlider.maxValue = maxCastleHealth;
                            RepairCastle(50);
                            upgradeWallCostText.text = "12000";
                            money -= 8000;
                        }
                        break;
                    case 3:
                        if (CanBuy(12000))
                        {
                            walltier++;
                            maxCastleHealth += 50;
                            castleHealthSlider.maxValue = maxCastleHealth;
                            RepairCastle(50);
                            money -= 12000;
                            repairWallButton.SetActive(false);
                            repairWallDarkButton.SetActive(true);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case "sniper":
                if (CanBuy(75000))
                {
                    money -= 75000;
                    hasSniper = true;
                    sniperButton.SetActive(false);
                    sniperDarkButton.SetActive(true);
                }
                break;
            case "fortify":
                switch (castleTier)
                {
                    case 1:
                        if (CanBuy(50000))
                        {
                            castleTier++;
                            maxCastleHealth += 375;
                            castleHealthSlider.maxValue = maxCastleHealth;
                            RepairCastle(375);
                            fortifyCostText.text = "100000";
                            money -= 50000;
                        }
                        break;
                    case 2:
                        if (CanBuy(100000))
                        {
                            castleTier++;
                            maxCastleHealth += 375;
                            castleHealthSlider.maxValue = maxCastleHealth;
                            RepairCastle(375);
                            money -= 100000;
                            fortifyButton.SetActive(false);
                            fortifyDarkButton.SetActive(true);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case "gunman":
                if (CanBuy(2000))
                {
                    money -= 2000;
                    payroll += 150;
                    payrollText.text = "$" + payroll;
                    numGunmen++;
                    gunmanText.text = "" + numGunmen + " gunmen";
                    int rand = Random.Range(0, 3);
                    GameObject gunman;
                    switch (rand)
                    {
                        case 0:
                            gunman = Instantiate(gunmanPrefab1, gunmanPrefab1.transform.position, Quaternion.identity) as GameObject;
                            gunman.transform.SetParent(gunmen.transform);
                            break;
                        case 1:
                            gunman = Instantiate(gunmanPrefab2, gunmanPrefab2.transform.position, Quaternion.identity) as GameObject;
                            gunman.transform.SetParent(gunmen.transform);
                            break;
                        case 2:
                            gunman = Instantiate(gunmanPrefab3, gunmanPrefab3.transform.position, Quaternion.identity) as GameObject;
                            gunman.transform.SetParent(gunmen.transform);
                            break;
                    }
                }
                break;
            case "craftsman":
                if (CanBuy(8000))
                {
                    money -= 8000;
                    payroll += 800;
                    payrollText.text = "$" + payroll;
                    numCraftsmen++;
                    craftsmanText.text = "" + numCraftsmen + " craftsmen";
                    GameObject craftsman = Instantiate(craftsmanPrefab, new Vector3(0, 0, -25), Quaternion.identity) as GameObject;
                    craftsman.transform.SetParent(craftsmen.transform);
                }
                break;
            default:
                break;
        }
        moneyText.text = "" + money;
    }

    private bool CanBuy(int cost)
    {
        if (money - cost >= 0)
            return true;
        else return false;
    }

    public void OpenShop()
    {
        Time.timeScale = 0;
        gamePaused = true;
        money -= payroll;
        moneyText.text = "" + money;
        ammo = maxAmmo;
        ammoSlider.value = ammo;
        ammoText.text = "" + ammo + " / " + maxAmmo;
        shopUI.SetActive(true);
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
        moneyText.text = "" + money;
        castleHealthSlider.value = castleHealth;
        castleHealthText.text = "" + castleHealth + " / " + maxCastleHealth;
        gunmanText.text = "" + numGunmen + " gunmen";
        craftsmanText.text = "" + numCraftsmen + " craftsmen";
        wave++;
        waveText.text = "Day " + wave;

        StartCoroutine("StartWave");
    }

    public IEnumerator StartWave()
    {
        waveUIText.text = "Day " + wave;
        waveText.text = "Day " + wave;
        waveUI.SetActive(true);

        Time.timeScale = 1;

        yield return new WaitForSeconds(2f);

        waveUI.SetActive(false);
        waveSpawner.AssignWave();
        gamePaused = false;
        startOfGame = false;
    }

    private void EndGame()
    {
        Time.timeScale = 0;
        gameOver = true;
        if (wave == 2)
            gameOverWaveText.text = "You protected the castle for " + (wave-1) + " day.";
        else
            gameOverWaveText.text = "You protected the castle for " + (wave-1) + " days.";
        gameOverUI.SetActive(true);
    }

    public void HandleButtonClick(string button)
    {
        if (button == "reset")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene("StartScene");
    }
}
