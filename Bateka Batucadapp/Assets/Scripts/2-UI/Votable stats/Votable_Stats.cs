#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Votable_Stats : MonoBehaviour
{
    [SerializeField]
    Transform pie_part_parent, pie_part_description_parent, voter_list_parent;

    [SerializeField]
    GameObject pie_part_prefab, bold_text_prefab, semibold_text_prefab;

    Votable votable;

    static List<Color> colors;

    void Start()
    {
        if (Database_Handler.Selected_Data.GetType().IsSubclassOf(typeof(Votable)))
            Initialize((Votable)Database_Handler.Selected_Data);
        else
            Debug.LogError(Database_Handler.Selected_Data.GetType().ToString() + " is not a base class of " + typeof(Votable).ToString());
    }

    public void Initialize(Votable votable)
    {
        if (votable.Privacy == Privacy.Secret && User.User_Info.Role == User.User_Role.default_)
        {
            gameObject.SetActive(false);
            return;
        }

        this.votable = votable;

        switch (votable.Votable_Type)
        {
            case Votable_Type.Binary:
                colors = new List<Color>
                {
                    Data_UI.color_rejected(1),
                    Data_UI.color_affirmed(1),
                };
                break;

            case Votable_Type.Multiple:
                colors = new List<Color>
                {
                    Data_UI.color_alternative_0(1),
                    Data_UI.color_affirmed(1),
                    Data_UI.color_alternative_1(1),
                    Data_UI.color_alternative_2(1),
                    Data_UI.color_rejected(1),
                    Data_UI.color_alternative_3(1)
                };
                break;
        }

        float rotation = 0;

        List<User.User_Information> not_voted = new List<User.User_Information>(User.Users_Info);
        not_voted.Add(User.User_Info);

        for (int x = 0; x < votable.Vote_Types.Count; x++)
        {
            foreach (User.User_Information user in votable.Vote_Voters[x])
                not_voted.Remove(not_voted.Find(a => a.Id == user.Id));

            Pie_Chart_Part part = Instantiate(pie_part_prefab, pie_part_parent).GetComponent<Pie_Chart_Part>();
            part.Descriptor = votable.Vote_Types[x];

            float percentage = (float)votable.Vote_Voters[x].Count / (float)User.Users_Info.Count;
            Image image = part.GetComponent<Image>();
            image.fillAmount = percentage;
            image.color = colors[x];
            part.transform.localRotation = Quaternion.Euler(0, 0, rotation);

            Text pie_info = Instantiate(bold_text_prefab, pie_part_description_parent).GetComponent<Text>();
            pie_info.text = Utils.Translate(part.Descriptor) + ": " + votable.Vote_Voters[x].Count.ToString();
            pie_info.color = Utils.Darken_Color(colors[x], 0.25f);

            if (votable.Vote_Voters[x].Count > 0 && (votable.Privacy == Privacy.Public || User.User_Info.Role == User.User_Role.admin))
            {
                Text vote_list = Instantiate(bold_text_prefab, voter_list_parent).GetComponent<Text>();
                vote_list.color = Utils.Darken_Color(colors[x], 0.25f);
                vote_list.text = Utils.Translate(part.Descriptor) + ":";

                vote_list = Instantiate(semibold_text_prefab, voter_list_parent).GetComponent<Text>();

                foreach (User.User_Information user in votable.Vote_Voters[x])
                    vote_list.text += " " + user.Name + ",";

                if (votable.Vote_Voters[x].Count > 0)
                    vote_list.text = vote_list.text.Substring(0, vote_list.text.Length - 1);
            }

            rotation -= 360 * percentage;
        }

        Text pie_info_no_vote = Instantiate(bold_text_prefab, pie_part_description_parent).GetComponent<Text>();
        pie_info_no_vote.text = "Sin contestar: " + not_voted.Count.ToString();
        pie_info_no_vote.color = Data_UI.color_not_answered(1);
        pie_info_no_vote.transform.SetAsFirstSibling();

        if(not_voted.Count > 0 && votable.Privacy == Privacy.Public || User.User_Info.Role == User.User_Role.admin)
        {
            Text no_vote_list = Instantiate(bold_text_prefab, voter_list_parent).GetComponent<Text>();
            no_vote_list.color = Data_UI.color_not_answered(1);
            no_vote_list.text = "Sin contestar:";

            no_vote_list = Instantiate(semibold_text_prefab, voter_list_parent).GetComponent<Text>();

            foreach (User.User_Information user in not_voted)
                no_vote_list.text += " " + user.Name + ",";

            no_vote_list.text = no_vote_list.text.Substring(0, no_vote_list.text.Length - 1);
        }
    }
}
