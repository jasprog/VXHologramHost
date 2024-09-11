using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class EntityResponse
{

    public string CategoryKey; 
    public string TextResponse; 
    public string AnimationTrigger; 

}
[Serializable]
public class EntityResponses
{

    public EntityResponse[] responses; 

}