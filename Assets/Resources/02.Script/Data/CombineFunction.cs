using CUSTOM_DATA;
using System.Diagnostics;

public class CombineFunction
{
    public int id = 0;
    public int unit_id = -1;
    public int a = -1;  // Àç·á 1
    public int b = -1;
    public int c = -1;
    public int result = -1;

    public int piece = 0;   // 101
    public int crystal = 0; // 102
    
    public int need_count = 0;

    public int[] c_function = new int[7] { 0, 0, 0, 0, 0, 0, 0 };

    public string c_function_string = "";

    public void Set_Function() {
        c_function[0] = unit_id;
        c_function[1] = a;
        c_function[2] = b;
        c_function[3] = c;
        c_function[4] = result;

        Utility.MergeSort(c_function, 0, 4);

        c_function[5] = piece;
        c_function[6] = crystal;

        c_function_string = Utility.builder.Append(c_function[0])
                                           .Append("/")
                                           .Append(c_function[1])
                                           .Append("/")
                                           .Append(c_function[2])
                                           .Append("/")
                                           .Append(c_function[3])
                                           .Append("/")
                                           .Append(c_function[4])
                                           .Append("/")
                                           .Append(c_function[5])
                                           .Append("/")
                                           .Append(c_function[6])
                                           .ToString();
        Utility.builder.Clear();
    }

    public void Init() {
        id = 0;
        a = -1;
        b = -1;
        c = -1;
        result = -1; 
        piece = 0; 
        crystal = 0;
        need_count = 0;
        c_function = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
    }
}
