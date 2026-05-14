using UnityEngine;

public class LoadNode
{
    public bool can_go;
    // tilemap_pos
    public Vector2 grid_pos;
    // monster_load pos 
    public int x;
    public int y;

    // f = g + h 
    // h == 목적지까지 남은 값
    // g == 현재까지의 값
    // ==> g가 최저값이면서 h가 가장 작은 값들을 우선적으로 골라봅시다.
    public int H;
    public int G;

    public int F { get { return G + H; } }

    public LoadNode parent;

    // 생성자
    public LoadNode(bool can_go, Vector2 grid_pos, int x, int y) {
        this.can_go = can_go;
        this.grid_pos = grid_pos;
        this.x = x;
        this.y = y;
    }
}
