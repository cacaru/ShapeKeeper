using CUSTOM_DATA;

public class Enemy
{
    public int id;
    public Enemy_Type type;
    public float speed;
    public float hp_val;
    public int material;
    public int ethereal;

    public float material_hp;
    public float ethereal_hp;

    public Enemy(int id, Enemy_Type type, float speed, float hp_val, int material, int ethereal) {
        this.id = id;
        this.type = type;
        this.speed = speed;
        this.hp_val = hp_val;
        this.material = material;
        this.ethereal = ethereal;

        material_hp = hp_val * material / 100;
        ethereal_hp = hp_val * ethereal / 100;
    }
}