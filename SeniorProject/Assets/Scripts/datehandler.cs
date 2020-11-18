using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class datehandler
{
    private static int levelTally;

    public static int levelT
    {
        get
        {
            return levelTally;
        }
        set
        {
            levelTally = value;
        }
    }
}
