using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database_Parser
{
    public class Parsed_Data
    {
        public Parsed_Data() { Node_Groups = new List<Parsed_Node_Group>(); }
        public bool Success;
        public List<Parsed_Node_Group> Node_Groups;
    }

    public class Parsed_Node_Group
    {
        public Parsed_Node_Group() { Nodes = new List<Parsed_Node>(); }
        public List<Parsed_Node> Nodes;
    }

    public struct Parsed_Node
    {
        public string Field_Name;
        public string Field_Value;
    }

    public static Parsed_Data Parse_Data(string data)
    {
        Parsed_Data result = new Parsed_Data();

        string[] data_main_sections = data.Split('|');

        if (data_main_sections[0] == "VERIFIED.")
        {

        }
        else
        {
        }

        return result;
    }
}
