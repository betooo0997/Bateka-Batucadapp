using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pie_Chart : MonoBehaviour
{
    [SerializeField]
    GameObject pie_part_prefab;

    [SerializeField]
    GameObject pie_base;

    Votable votable;

    public void Initialize(Votable votable)
    {
        this.votable = votable;

        if (votable.Vote_Privacy == Privacy.Secret)
        {
            gameObject.SetActive(false);
            return;
        }

        float rotation = 0;

        for(int x = 0; x < votable.Vote_Types.Count; x++)
        {
            Pie_Chart_Part part = Instantiate(pie_part_prefab, transform).GetComponent<Pie_Chart_Part>();
            part.Descriptor = votable.Vote_Types[x];

            int total_votes = 0;

            foreach (List<User.User_Information> list in votable.Vote_Voters)
                foreach (User.User_Information user in list)
                    total_votes++;

            float percentage = (float)votable.Vote_Voters[x].Count / (float)total_votes;
            part.GetComponent<Image>().fillAmount = percentage;
            part.transform.localRotation.eulerAngles.Set(0, 0, rotation);

            rotation += 360 * percentage;
        }
    }
}
