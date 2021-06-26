using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HackGameState
{
    Init,
    Setup,
    Input,
    Pending,
    End
}
public class HackBoard : MonoBehaviour
{
    [SerializeField]
    private HackBlock block;

    private const float StateSwitchDelayTime = 3.0f, GameTimer = 5.0f, SetupUpdateDelay = 1.0f, ErrorTextShow = 0.5f;
    private const int NumberOfTilesToHack = 6, maxMental = 100, dmgMental = 10;

    private float GameVisualTimer = 0.0f, StateTimer = 0.0f, SetupUpdateTimer = 0.0f, ErrorTextTimer = 0.0f;

    private int w = 6, h = 6, NumberOfTilesSetUp, NumberOfHackedTiles, WrongTiles, currentMental;

    private HackBlock[,] grid;

    private HackGameState state;

    public HealthBarScript healthBar;

    public Text txtStateTimer, txtGameTimer, txtGameState, txtEvaluation;
    void Start()
    {
        grid = new HackBlock[w, h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                grid[i, j] = Instantiate(block, new Vector2(i - (w / 2) + 4.5f, j - (h / 2) + 0.5f), Quaternion.identity);
                grid[i, j].Selected = false;
                grid[i, j].Show = true;
                grid[i, j].Wrong = false;
                grid[i, j].i = i;
                grid[i, j].j = j;
            }
        }
        txtGameTimer.text = "System.Time = 0;";
        txtStateTimer.text = "System.State.Time = 0;";
        txtGameState.text = "System.Status = Status.INIT;";

        NumberOfTilesSetUp = NumberOfHackedTiles = WrongTiles = 0;

        currentMental = maxMental;
        healthBar.SetMaxHealth(maxMental);

        StateTimer = StateSwitchDelayTime;
        state = HackGameState.Init;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case HackGameState.Init:
                GameVisualTimer = GameTimer;
                if (StateTimer > 0f)
                {
                    txtStateTimer.text = "System.State.Time = " + ((int)StateTimer + 1).ToString() + ";";
                    StateTimer -= Time.deltaTime;
                }
                else
                {
                    txtStateTimer.text = "System.State.Time = 0;";
                    txtGameState.text = "System.Status = Status.DEBUG;";
                    StateTimer = StateSwitchDelayTime;
                    Debug.Log("Setting up game board...");
                    SetupUpdateTimer = SetupUpdateDelay;
                    state = HackGameState.Setup;
                }
                break;
            case HackGameState.Setup:
                if (NumberOfTilesSetUp < NumberOfTilesToHack)
                {
                    if (SetupUpdateTimer <= 0)
                    {
                        int rndW, rndH;
                        do
                        {
                            rndW = UnityEngine.Random.Range(0, w);
                            rndH = UnityEngine.Random.Range(0, h);
                        } while (grid[rndW, rndH].blockType != BlockType.Empty);
                        grid[rndW, rndH].blockType = getBlockType();
                        NumberOfTilesSetUp++;
                        SetupUpdateTimer = SetupUpdateDelay;
                    }
                    else
                        SetupUpdateTimer -= Time.deltaTime;
                }
                else
                {
                    StateTimer -= Time.deltaTime;
                    if (StateTimer > 0f)
                    {
                        txtStateTimer.text = "System.State.Time = " + ((int)StateTimer + 1).ToString() + ";";
                    }
                    else
                    {
                        for (int i = 0; i < w; i++)
                        {
                            for (int j = 0; j < h; j++)
                            {
                                grid[i, j].Show = false;
                            }
                        }
                        Debug.Log("Game start!");
                        NumberOfHackedTiles = 0;
                        txtStateTimer.text = "System.State.Time = 0;";
                        txtGameState.text = "System.Status = Status.PATCH;";
                        state = HackGameState.Input;
                    }
                }
                break;
            case HackGameState.Input:
                txtGameTimer.text = "System.Time = " + ((int)GameVisualTimer + 1).ToString() + ";";
                if ((GameVisualTimer > 0f) && (NumberOfHackedTiles < NumberOfTilesToHack) && (healthBar.GetHealth() > 0))
                    GameVisualTimer -= Time.deltaTime;
                else
                {
                    if (NumberOfHackedTiles == NumberOfTilesToHack)
                    {
                        StateTimer = StateSwitchDelayTime;
                        txtGameState.text = "System.Status = Status.EVAL;";

                        txtEvaluation.text = WrongTiles.ToString() + " error(s) found. Restarting...";

                        state = HackGameState.Pending;
                    }
                    
                    if ((GameVisualTimer <= 0) || (healthBar.GetHealth() == 0))
                    {
                        txtGameTimer.text = "System.Time = 0;";
                        Debug.Log("Game over!");
                        if (GameVisualTimer <= 0)
                            txtGameState.text = "ByteDef Patch Module ended with exit code (0).";
                        else
                        {
                            if (healthBar.GetHealth() <= 0)
                                txtEvaluation.text = "FATAL ERROR - ByteDef has encountered a problem and needs to stop. (Memory Overflow Error)\nDISCONNECTED - Mental breakdown detected in User.";
                        }
                        state = HackGameState.End;
                    }
                }

                if (ErrorTextTimer <= 0)
                    txtEvaluation.text = String.Empty;
                else
                    ErrorTextTimer -= Time.deltaTime;
                break;
            case HackGameState.Pending:
                txtStateTimer.text = "System.State.Time = " + ((int)StateTimer + 1).ToString() + ";";
                if (StateTimer > 0)
                {           
                    for (int i = 0; i < w; i++)
                    {
                        for (int j = 0; j < h; j++)
                        {
                            if (grid[i, j].Wrong)
                            {
                                if (grid[i, j].blockType != BlockType.Empty)
                                {
                                    grid[i, j].Wrong = false;
                                    grid[i, j].Show = true;
                                }
                            }
                            else
                                grid[i, j].Show = true;
                        }
                    }
                    StateTimer -= Time.deltaTime;
                }
                else
                {
                    txtGameState.text = "System.Status = Status.SETUP;";
                    txtStateTimer.text = "System.State.Time = 0;";
                    WrongTiles = 0;
                    txtEvaluation.text = String.Empty;
                    for (int i = 0; i < w; i++)
                    {
                        for (int j = 0; j < h; j++)
                        {
                            grid[i, j].blockType = BlockType.Empty;
                            grid[i, j].Wrong = false;
                            grid[i, j].Show = true;
                        }
                    }
                    NumberOfTilesSetUp = 0;
                    StateTimer = StateSwitchDelayTime;
                    SetupUpdateTimer = SetupUpdateDelay; 
                    state = HackGameState.Setup;
                }
                break;
            case HackGameState.End:
                break;
        }
    }

    BlockType getBlockType()
    {
        switch(UnityEngine.Random.Range(4, 8))
        {
            case 4:
                return BlockType.East;
            case 5:
                return BlockType.South;
            case 6:
                return BlockType.West;
            case 7:
                return BlockType.North;
            default:
                return getBlockType();
        }
    }

    public HackGameState getState() { return state; }
    public void ReduceProgess() { NumberOfHackedTiles++; }
    public void onWrongTile() {
        WrongTiles++;
        txtEvaluation.text = "ByteInputException: Input type does not match (0x00004200)";
        ErrorTextTimer = ErrorTextShow;
        currentMental -= dmgMental;
        healthBar.SetHealth(currentMental);
    }
}
