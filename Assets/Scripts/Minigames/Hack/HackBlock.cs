using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BlockType
{
    Empty,
    Wrong = 2,
    Selected,
    East,
    South,
    West,
    North,
    Exclaim,
    Question,
    Plus,
    Minus,
    At,
    Sharp,
    Dollar,
    Percent,
    Ampersand,
    Asterisk,
}

public class HackBlock : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> blockSprites;

    public BlockType blockType = BlockType.Empty;
    public int i, j;
    public bool Selected, Show, Wrong;
    public HackBoard parent;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        parent = GameObject.Find("HackBoard").GetComponent<HackBoard>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Selected)
        {
            if ((!Show) && (!Wrong) && (parent.getState() == HackGameState.Input))
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (blockType == BlockType.North)
                        Show = true;
                    else
                    {
                        Wrong = true;
                        parent.onWrongTile();
                    }
                    parent.ReduceProgess();
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (blockType == BlockType.South)
                        Show = true;
                    else
                    {
                        Wrong = true;
                        parent.onWrongTile();
                    }
                    parent.ReduceProgess();
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (blockType == BlockType.West)
                        Show = true;
                    else
                    {
                        Wrong = true;
                        parent.onWrongTile();
                    }
                    parent.ReduceProgess();
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (blockType == BlockType.East)
                        Show = true;
                    else
                    {
                        Wrong = true;
                        parent.onWrongTile();
                    }
                    parent.ReduceProgess();
                }
            }
        }

        Render();
    }

    private void Render()
    {
        if (Wrong)
            sr.sprite = blockSprites[(int)BlockType.Wrong];
        else
        {
            if (Selected)
                sr.sprite = blockSprites[(int)BlockType.Selected];
            else
            {
                if (Show)
                    sr.sprite = blockSprites[(int)blockType];
                else
                    sr.sprite = blockSprites[(int)BlockType.Empty];
            }
        }
    }

    private void OnMouseOver()
    {
        Selected = true;
    }

    private void OnMouseExit()
    {
        Selected = false;
    }
}
